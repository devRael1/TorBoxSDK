[CmdletBinding()]
param(
    [string] $Version,

    [string] $FileVersion,

    [string] $ResultsDirectory = 'artifacts/test-results',

    [switch] $SkipPackageValidation
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($Version) -xor [string]::IsNullOrWhiteSpace($FileVersion)) {
    throw 'Version and FileVersion must be supplied together.'
}

function Invoke-DotNet {
    param(
        [Parameter(Mandatory)]
        [string[]] $Arguments
    )

    & dotnet @Arguments

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

function Get-PackageVersion {
    param(
        [string[]] $VersionProperties = @()
    )

    $msbuildArguments = @(
        'msbuild',
        'src/TorBoxSDK/TorBoxSDK.csproj',
        '--getProperty:PackageVersion',
        '--nologo'
    ) + $VersionProperties

    $packageVersionOutput = & dotnet @msbuildArguments

    if ($LASTEXITCODE -ne 0) {
        throw "Unable to resolve PackageVersion with 'dotnet $($msbuildArguments -join ' ')'."
    }

    $packageVersionLines = @($packageVersionOutput | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    [string] $packageVersion = $packageVersionLines[$packageVersionLines.Count - 1].Trim()

    if ($packageVersion -notmatch '^2\.[0-9]+\.[0-9]+(?:-[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?$') {
        throw "PackageVersion '$packageVersion' is not a supported v2 NuGet version."
    }

    return $packageVersion
}

function Invoke-PackageValidation {
    param(
        [Parameter(Mandatory)]
        [string] $PackageDirectory,

        [string[]] $VersionProperties = @()
    )

    [System.IO.Directory]::CreateDirectory($PackageDirectory) | Out-Null

    Invoke-DotNet -Arguments (@(
        'pack',
        'src/TorBoxSDK/TorBoxSDK.csproj',
        '--configuration', 'Release',
        '--no-build',
        '--no-restore',
        '--output', $PackageDirectory
    ) + $VersionProperties)

    $packageFiles = @(Get-ChildItem -LiteralPath $PackageDirectory -File -Filter 'TorBoxSDK.*.nupkg')

    if ($packageFiles.Count -ne 1) {
        throw "Expected exactly one TorBoxSDK .nupkg in '$PackageDirectory', found $($packageFiles.Count)."
    }

    $packageName = $packageFiles[0].Name
    $prefix = 'TorBoxSDK.'
    $extension = '.nupkg'

    if ((-not $packageName.StartsWith($prefix, [System.StringComparison]::Ordinal)) -or
        (-not $packageName.EndsWith($extension, [System.StringComparison]::Ordinal)) -or
        ($packageName.Length -le ($prefix.Length + $extension.Length))) {
        throw "Package '$packageName' does not have the expected TorBoxSDK.<version>.nupkg name."
    }

    $packageVersionLength = $packageName.Length - $prefix.Length - $extension.Length
    $packageVersion = $packageName.Substring($prefix.Length, $packageVersionLength)
    $packageTag = "v$packageVersion"
    [string] $commit = (& git rev-parse HEAD).Trim()

    if ($LASTEXITCODE -ne 0) {
        throw 'Unable to resolve the current Git commit for package validation.'
    }

    if ($commit -notmatch '^[0-9a-fA-F]{40}$') {
        throw "Resolved Git commit '$commit' is not a full SHA."
    }

    $manifestPath = Join-Path $PackageDirectory 'package-manifest.json'
    $validatorPath = Join-Path $PSScriptRoot 'Test-NuGetReleaseArtifact.ps1'

    & $validatorPath `
        -Tag $packageTag `
        -Commit $commit `
        -PackageDirectory $PackageDirectory `
        -ManifestPath $manifestPath `
        -WriteManifest

    if ($LASTEXITCODE -ne 0) {
        throw "Package artifact validation failed with exit code $LASTEXITCODE."
    }
}

& (Join-Path $PSScriptRoot 'Invoke-GenerateInternalContracts.ps1') -Verify

$fullResultsDirectory = [System.IO.Path]::GetFullPath($ResultsDirectory)
[System.IO.Directory]::CreateDirectory($fullResultsDirectory) | Out-Null

$versionProperties = @()

if (-not [string]::IsNullOrWhiteSpace($Version)) {
    $versionProperties = @(
        "-p:Version=$Version",
        "-p:PackageVersion=$Version",
        "-p:FileVersion=$FileVersion"
    )
}

Invoke-DotNet -Arguments @('restore', 'TorBoxSDK.slnx', '--locked-mode')
Invoke-DotNet -Arguments (@('build', 'TorBoxSDK.slnx', '--configuration', 'Release', '--no-restore') + $versionProperties)

$testPlans = @(
    [pscustomobject]@{
        Name = 'unit-tests'
        Project = 'tests/TorboxSDK.UnitTests/TorboxSDK.UnitTests.csproj'
        Filter = 'Category!=Live&Category!=Integration'
    },
    [pscustomobject]@{
        Name = 'schema-validation'
        Project = 'tests/TorBoxSDK.SchemaValidationTests/TorBoxSDK.SchemaValidationTests.csproj'
        Filter = 'Category=Contract'
    }
)

$targetFrameworks = @('net6.0', 'net7.0', 'net8.0', 'net9.0', 'net10.0')

foreach ($testPlan in $testPlans) {
    foreach ($targetFramework in $targetFrameworks) {
        $testArguments = @(
            'test',
            $testPlan.Project,
            '--configuration', 'Release',
            '--framework', $targetFramework,
            '--no-build',
            '--no-restore',
            '--logger', "trx;LogFileName=$($testPlan.Name)-$targetFramework.trx",
            '--results-directory', $fullResultsDirectory
        )

        if ($null -ne $testPlan.Filter) {
            $testArguments += @('--filter', $testPlan.Filter)
        }

        Invoke-DotNet -Arguments $testArguments
    }
}

if ($SkipPackageValidation) {
    Write-Host 'Package validation was skipped; the caller is responsible for packing and validating the exact candidate artifact.'
}
else {
    $packageVersion = Get-PackageVersion -VersionProperties $versionProperties
    $packageDirectory = Join-Path (Join-Path $fullResultsDirectory 'package-validation') $packageVersion
    Invoke-PackageValidation -PackageDirectory $packageDirectory -VersionProperties $versionProperties
}
