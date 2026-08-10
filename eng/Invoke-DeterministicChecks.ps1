[CmdletBinding()]
param(
    [string] $Version,

    [string] $FileVersion,

    [string] $ResultsDirectory = 'artifacts/test-results'
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
        Filter = $null
    },
    [pscustomobject]@{
        Name = 'schema-validation'
        Project = 'tests/TorBoxSDK.SchemaValidationTests/TorBoxSDK.SchemaValidationTests.csproj'
        Filter = 'Category!=Live'
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
