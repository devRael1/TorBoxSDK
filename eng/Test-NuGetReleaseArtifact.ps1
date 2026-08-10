[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $Tag,

    [Parameter(Mandatory)]
    [string] $Commit,

    [Parameter(Mandatory)]
    [string] $PackageDirectory,

    [Parameter(Mandatory)]
    [string] $ManifestPath,

    [switch] $WriteManifest
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$packageId = 'TorBoxSDK'
$expectedAssemblyVersion = '2.0.0.0'

if ($Commit -notmatch '^[0-9a-fA-F]{40}$') {
    throw "Commit '$Commit' must be a full 40-character Git SHA."
}

function Get-FileRecord {
    param(
        [Parameter(Mandatory)]
        [System.IO.FileInfo] $File
    )

    return [pscustomobject]@{
        File = $File.Name
        Sha256 = (Get-FileHash -LiteralPath $File.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    }
}

function Read-EntryText {
    param(
        [Parameter(Mandatory)]
        [System.IO.Compression.ZipArchiveEntry] $Entry
    )

    $stream = $Entry.Open()
    $reader = [System.IO.StreamReader]::new($stream)

    try {
        return $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
        $stream.Dispose()
    }
}

function Copy-ArchiveEntry {
    param(
        [Parameter(Mandatory)]
        [System.IO.Compression.ZipArchiveEntry] $Entry,

        [Parameter(Mandatory)]
        [string] $Destination
    )

    $source = $Entry.Open()
    $target = [System.IO.File]::Open($Destination, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::Write)

    try {
        $source.CopyTo($target)
    }
    finally {
        $target.Dispose()
        $source.Dispose()
    }
}

function Get-PackageMetadata {
    param(
        [Parameter(Mandatory)]
        [System.IO.FileInfo] $Package,

        [Parameter(Mandatory)]
        [string] $ExpectedVersion,

        [Parameter(Mandatory)]
        [string] $ExpectedFileVersion,

        [Parameter(Mandatory)]
        [string] $ExpectedCommit
    )

    $archive = [System.IO.Compression.ZipFile]::OpenRead($Package.FullName)

    try {
        $nuspecEntries = @($archive.Entries | Where-Object { $_.FullName -match '^[^/]+\.nuspec$' })

        if ($nuspecEntries.Count -ne 1) {
            throw "Package '$($Package.Name)' must contain exactly one root .nuspec file."
        }

        [xml] $nuspec = Read-EntryText -Entry $nuspecEntries[0]
        $metadataNode = $nuspec.SelectSingleNode("/*[local-name()='package']/*[local-name()='metadata']")

        if ($null -eq $metadataNode) {
            throw "Package '$($Package.Name)' has no metadata node in its .nuspec."
        }

        $actualPackageId = $metadataNode.SelectSingleNode("*[local-name()='id']").InnerText
        $actualVersion = $metadataNode.SelectSingleNode("*[local-name()='version']").InnerText
        $repositoryNode = $metadataNode.SelectSingleNode("*[local-name()='repository']")

        if ($actualPackageId -cne $packageId) {
            throw "Package id '$actualPackageId' does not equal '$packageId'."
        }

        if ($actualVersion -cne $ExpectedVersion) {
            throw "Package version '$actualVersion' does not equal expected version '$ExpectedVersion'."
        }

        if (($null -eq $repositoryNode) -or ($repositoryNode.GetAttribute('commit') -cne $ExpectedCommit)) {
            throw "Package repository commit does not equal expected commit '$ExpectedCommit'."
        }

        $entryNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)

        foreach ($entry in $archive.Entries) {
            [void] $entryNames.Add($entry.FullName)
        }

        $dllEntries = @($archive.Entries | Where-Object { $_.FullName -match '^lib/[^/]+/TorBoxSDK\.dll$' })

        if ($dllEntries.Count -eq 0) {
            throw "Package '$($Package.Name)' does not contain a TorBoxSDK assembly under lib/."
        }

        $temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "torboxsdk-release-$([guid]::NewGuid().ToString('N'))"
        [System.IO.Directory]::CreateDirectory($temporaryDirectory) | Out-Null
        $assemblies = @()

        try {
            foreach ($dllEntry in $dllEntries) {
                $xmlPath = $dllEntry.FullName -replace '\.dll$', '.xml'

                if (-not $entryNames.Contains($xmlPath)) {
                    throw "Package '$($Package.Name)' is missing XML documentation '$xmlPath'."
                }

                $relativePath = $dllEntry.FullName.Replace('/', [System.IO.Path]::DirectorySeparatorChar)
                $assemblyPath = Join-Path $temporaryDirectory $relativePath
                [System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($assemblyPath)) | Out-Null
                Copy-ArchiveEntry -Entry $dllEntry -Destination $assemblyPath

                $assemblyVersion = [System.Reflection.AssemblyName]::GetAssemblyName($assemblyPath).Version.ToString()
                $fileVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($assemblyPath).FileVersion

                if ($assemblyVersion -cne $expectedAssemblyVersion) {
                    throw "Assembly '$($dllEntry.FullName)' has AssemblyVersion '$assemblyVersion', expected '$expectedAssemblyVersion'."
                }

                if ($fileVersion -cne $ExpectedFileVersion) {
                    throw "Assembly '$($dllEntry.FullName)' has FileVersion '$fileVersion', expected '$ExpectedFileVersion'."
                }

                $assemblies += [pscustomobject]@{
                    Framework = ($dllEntry.FullName -split '/')[1]
                    AssemblyVersion = $assemblyVersion
                    FileVersion = $fileVersion
                }
            }
        }
        finally {
            Remove-Item -LiteralPath $temporaryDirectory -Recurse -Force -ErrorAction SilentlyContinue
        }

        return @($assemblies | Sort-Object Framework)
    }
    finally {
        $archive.Dispose()
    }
}

function Assert-SymbolPackage {
    param(
        [Parameter(Mandatory)]
        [System.IO.FileInfo] $SymbolPackage
    )

    $archive = [System.IO.Compression.ZipFile]::OpenRead($SymbolPackage.FullName)

    try {
        $pdbEntries = @($archive.Entries | Where-Object { $_.FullName -match '^lib/[^/]+/TorBoxSDK\.pdb$' })

        if ($pdbEntries.Count -eq 0) {
            throw "Symbol package '$($SymbolPackage.Name)' does not contain a TorBoxSDK PDB under lib/."
        }
    }
    finally {
        $archive.Dispose()
    }
}

function Assert-ManifestMatches {
    param(
        [Parameter(Mandatory)]
        [pscustomobject] $Expected,

        [Parameter(Mandatory)]
        [pscustomobject] $Actual
    )

    foreach ($property in @('schemaVersion', 'tag', 'commit', 'packageId', 'version', 'assemblyVersion', 'fileVersion')) {
        if ([string] $Expected.$property -cne [string] $Actual.$property) {
            throw "Manifest property '$property' differs from the downloaded artifact."
        }
    }

    $expectedPackages = @($Expected.packages)
    $actualPackages = @($Actual.packages)

    if ($expectedPackages.Count -ne $actualPackages.Count) {
        throw 'Manifest package count differs from the downloaded artifact.'
    }

    foreach ($expectedPackage in $expectedPackages) {
        $actualPackage = @($actualPackages | Where-Object { $_.file -ceq $expectedPackage.file })

        if (($actualPackage.Count -ne 1) -or ($actualPackage[0].sha256 -cne $expectedPackage.sha256)) {
            throw "Manifest hash for '$($expectedPackage.file)' differs from the downloaded artifact."
        }
    }

    $expectedAssemblies = @($Expected.assemblies | Sort-Object framework)
    $actualAssemblies = @($Actual.assemblies | Sort-Object framework)

    if ($expectedAssemblies.Count -ne $actualAssemblies.Count) {
        throw 'Manifest assembly count differs from the downloaded artifact.'
    }

    for ($index = 0; $index -lt $expectedAssemblies.Count; $index++) {
        $expectedAssembly = $expectedAssemblies[$index]
        $actualAssembly = $actualAssemblies[$index]

        if (($expectedAssembly.framework -cne $actualAssembly.framework) -or
            ($expectedAssembly.assemblyVersion -cne $actualAssembly.assemblyVersion) -or
            ($expectedAssembly.fileVersion -cne $actualAssembly.fileVersion)) {
            throw "Manifest assembly metadata differs for '$($expectedAssembly.framework)'."
        }
    }
}

$release = & (Join-Path $PSScriptRoot 'Resolve-NuGetReleaseTag.ps1') -Tag $Tag | ConvertFrom-Json
$fullPackageDirectory = [System.IO.Path]::GetFullPath($PackageDirectory)

if (-not (Test-Path -LiteralPath $fullPackageDirectory -PathType Container)) {
    throw "Package directory '$PackageDirectory' does not exist."
}

$nupkgName = "$packageId.$($release.Version).nupkg"
$snupkgName = "$packageId.$($release.Version).snupkg"
$nupkgPath = Join-Path $fullPackageDirectory $nupkgName
$snupkgPath = Join-Path $fullPackageDirectory $snupkgName

foreach ($packagePath in @($nupkgPath, $snupkgPath)) {
    if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
        throw "Expected package '$packagePath' was not produced."
    }
}

