[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $PackageDirectory,

    [Parameter(Mandatory)]
    [string] $ManifestPath,

    [string] $Source = 'https://api.nuget.org/v3/index.json',

    [ValidateRange(5, 300)]
    [int] $VerificationTimeoutSeconds = 60
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($env:NUGET_API_KEY)) {
    throw 'NUGET_API_KEY must be provided by the OIDC authentication step.'
}

function Get-Hash {
    param(
        [Parameter(Mandatory)]
        [string] $Path
    )

    return (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()
}

function Get-PackageBaseAddress {
    param(
        [Parameter(Mandatory)]
        [string] $ServiceIndex
    )

    $index = Invoke-RestMethod -Uri $ServiceIndex -ErrorAction Stop

    foreach ($resource in @($index.resources)) {
        foreach ($resourceType in @($resource.'@type')) {
            if ($resourceType -like 'PackageBaseAddress/*') {
                return $resource.'@id'.TrimEnd([char] '/')
            }
        }
    }

    throw "NuGet service index '$ServiceIndex' does not expose a PackageBaseAddress resource."
}

function Get-RemotePackageHash {
    param(
        [Parameter(Mandatory)]
        [string] $PackageBaseAddress,

        [Parameter(Mandatory)]
        [string] $PackageId,

        [Parameter(Mandatory)]
        [string] $Version
    )

    $lowerPackageId = $PackageId.ToLowerInvariant()
    $lowerVersion = $Version.ToLowerInvariant()
    $packageUri = "$PackageBaseAddress/$lowerPackageId/$lowerVersion/$lowerPackageId.$lowerVersion.nupkg"
    $temporaryPath = [System.IO.Path]::GetTempFileName()

    try {
        Invoke-WebRequest -Uri $packageUri -OutFile $temporaryPath -MaximumRedirection 5 -ErrorAction Stop
        return Get-Hash -Path $temporaryPath
    }
    catch {
        Write-Verbose "Package '$packageUri' is not available yet: $($_.Exception.Message)"
        return $null
    }
    finally {
        Remove-Item -LiteralPath $temporaryPath -Force -ErrorAction SilentlyContinue
    }
}

function Assert-PublishedPackageHash {
    param(
        [Parameter(Mandatory)]
        [string] $PackageBaseAddress,

        [Parameter(Mandatory)]
        [string] $PackageId,

        [Parameter(Mandatory)]
        [string] $Version,

        [Parameter(Mandatory)]
        [string] $ExpectedHash,

        [Parameter(Mandatory)]
        [int] $TimeoutSeconds
    )

    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)

    do {
        $actualHash = Get-RemotePackageHash -PackageBaseAddress $PackageBaseAddress -PackageId $PackageId -Version $Version

        if ($null -ne $actualHash) {
            if ($actualHash -cne $ExpectedHash) {
                throw "NuGet package '$PackageId $Version' has SHA-256 '$actualHash', expected '$ExpectedHash'."
            }

            return
        }

        if ([DateTime]::UtcNow -ge $deadline) {
            break
        }

        Start-Sleep -Seconds 5
    }
    while ($true)

    throw "NuGet package '$PackageId $Version' was not available for SHA-256 verification within $TimeoutSeconds seconds."
}

$fullPackageDirectory = [System.IO.Path]::GetFullPath($PackageDirectory)

if (-not (Test-Path -LiteralPath $fullPackageDirectory -PathType Container)) {
    throw "Package directory '$PackageDirectory' does not exist."
}

if (-not (Test-Path -LiteralPath $ManifestPath -PathType Leaf)) {
    throw "Manifest '$ManifestPath' does not exist."
}

$manifest = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json

if (($manifest.schemaVersion -ne 1) -or [string]::IsNullOrWhiteSpace($manifest.packageId) -or [string]::IsNullOrWhiteSpace($manifest.version)) {
    throw "Manifest '$ManifestPath' is not a supported release manifest."
}

$packageRecords = @($manifest.packages)
$primaryRecords = @($packageRecords | Where-Object { $_.file -cmatch '\.nupkg$' })
$symbolRecords = @($packageRecords | Where-Object { $_.file -cmatch '\.snupkg$' })

if (($packageRecords.Count -ne 2) -or ($primaryRecords.Count -ne 1) -or ($symbolRecords.Count -ne 1)) {
    throw "Manifest '$ManifestPath' must describe exactly one .nupkg and one .snupkg."
}

$primaryPackagePath = Join-Path $fullPackageDirectory $primaryRecords[0].file
$symbolPackagePath = Join-Path $fullPackageDirectory $symbolRecords[0].file

foreach ($packagePath in @($primaryPackagePath, $symbolPackagePath)) {
    if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
        throw "Manifest package '$packagePath' does not exist."
    }
}

if ((Get-Hash -Path $primaryPackagePath) -cne $primaryRecords[0].sha256) {
    throw "Primary package '$primaryPackagePath' does not match its release manifest hash."
}

if ((Get-Hash -Path $symbolPackagePath) -cne $symbolRecords[0].sha256) {
    throw "Symbol package '$symbolPackagePath' does not match its release manifest hash."
}

& dotnet nuget push $primaryPackagePath --no-symbols --api-key $env:NUGET_API_KEY --source $Source
$primaryPushExitCode = $LASTEXITCODE

if ($primaryPushExitCode -ne 0) {
    Write-Warning "Primary package push returned exit code $primaryPushExitCode. Verifying the package already visible on NuGet before any symbol retry."
}

$packageBaseAddress = Get-PackageBaseAddress -ServiceIndex $Source
Assert-PublishedPackageHash -PackageBaseAddress $packageBaseAddress -PackageId $manifest.packageId -Version $manifest.version -ExpectedHash $primaryRecords[0].sha256 -TimeoutSeconds $VerificationTimeoutSeconds

& dotnet nuget push $symbolPackagePath --api-key $env:NUGET_API_KEY --source $Source

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Host "Published the verified primary package and candidate symbol package for $($manifest.packageId) $($manifest.version)."
