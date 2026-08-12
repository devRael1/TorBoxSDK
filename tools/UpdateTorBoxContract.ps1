[CmdletBinding()]
param(
    [switch] $Refresh,
    [switch] $InitializeCoverage,
    [switch] $Validate
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$sourceUrl = 'https://api.torbox.app/openapi.json'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$contractDirectory = Join-Path $repositoryRoot 'contracts/torbox'
$baselineDirectory = Join-Path $contractDirectory 'baseline'
$snapshotPath = Join-Path $baselineDirectory 'openapi.json'
$manifestPath = Join-Path $baselineDirectory 'manifest.json'
$coveragePath = Join-Path $contractDirectory 'coverage.json'

if (-not ($Refresh -or $InitializeCoverage -or $Validate)) {
    throw 'Specify at least one of -Refresh, -InitializeCoverage, or -Validate.'
}

if ($InitializeCoverage -and (Test-Path -LiteralPath $coveragePath)) {
    throw "Coverage already exists at '$coveragePath'. Refusing to overwrite a reviewed mapping."
}

function Get-SnapshotFacts {
    if (-not (Test-Path -LiteralPath $snapshotPath)) {
        throw "The contract snapshot is missing at '$snapshotPath'. Run with -Refresh first."
    }

    [byte[]] $snapshotBytes = [System.IO.File]::ReadAllBytes($snapshotPath)
    [string] $sha256 = (Get-FileHash -LiteralPath $snapshotPath -Algorithm SHA256).Hash.ToLowerInvariant()
    [string] $snapshotJson = [System.Text.Encoding]::UTF8.GetString($snapshotBytes)
    $openApiDocument = $snapshotJson | ConvertFrom-Json -Depth 100

    if ([string]::IsNullOrWhiteSpace([string] $openApiDocument.info.version)) {
        throw 'The OpenAPI document does not contain info.version.'
    }

    return [pscustomobject]@{
        ByteLength = [int64] $snapshotBytes.LongLength
        Sha256 = $sha256
        OpenApiVersion = [string] $openApiDocument.info.version
        Document = $openApiDocument
    }
}

function Write-Manifest([object] $snapshotFacts) {
    $manifest = [ordered]@{
        sourceUrl = $sourceUrl
        retrievedAtUtc = [DateTime]::UtcNow.ToString('O')
        openApiVersion = $snapshotFacts.OpenApiVersion
        byteLength = $snapshotFacts.ByteLength
        sha256 = $snapshotFacts.Sha256
    }

    $manifest | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $manifestPath -Encoding utf8
}

function Initialize-Coverage([object] $openApiDocument) {
    if ($null -eq $openApiDocument.paths) {
        throw 'The OpenAPI document does not contain paths.'
    }

    $httpMethods = @('get', 'put', 'post', 'delete', 'options', 'head', 'patch', 'trace')
    $coverage = foreach ($pathProperty in $openApiDocument.paths.PSObject.Properties) {
        foreach ($methodProperty in $pathProperty.Value.PSObject.Properties) {
            if ($httpMethods -notcontains $methodProperty.Name.ToLowerInvariant()) {
                continue
            }

            if ($methodProperty.Value -isnot [pscustomobject]) {
                continue
            }

            [pscustomobject][ordered]@{
                operationKey = "$( $methodProperty.Name.ToUpperInvariant()) $($pathProperty.Name)"
                family = 'Unassigned'
                resource = 'Unassigned'
                publicInterface = $null
                publicMethod = $null
                parameterTypes = @()
                requestType = $null
                resultType = $null
                responseMode = 'requires-validation'
                implementationState = 'Planned'
                divergenceIds = @()
            }
        }
    }

    @($coverage | Sort-Object -Property operationKey) | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $coveragePath -Encoding utf8
}

if ($Refresh) {
    New-Item -ItemType Directory -Force -Path $baselineDirectory | Out-Null
    Invoke-WebRequest -Uri $sourceUrl -OutFile $snapshotPath
    $snapshotFacts = Get-SnapshotFacts
    Write-Manifest $snapshotFacts
}

if ($InitializeCoverage) {
    $snapshotFacts = Get-SnapshotFacts
    Initialize-Coverage $snapshotFacts.Document
}

if ($Validate) {
    if (-not (Test-Path -LiteralPath $manifestPath)) {
        throw "The contract manifest is missing at '$manifestPath'."
    }

    $snapshotFacts = Get-SnapshotFacts
    $manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json -Depth 10

    if ([string] $manifest.sha256 -ne $snapshotFacts.Sha256) {
        throw 'The contract snapshot SHA-256 does not match manifest.json.'
    }

    if ([int64] $manifest.byteLength -ne $snapshotFacts.ByteLength) {
        throw 'The contract snapshot byte length does not match manifest.json.'
    }

    if ([string] $manifest.openApiVersion -ne $snapshotFacts.OpenApiVersion) {
        throw 'The contract snapshot OpenAPI version does not match manifest.json.'
    }
}