$packageFiles = @(Get-ChildItem -LiteralPath $fullPackageDirectory -File | Where-Object { $_.Extension -in @('.nupkg', '.snupkg') })

if (($packageFiles.Count -ne 2) -or ($packageFiles.Name -notcontains $nupkgName) -or ($packageFiles.Name -notcontains $snupkgName)) {
    throw "Package directory '$PackageDirectory' must contain exactly '$nupkgName' and '$snupkgName'."
}

$nupkg = Get-Item -LiteralPath $nupkgPath
$snupkg = Get-Item -LiteralPath $snupkgPath
$assemblies = Get-PackageMetadata -Package $nupkg -ExpectedVersion $release.Version -ExpectedFileVersion $release.FileVersion -ExpectedCommit $Commit
Assert-SymbolPackage -SymbolPackage $snupkg

$actualManifest = [pscustomobject]@{
    schemaVersion = 1
    tag = $Tag
    commit = $Commit.ToLowerInvariant()
    packageId = $packageId
    version = $release.Version
    assemblyVersion = $expectedAssemblyVersion
    fileVersion = $release.FileVersion
    packages = @(
        Get-FileRecord -File $nupkg
        Get-FileRecord -File $snupkg
    )
    assemblies = @($assemblies)
}

if ($WriteManifest) {
    $manifestDirectory = Split-Path -Parent $ManifestPath

    if (-not [string]::IsNullOrWhiteSpace($manifestDirectory)) {
        [System.IO.Directory]::CreateDirectory($manifestDirectory) | Out-Null
    }

    $actualManifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $ManifestPath -Encoding utf8NoBOM
}
else {
    if (-not (Test-Path -LiteralPath $ManifestPath -PathType Leaf)) {
        throw "Manifest '$ManifestPath' does not exist."
    }

    $expectedManifest = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
    Assert-ManifestMatches -Expected $expectedManifest -Actual $actualManifest
}

Write-Host "Validated $nupkgName and $snupkgName for $Tag at $Commit."
