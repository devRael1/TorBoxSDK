[CmdletBinding()]
param(
    [switch] $Refresh,
    [switch] $InitializeCoverage,
    [switch] $Validate
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$officialSourceUrl = 'https://api.torbox.app/openapi.json'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$contractDirectory = Join-Path $repositoryRoot 'contracts/torbox'
$baselineDirectory = Join-Path $contractDirectory 'baseline'
$snapshotPath = Join-Path $baselineDirectory 'openapi.json'
$manifestPath = Join-Path $baselineDirectory 'manifest.json'
$coveragePath = Join-Path $contractDirectory 'coverage.json'
$transactionDirectory = Join-Path $contractDirectory '.update-transaction'
$httpMethods = @('get', 'put', 'post', 'delete', 'options', 'head', 'patch', 'trace')

if (-not ($Refresh -or $InitializeCoverage -or $Validate)) {
    throw 'Specify at least one of -Refresh, -InitializeCoverage, or -Validate.'
}

function Get-SnapshotFacts([string] $candidateSnapshotPath) {
    if (-not (Test-Path -LiteralPath $candidateSnapshotPath)) {
        throw "The contract snapshot is missing at '$candidateSnapshotPath'."
    }

    [byte[]] $snapshotBytes = [System.IO.File]::ReadAllBytes($candidateSnapshotPath)
    [string] $sha256 = (Get-FileHash -LiteralPath $candidateSnapshotPath -Algorithm SHA256).Hash.ToLowerInvariant()
    [string] $snapshotJson = [System.Text.Encoding]::UTF8.GetString($snapshotBytes)
    $openApiDocument = $snapshotJson | ConvertFrom-Json -Depth 100

    if ($null -eq $openApiDocument.info -or [string]::IsNullOrWhiteSpace([string] $openApiDocument.info.version)) {
        throw 'The OpenAPI document does not contain info.version.'
    }

    return [pscustomobject]@{
        ByteLength = [int64] $snapshotBytes.LongLength
        Sha256 = $sha256
        OpenApiVersion = [string] $openApiDocument.info.version
        Document = $openApiDocument
    }
}

function Get-Manifest([string] $candidateManifestPath) {
    if (-not (Test-Path -LiteralPath $candidateManifestPath)) {
        throw "The contract manifest is missing at '$candidateManifestPath'."
    }

    [string] $manifestJson = Get-Content -LiteralPath $candidateManifestPath -Raw
    $manifest = $manifestJson | ConvertFrom-Json -Depth 10
    $retrievedAtUtcMatch = [regex]::Match($manifestJson, '"retrievedAtUtc"\s*:\s*"(?<value>[^"]+)"')
    $manifest | Add-Member -NotePropertyName retrievedAtUtc -NotePropertyValue $(if ($retrievedAtUtcMatch.Success) { $retrievedAtUtcMatch.Groups['value'].Value } else { $null }) -Force
    return $manifest
}

function Get-RequiredManifestValue([object] $manifest, [string] $propertyName) {
    $property = $manifest.PSObject.Properties[$propertyName]
    if ($null -eq $property -or $null -eq $property.Value) {
        throw "The contract manifest property '$propertyName' is required."
    }

    return $property.Value
}

function Test-Manifest([object] $snapshotFacts, [object] $manifest) {
    [string] $sourceUrl = [string] (Get-RequiredManifestValue $manifest 'sourceUrl')
    if ($sourceUrl -cne $officialSourceUrl) {
        throw 'The contract manifest source URL is not the official TorBox OpenAPI URL.'
    }

    [string] $retrievedAtText = [string] (Get-RequiredManifestValue $manifest 'retrievedAtUtc')
    [DateTimeOffset] $retrievedAtUtc = [DateTimeOffset]::MinValue
    if (-not $retrievedAtText.EndsWith('Z', [System.StringComparison]::Ordinal) -or
        -not [DateTimeOffset]::TryParse(
            $retrievedAtText,
            [System.Globalization.CultureInfo]::InvariantCulture,
            [System.Globalization.DateTimeStyles]::RoundtripKind,
            [ref] $retrievedAtUtc) -or $retrievedAtUtc.Offset -ne [TimeSpan]::Zero) {
        throw 'The contract manifest retrieval timestamp must be a UTC timestamp.'
    }

    [string] $openApiVersion = [string] (Get-RequiredManifestValue $manifest 'openApiVersion')
    if ([string]::IsNullOrWhiteSpace($openApiVersion) -or $openApiVersion -cne $snapshotFacts.OpenApiVersion) {
        throw 'The contract manifest OpenAPI version is missing or does not match the snapshot.'
    }

    [int64] $byteLength = [int64] (Get-RequiredManifestValue $manifest 'byteLength')
    if ($byteLength -ne $snapshotFacts.ByteLength) {
        throw 'The contract manifest byte length does not match the snapshot.'
    }

    [string] $sha256 = [string] (Get-RequiredManifestValue $manifest 'sha256')
    if ([string]::IsNullOrWhiteSpace($sha256) -or $sha256 -cne $snapshotFacts.Sha256) {
        throw 'The contract manifest SHA-256 does not match the snapshot.'
    }
}

function Get-OperationKeys([object] $openApiDocument) {
    if ($null -eq $openApiDocument.paths) {
        throw 'The OpenAPI document does not contain paths.'
    }

    foreach ($pathProperty in $openApiDocument.paths.PSObject.Properties) {
        foreach ($methodProperty in $pathProperty.Value.PSObject.Properties) {
            if ($httpMethods -contains $methodProperty.Name.ToLowerInvariant() -and $methodProperty.Value -is [pscustomobject]) {
                "$( $methodProperty.Name.ToUpperInvariant()) $($pathProperty.Name)"
            }
        }
    }
}

function Test-Coverage([object] $openApiDocument, [string] $candidateCoveragePath) {
    if (-not (Test-Path -LiteralPath $candidateCoveragePath)) {
        throw "The contract coverage inventory is missing at '$candidateCoveragePath'."
    }

    [string] $coverageJson = Get-Content -LiteralPath $candidateCoveragePath -Raw
    if (-not $coverageJson.TrimStart().StartsWith('[')) {
        throw 'The contract coverage inventory must be a JSON array.'
    }

    $coverageRows = @($coverageJson | ConvertFrom-Json -Depth 100)
    $coverageOperationKeys = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($coverageRow in $coverageRows) {
        $operationKeyProperty = $coverageRow.PSObject.Properties['operationKey']
        if ($null -eq $operationKeyProperty -or [string]::IsNullOrWhiteSpace([string] $operationKeyProperty.Value)) {
            throw 'Every contract coverage record must have a non-empty operationKey.'
        }

        [string] $operationKey = [string] $operationKeyProperty.Value
        if (-not $coverageOperationKeys.Add($operationKey)) {
            throw "The contract coverage inventory contains duplicate operationKey '$operationKey'."
        }
    }

    $snapshotOperationKeys = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($operationKey in @(Get-OperationKeys $openApiDocument)) {
        [void] $snapshotOperationKeys.Add($operationKey)
    }

    if (-not $coverageOperationKeys.SetEquals($snapshotOperationKeys)) {
        throw 'The contract coverage inventory must contain exactly the snapshot METHOD path operation keys.'
    }
}

function Test-Baseline([string] $candidateSnapshotPath, [string] $candidateManifestPath, [string] $candidateCoveragePath) {
    $snapshotFacts = Get-SnapshotFacts $candidateSnapshotPath
    $manifest = Get-Manifest $candidateManifestPath
    Test-Manifest $snapshotFacts $manifest
    Test-Coverage $snapshotFacts.Document $candidateCoveragePath
    return $snapshotFacts
}

function Write-Manifest([object] $snapshotFacts, [string] $candidateManifestPath) {
    $manifest = [ordered]@{
        sourceUrl = $officialSourceUrl
        retrievedAtUtc = [DateTime]::UtcNow.ToString('O')
        openApiVersion = $snapshotFacts.OpenApiVersion
        byteLength = $snapshotFacts.ByteLength
        sha256 = $snapshotFacts.Sha256
    }

    $manifest | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $candidateManifestPath -Encoding utf8
}

function Write-Coverage([object] $openApiDocument, [string] $candidateCoveragePath) {
    $coverage = foreach ($operationKey in @(Get-OperationKeys $openApiDocument)) {
        [pscustomobject][ordered]@{
            operationKey = $operationKey
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

    @($coverage | Sort-Object -Property operationKey) | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $candidateCoveragePath -Encoding utf8
}

function Get-ContractTransaction() {
    $journalPath = Join-Path $transactionDirectory 'journal.json'
    if (-not (Test-Path -LiteralPath $journalPath)) {
        return $null
    }

    $journal = Get-Content -LiteralPath $journalPath -Raw | ConvertFrom-Json -Depth 10
    if ($null -eq $journal -or [string] $journal.activeGeneration -cne 'candidate') {
        throw 'The contract transaction journal does not identify a supported complete generation.'
    }

    if ($null -eq $journal.PSObject.Properties['publishCoverage']) {
        throw 'The contract transaction journal does not state whether coverage must be published.'
    }

    return $journal
}

function Get-ContractPaths([object] $transaction) {
    if ($null -eq $transaction) {
        return [pscustomobject]@{
            SnapshotPath = $snapshotPath
            ManifestPath = $manifestPath
            CoveragePath = $coveragePath
        }
    }

    $candidateDirectory = Join-Path $transactionDirectory 'candidate'
    $candidatePaths = [pscustomobject]@{
        SnapshotPath = Join-Path $candidateDirectory 'baseline/openapi.json'
        ManifestPath = Join-Path $candidateDirectory 'baseline/manifest.json'
        CoveragePath = Join-Path $candidateDirectory 'coverage.json'
    }

    if (-not (Test-Path -LiteralPath $candidatePaths.SnapshotPath) -or
        -not (Test-Path -LiteralPath $candidatePaths.ManifestPath) -or
        -not (Test-Path -LiteralPath $candidatePaths.CoveragePath)) {
        throw 'The contract transaction candidate is incomplete and cannot be recovered safely.'
    }

    return $candidatePaths
}

function Publish-File([string] $sourcePath, [string] $destinationPath) {
    $destinationDirectory = Split-Path -Parent $destinationPath
    New-Item -ItemType Directory -Force -Path $destinationDirectory | Out-Null

    $temporaryDestinationPath = "$destinationPath.$([Guid]::NewGuid().ToString('N')).tmp"
    $backupDestinationPath = "$destinationPath.$([Guid]::NewGuid().ToString('N')).backup"
    try {
        Copy-Item -LiteralPath $sourcePath -Destination $temporaryDestinationPath
        if (Test-Path -LiteralPath $destinationPath) {
            [System.IO.File]::Replace($temporaryDestinationPath, $destinationPath, $backupDestinationPath)
        }
        else {
            Move-Item -LiteralPath $temporaryDestinationPath -Destination $destinationPath
        }
    }
    finally {
        Remove-Item -LiteralPath $temporaryDestinationPath, $backupDestinationPath -Force -ErrorAction SilentlyContinue
    }
}

function Complete-ContractTransaction() {
    $transaction = Get-ContractTransaction
    if ($null -eq $transaction) {
        return
    }

    $candidatePaths = Get-ContractPaths $transaction
    [void] (Test-Baseline $candidatePaths.SnapshotPath $candidatePaths.ManifestPath $candidatePaths.CoveragePath)

    Publish-File $candidatePaths.SnapshotPath $snapshotPath
    Publish-File $candidatePaths.ManifestPath $manifestPath
    if ([bool] $transaction.publishCoverage) {
        Publish-File $candidatePaths.CoveragePath $coveragePath
    }

    [void] (Test-Baseline $snapshotPath $manifestPath $coveragePath)
    Remove-Item -LiteralPath $transactionDirectory -Recurse -Force
}

function Publish-ContractTransaction(
    [string] $candidateSnapshotPath,
    [string] $candidateManifestPath,
    [string] $candidateCoveragePath,
    [bool] $publishCoverage) {
    if (Test-Path -LiteralPath $transactionDirectory) {
        throw "The contract transaction directory '$transactionDirectory' must be completed before publishing another candidate."
    }

    $stagingDirectory = "$transactionDirectory.$([Guid]::NewGuid().ToString('N')).staging"
    $candidateDirectory = Join-Path $stagingDirectory 'candidate'
    try {
        New-Item -ItemType Directory -Force -Path (Join-Path $candidateDirectory 'baseline') | Out-Null
        Copy-Item -LiteralPath $candidateSnapshotPath -Destination (Join-Path $candidateDirectory 'baseline/openapi.json')
        Copy-Item -LiteralPath $candidateManifestPath -Destination (Join-Path $candidateDirectory 'baseline/manifest.json')
        Copy-Item -LiteralPath $candidateCoveragePath -Destination (Join-Path $candidateDirectory 'coverage.json')

        $candidatePaths = [pscustomobject]@{
            SnapshotPath = Join-Path $candidateDirectory 'baseline/openapi.json'
            ManifestPath = Join-Path $candidateDirectory 'baseline/manifest.json'
            CoveragePath = Join-Path $candidateDirectory 'coverage.json'
        }
        [void] (Test-Baseline $candidatePaths.SnapshotPath $candidatePaths.ManifestPath $candidatePaths.CoveragePath)

        $journal = [ordered]@{
            activeGeneration = 'candidate'
            publishCoverage = $publishCoverage
        }
        $journal | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $stagingDirectory 'journal.json') -Encoding utf8
        Move-Item -LiteralPath $stagingDirectory -Destination $transactionDirectory
    }
    finally {
        Remove-Item -LiteralPath $stagingDirectory -Recurse -Force -ErrorAction SilentlyContinue
    }

    Complete-ContractTransaction
}

$temporarySuffix = [Guid]::NewGuid().ToString('N')
$temporarySnapshotPath = "$snapshotPath.$temporarySuffix.tmp"
$temporaryManifestPath = "$manifestPath.$temporarySuffix.tmp"
$temporaryCoveragePath = "$coveragePath.$temporarySuffix.tmp"

try {
    if ($Refresh -or $InitializeCoverage) {
        Complete-ContractTransaction
    }

    if ($InitializeCoverage -and (Test-Path -LiteralPath $coveragePath)) {
        throw "Coverage already exists at '$coveragePath'. Refusing to overwrite a reviewed mapping."
    }

    if ($Refresh) {
        New-Item -ItemType Directory -Force -Path $baselineDirectory | Out-Null
        Invoke-WebRequest -Uri $officialSourceUrl -OutFile $temporarySnapshotPath
        $snapshotFacts = Get-SnapshotFacts $temporarySnapshotPath
        Write-Manifest $snapshotFacts $temporaryManifestPath
    }

    if ($InitializeCoverage) {
        if ($Refresh) {
            $snapshotFacts = Get-SnapshotFacts $temporarySnapshotPath
        }
        else {
            $snapshotFacts = Get-SnapshotFacts $snapshotPath
        }

        Write-Coverage $snapshotFacts.Document $temporaryCoveragePath
    }

    if ($Refresh -or $InitializeCoverage) {
        $candidateSnapshotPath = if ($Refresh) { $temporarySnapshotPath } else { $snapshotPath }
        $candidateManifestPath = if ($Refresh) { $temporaryManifestPath } else { $manifestPath }
        $candidateCoveragePath = if ($InitializeCoverage) { $temporaryCoveragePath } else { $coveragePath }
        [void] (Test-Baseline $candidateSnapshotPath $candidateManifestPath $candidateCoveragePath)
    }

    if ($Refresh -or $InitializeCoverage) {
        Publish-ContractTransaction $candidateSnapshotPath $candidateManifestPath $candidateCoveragePath $InitializeCoverage.IsPresent
    }

    if ($Validate) {
        $transaction = Get-ContractTransaction
        $effectivePaths = Get-ContractPaths $transaction
        [void] (Test-Baseline $effectivePaths.SnapshotPath $effectivePaths.ManifestPath $effectivePaths.CoveragePath)
    }
}
finally {
    Remove-Item -LiteralPath $temporarySnapshotPath, $temporaryManifestPath, $temporaryCoveragePath -Force -ErrorAction SilentlyContinue
}
