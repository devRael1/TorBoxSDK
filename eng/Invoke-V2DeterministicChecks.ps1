[CmdletBinding()]
param(
    [switch]$SkipPack,
    [switch]$IncludeReleaseContract
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $repositoryRoot

$solutionPath = 'TorBoxSDK.V2.slnx'
$unitTestProjectPath = 'tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj'
$contractTestProjectPath = 'tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj'
$coreProjectPath = 'src/TorBoxSDK.V2/TorBoxSDK.V2.csproj'
$dependencyInjectionProjectPath = 'src/TorBoxSDK.DependencyInjection.V2/TorBoxSDK.DependencyInjection.V2.csproj'
$integrationTestLockPath = 'tests/TorBoxSDK.V2.' + 'Integration' + 'Tests/packages.lock.json'
$packageOutputPath = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts/v2-packages'))
$repositoryRootWithSeparator = $repositoryRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar

$requiredRelativePaths = @(
    $solutionPath,
    $unitTestProjectPath,
    $contractTestProjectPath,
    $coreProjectPath,
    $dependencyInjectionProjectPath,
    'contracts/torbox/baseline/openapi.json',
    'contracts/torbox/baseline/manifest.json',
    'contracts/torbox/relay/openapi.json',
    'contracts/torbox/relay/manifest.json',
    'contracts/torbox/search/operations.json',
    'contracts/torbox/search/manifest.json',
    'contracts/torbox/sources.json',
    'contracts/torbox/divergences.json',
    'contracts/torbox/coverage.json',
    'src/TorBoxSDK.V2/packages.lock.json',
    'src/TorBoxSDK.DependencyInjection.V2/packages.lock.json',
    'src/TorBoxSDK.V2.Examples/packages.lock.json',
    'tests/TorBoxSDK.V2.Testing/packages.lock.json',
    'tests/TorBoxSDK.V2.UnitTests/packages.lock.json',
    'tests/TorBoxSDK.V2.ContractTests/packages.lock.json',
    $integrationTestLockPath
)

function Assert-RequiredFiles {
    foreach ($relativePath in $requiredRelativePaths) {
        $absolutePath = Join-Path $repositoryRoot $relativePath
        if (-not (Test-Path -LiteralPath $absolutePath -PathType Leaf)) {
            throw "Required V2 validation file is missing: $relativePath"
        }
    }
}

function Invoke-DotNet {
    param(
        [Parameter(Mandatory)]
        [string[]]$Arguments,
        [Parameter(Mandatory)]
        [string]$FailureMessage
    )

    Write-Host ("+ dotnet {0}" -f ($Arguments -join ' '))
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw $FailureMessage
    }
}

function Assert-DescendantPackageOutputPath {
    if (-not $packageOutputPath.StartsWith($repositoryRootWithSeparator, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "The package output path must be a descendant of the repository root: $packageOutputPath"
    }

    $relativePackageOutputPath = [System.IO.Path]::GetRelativePath($repositoryRoot, $packageOutputPath)
    $pathSegments = @($relativePackageOutputPath.Split([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $currentPath = $repositoryRoot
    foreach ($pathSegment in $pathSegments) {
        $currentPath = Join-Path $currentPath $pathSegment
        if (Test-Path -LiteralPath $currentPath) {
            $currentItem = Get-Item -LiteralPath $currentPath -Force
            if ($currentItem.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
                throw "The package output path contains a reparse point: $currentPath"
            }
        }
    }

    $packageOutputParentPath = Split-Path -Parent $packageOutputPath
    if (-not (Test-Path -LiteralPath $packageOutputParentPath)) {
        New-Item -ItemType Directory -Path $packageOutputParentPath | Out-Null
    }

    $resolvedRepositoryRoot = (Resolve-Path -LiteralPath $repositoryRoot).Path
    $resolvedRepositoryRootWithSeparator = $resolvedRepositoryRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
    $resolvedPackageOutputParentPath = (Resolve-Path -LiteralPath $packageOutputParentPath).Path
    $resolvedPackageOutputPath = [System.IO.Path]::GetFullPath((Join-Path $resolvedPackageOutputParentPath (Split-Path -Leaf $packageOutputPath)))
    if (-not $resolvedPackageOutputPath.StartsWith($resolvedRepositoryRootWithSeparator, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "The resolved package output path must remain below the repository root: $resolvedPackageOutputPath"
    }

    $currentPath = $repositoryRoot
    foreach ($pathSegment in $pathSegments) {
        $currentPath = Join-Path $currentPath $pathSegment
        if (Test-Path -LiteralPath $currentPath) {
            $currentItem = Get-Item -LiteralPath $currentPath -Force
            if ($currentItem.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
                throw "The package output path contains a reparse point: $currentPath"
            }
        }
    }
}

function Get-PackageMetadata {
    param(
        [Parameter(Mandatory)]
        [string]$PackagePath
    )

    $archive = [System.IO.Compression.ZipFile]::OpenRead($PackagePath)
    try {
        $entries = @($archive.Entries)
        $nuspecEntries = @($entries | Where-Object {
            $_.FullName.EndsWith('.nuspec', [System.StringComparison]::OrdinalIgnoreCase)
        })
        if ($nuspecEntries.Count -ne 1) {
            throw "Package '$PackagePath' must contain exactly one nuspec file."
        }

        $reader = [System.IO.StreamReader]::new($nuspecEntries[0].Open())
        try {
            [xml]$nuspec = $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }

        $metadata = $nuspec.SelectSingleNode("/*[local-name()='package']/*[local-name()='metadata']")
        if ($null -eq $metadata) {
            throw "Package '$PackagePath' has no nuspec metadata."
        }

        $packageId = [string]$metadata.SelectSingleNode("./*[local-name()='id']").InnerText
        $packageVersion = [string]$metadata.SelectSingleNode("./*[local-name()='version']").InnerText
        $dependencyIds = @(
            $metadata.SelectNodes(".//*[local-name()='dependency']") |
                ForEach-Object { [string]$_.GetAttribute('id') } |
                Sort-Object -Unique
        )

        return [pscustomobject]@{
            Id = $packageId
            Version = $packageVersion
            DependencyIds = $dependencyIds
            EntryNames = @($entries | ForEach-Object { $_.FullName })
        }
    }
    finally {
        $archive.Dispose()
    }
}

function Assert-PackageContents {
    param(
        [Parameter(Mandatory)]
        [object]$Package,
        [Parameter(Mandatory)]
        [string]$ExpectedId,
        [Parameter(Mandatory)]
        [string]$ExpectedAssemblyName
    )

    if ($Package.Id -cne $ExpectedId -or $Package.Version -cne '2.0.0') {
        throw "Unexpected package identity '$($Package.Id) $($Package.Version)'."
    }

    foreach ($targetFramework in @('netstandard2.0', 'net6.0', 'net7.0', 'net8.0', 'net9.0', 'net10.0')) {
        $requiredEntry = "lib/$targetFramework/$ExpectedAssemblyName.dll"
        if ($Package.EntryNames -cnotcontains $requiredEntry) {
            throw "Package '$ExpectedId' is missing '$requiredEntry'."
        }
    }
}

function Assert-PackageDependencies {
    param(
        [Parameter(Mandatory)]
        [string]$PackageId,
        [Parameter(Mandatory)]
        [string[]]$DependencyIds,
        [Parameter(Mandatory)]
        [string[]]$AllowedDependencyIds,
        [string[]]$RequiredDependencyIds = @()
    )

    $unexpectedDependencyIds = @($DependencyIds | Where-Object { $AllowedDependencyIds -notcontains $_ })
    if ($unexpectedDependencyIds.Count -ne 0) {
        throw "Package '$PackageId' has unexpected production dependencies: $($unexpectedDependencyIds -join ', ')."
    }

    $missingDependencyIds = @($RequiredDependencyIds | Where-Object { $DependencyIds -notcontains $_ })
    if ($missingDependencyIds.Count -ne 0) {
        throw "Package '$PackageId' is missing required dependencies: $($missingDependencyIds -join ', ')."
    }
}

Assert-RequiredFiles

Invoke-DotNet @('restore', $solutionPath, '--locked-mode') 'V2 locked restore failed.'
Invoke-DotNet @('build', $solutionPath, '--configuration', 'Release', '--no-restore') 'V2 Release build failed.'
Invoke-DotNet @('test', $unitTestProjectPath, '--configuration', 'Release', '--no-build', '--no-restore') 'V2 unit tests failed.'
Invoke-DotNet @('test', $contractTestProjectPath, '--configuration', 'Release', '--no-build', '--no-restore', '--filter', 'Category!=Release') 'V2 non-release contract tests failed.'

if ($IncludeReleaseContract) {
    Invoke-DotNet @('test', $contractTestProjectPath, '--configuration', 'Release', '--no-build', '--no-restore', '--filter', 'Category=Release') 'V2 release contract tests failed.'
}
else {
    Write-Host 'V2 release contract tests are excluded by default. Re-run with -IncludeReleaseContract after cutover eligibility is declared.'
}

if ($SkipPack) {
    Write-Host 'V2 package validation was skipped.'
    return
}

Assert-DescendantPackageOutputPath
if (Test-Path -LiteralPath $packageOutputPath) {
    Remove-Item -LiteralPath $packageOutputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $packageOutputPath | Out-Null

Invoke-DotNet @('pack', $coreProjectPath, '--configuration', 'Release', '--no-build', '--no-restore', '--output', $packageOutputPath) 'V2 core package creation failed.'
Invoke-DotNet @('pack', $dependencyInjectionProjectPath, '--configuration', 'Release', '--no-build', '--no-restore', '--output', $packageOutputPath) 'V2 dependency-injection package creation failed.'

$corePackagePath = Join-Path $packageOutputPath 'TorBoxSDK.2.0.0.nupkg'
$dependencyInjectionPackagePath = Join-Path $packageOutputPath 'TorBoxSDK.DependencyInjection.2.0.0.nupkg'
foreach ($packagePath in @($corePackagePath, $dependencyInjectionPackagePath)) {
    if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
        throw "Expected V2 package is missing: $packagePath"
    }
}

$corePackage = Get-PackageMetadata $corePackagePath
$dependencyInjectionPackage = Get-PackageMetadata $dependencyInjectionPackagePath

Assert-PackageContents $corePackage 'TorBoxSDK' 'TorBoxSDK'
Assert-PackageContents $dependencyInjectionPackage 'TorBoxSDK.DependencyInjection' 'TorBoxSDK.DependencyInjection'

$coreMicrosoftExtensionsDependencies = @($corePackage.DependencyIds | Where-Object { $_ -like 'Microsoft.Extensions.*' })
if ($coreMicrosoftExtensionsDependencies.Count -ne 0) {
    throw "Package 'TorBoxSDK' must not depend on Microsoft.Extensions packages: $($coreMicrosoftExtensionsDependencies -join ', ')."
}
Assert-PackageDependencies 'TorBoxSDK' $corePackage.DependencyIds @('System.Text.Json')
Assert-PackageDependencies 'TorBoxSDK.DependencyInjection' $dependencyInjectionPackage.DependencyIds @(
    'TorBoxSDK',
    'Microsoft.Extensions.Configuration.Abstractions',
    'Microsoft.Extensions.DependencyInjection.Abstractions',
    'Microsoft.Extensions.Http',
    'Microsoft.Extensions.Options',
    'Microsoft.Extensions.Options.ConfigurationExtensions'
) @(
    'TorBoxSDK',
    'Microsoft.Extensions.Configuration.Abstractions',
    'Microsoft.Extensions.DependencyInjection.Abstractions',
    'Microsoft.Extensions.Http',
    'Microsoft.Extensions.Options',
    'Microsoft.Extensions.Options.ConfigurationExtensions'
)

Write-Host "V2 packages were validated in '$packageOutputPath'."
