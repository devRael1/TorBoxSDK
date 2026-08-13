[CmdletBinding()]
param(
    [switch] $Refresh,
    [switch] $InitializeCoverage,
    [switch] $Validate,
    [ValidateSet('Main', 'Relay', 'Search')]
    [string] $Source = 'Main'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$contractDirectory = Join-Path $repositoryRoot 'contracts/torbox'
$coveragePath = Join-Path $contractDirectory 'coverage.json'
$transactionDirectory = Join-Path $contractDirectory '.update-transaction'
$httpMethods = @('get', 'put', 'post', 'delete', 'options', 'head', 'patch', 'trace')
$supportedSourceIds = @('main', 'relay', 'search')

if (-not ($Refresh -or $InitializeCoverage -or $Validate)) {
    throw 'Specify at least one of -Refresh, -InitializeCoverage, or -Validate.'
}

if ($InitializeCoverage -and $Source -ne 'Main') {
    throw '-InitializeCoverage is Main-only and cannot initialize Relay or Search coverage.'
}

if ($Refresh -and $Source -eq 'Search') {
    throw 'Search is a reviewed documentation inventory and has no automatic refresh path.'
}

function Get-SourceDefinition([string] $sourceName) {
    switch ($sourceName) {
        'Main' {
            return [pscustomobject]@{
                Name = 'Main'
                Id = 'main'
                Kind = 'openapi'
                SourceUrl = 'https://api.torbox.app/openapi.json'
                SnapshotRelativePath = 'baseline/openapi.json'
                ManifestRelativePath = 'baseline/manifest.json'
            }
        }
        'Relay' {
            return [pscustomobject]@{
                Name = 'Relay'
                Id = 'relay'
                Kind = 'openapi'
                SourceUrl = 'https://relay.torbox.app/openapi.json'
                SnapshotRelativePath = 'relay/openapi.json'
                ManifestRelativePath = 'relay/manifest.json'
            }
        }
        'Search' {
            return [pscustomobject]@{
                Name = 'Search'
                Id = 'search'
                Kind = 'documented-operations'
                SourceUrl = 'https://www.postman.com/torbox/torbox-api/documentation/u47iwao/search-api'
                SnapshotRelativePath = 'search/operations.json'
                ManifestRelativePath = 'search/manifest.json'
            }
        }
        default { throw "Unsupported contract source '$sourceName'." }
    }
}

function Get-SourceDefinitionById([string] $sourceId) {
    switch ($sourceId) {
        'main' { return Get-SourceDefinition 'Main' }
        'relay' { return Get-SourceDefinition 'Relay' }
        'search' { return Get-SourceDefinition 'Search' }
        default { throw "Unsupported contract source ID '$sourceId'." }
    }
}

function Get-DurableSourcePaths([object] $definition) {
    return [pscustomobject]@{
        SnapshotPath = Join-Path $contractDirectory $definition.SnapshotRelativePath
        ManifestPath = Join-Path $contractDirectory $definition.ManifestRelativePath
        CoveragePath = $coveragePath
    }
}

function Get-OpenApiOperationKeys([object] $openApiDocument) {
    if ($null -eq $openApiDocument.paths) {
        throw 'The OpenAPI document does not contain paths.'
    }

    foreach ($pathProperty in $openApiDocument.paths.PSObject.Properties) {
        foreach ($methodProperty in $pathProperty.Value.PSObject.Properties) {
            if ($httpMethods -contains $methodProperty.Name.ToLowerInvariant() -and $methodProperty.Value -is [pscustomobject]) {
                "$($methodProperty.Name.ToUpperInvariant()) $($pathProperty.Name)"
            }
        }
    }
}

function Get-DocumentedOperationKeys([object] $document) {
    if ([string] $document.format -cne 'torbox-sdk/documented-operations/v1' -or $null -eq $document.operations) {
        throw 'The Search documented operation inventory has an unsupported format.'
    }

    foreach ($operation in @($document.operations)) {
        [string] $method = [string] $operation.method
        [string] $path = [string] $operation.path
        if ($httpMethods -notcontains $method.ToLowerInvariant() -or [string]::IsNullOrWhiteSpace($path) -or -not $path.StartsWith('/')) {
            throw 'Every Search documented operation must contain a supported method and absolute path.'
        }

        "$($method.ToUpperInvariant()) $path"
    }
}

function Get-SnapshotFacts([object] $definition, [string] $candidateSnapshotPath) {
    if (-not (Test-Path -LiteralPath $candidateSnapshotPath)) {
        throw "The $($definition.Name) contract snapshot is missing at '$candidateSnapshotPath'."
    }

    [byte[]] $snapshotBytes = [System.IO.File]::ReadAllBytes($candidateSnapshotPath)
    [string] $sha256 = (Get-FileHash -LiteralPath $candidateSnapshotPath -Algorithm SHA256).Hash.ToLowerInvariant()
    [string] $snapshotJson = [System.Text.Encoding]::UTF8.GetString($snapshotBytes)
    $document = $snapshotJson | ConvertFrom-Json -Depth 100

    if ($definition.Kind -ceq 'openapi') {
        if ($null -eq $document.info -or [string]::IsNullOrWhiteSpace([string] $document.info.version)) {
            throw 'The OpenAPI document does not contain info.version.'
        }

        $operationKeys = @(Get-OpenApiOperationKeys $document)
        [string] $openApiVersion = [string] $document.info.version
    }
    else {
        $operationKeys = @(Get-DocumentedOperationKeys $document)
        [string] $openApiVersion = ''
    }

    $uniqueOperationKeys = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($operationKey in $operationKeys) {
        if (-not $uniqueOperationKeys.Add([string] $operationKey)) {
            throw "The $($definition.Name) contract contains duplicate operation '$operationKey'."
        }
    }

    return [pscustomobject]@{
        ByteLength = [int64] $snapshotBytes.LongLength
        Sha256 = $sha256
        OpenApiVersion = $openApiVersion
        Document = $document
        OperationKeys = $uniqueOperationKeys
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

function Test-UtcTimestamp([string] $retrievedAtText) {
    [DateTimeOffset] $retrievedAtUtc = [DateTimeOffset]::MinValue
    if (-not $retrievedAtText.EndsWith('Z', [System.StringComparison]::Ordinal) -or
        -not [DateTimeOffset]::TryParse(
            $retrievedAtText,
            [System.Globalization.CultureInfo]::InvariantCulture,
            [System.Globalization.DateTimeStyles]::RoundtripKind,
            [ref] $retrievedAtUtc) -or $retrievedAtUtc.Offset -ne [TimeSpan]::Zero) {
        throw 'The contract manifest retrieval timestamp must be a UTC timestamp.'
    }
}

function Test-Manifest([object] $definition, [object] $snapshotFacts, [object] $manifest) {
    [string] $sourceUrl = [string] (Get-RequiredManifestValue $manifest 'sourceUrl')
    if ($sourceUrl -cne $definition.SourceUrl) {
        throw "The $($definition.Name) contract manifest source URL is not the approved source URL."
    }

    Test-UtcTimestamp ([string] (Get-RequiredManifestValue $manifest 'retrievedAtUtc'))

    if ($definition.Kind -ceq 'openapi') {
        [string] $openApiVersion = [string] (Get-RequiredManifestValue $manifest 'openApiVersion')
        if ([string]::IsNullOrWhiteSpace($openApiVersion) -or $openApiVersion -cne $snapshotFacts.OpenApiVersion) {
            throw 'The contract manifest OpenAPI version is missing or does not match the snapshot.'
        }
    }
    else {
        [string] $sourceKind = [string] (Get-RequiredManifestValue $manifest 'sourceKind')
        [string] $releaseEligibility = [string] (Get-RequiredManifestValue $manifest 'releaseEligibility')
        if ($sourceKind -cne 'postman-documentation' -or $releaseEligibility -cne 'requires-live-validation') {
            throw 'The Search contract manifest does not preserve its documentation source and live-validation requirement.'
        }
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

function Test-Coverage([object] $definition, [object] $snapshotFacts, [string] $candidateCoveragePath) {
    if (-not (Test-Path -LiteralPath $candidateCoveragePath)) {
        throw "The contract coverage inventory is missing at '$candidateCoveragePath'."
    }

    [string] $coverageJson = Get-Content -LiteralPath $candidateCoveragePath -Raw
    if (-not $coverageJson.TrimStart().StartsWith('[')) {
        throw 'The contract coverage inventory must be a JSON array.'
    }

    $coverageRows = @($coverageJson | ConvertFrom-Json -Depth 100)
    $identities = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $selectedOperationKeys = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($coverageRow in $coverageRows) {
        $sourceIdProperty = $coverageRow.PSObject.Properties['sourceId']
        $operationKeyProperty = $coverageRow.PSObject.Properties['operationKey']
        if ($null -eq $sourceIdProperty -or [string]::IsNullOrWhiteSpace([string] $sourceIdProperty.Value) -or
            $supportedSourceIds -cnotcontains [string] $sourceIdProperty.Value) {
            throw 'Every contract coverage record must have an approved non-empty sourceId.'
        }

        if ($null -eq $operationKeyProperty -or [string]::IsNullOrWhiteSpace([string] $operationKeyProperty.Value)) {
            throw 'Every contract coverage record must have a non-empty operationKey.'
        }

        [string] $sourceId = [string] $sourceIdProperty.Value
        [string] $operationKey = [string] $operationKeyProperty.Value
        [string] $identity = "${sourceId}:$operationKey"
        if (-not $identities.Add($identity)) {
            throw "The contract coverage inventory contains duplicate identity '$identity'."
        }

        if ($sourceId -ceq $definition.Id) {
            [void] $selectedOperationKeys.Add($operationKey)
        }
    }

    if (-not $selectedOperationKeys.SetEquals($snapshotFacts.OperationKeys)) {
        throw "The $($definition.Name) coverage rows must contain exactly the selected source METHOD path operations."
    }
}

function Test-Baseline([object] $definition, [string] $candidateSnapshotPath, [string] $candidateManifestPath, [string] $candidateCoveragePath) {
    $snapshotFacts = Get-SnapshotFacts $definition $candidateSnapshotPath
    $manifest = Get-Manifest $candidateManifestPath
    Test-Manifest $definition $snapshotFacts $manifest
    Test-Coverage $definition $snapshotFacts $candidateCoveragePath
    return $snapshotFacts
}

function Write-Manifest([object] $definition, [object] $snapshotFacts, [string] $candidateManifestPath) {
    $manifest = [ordered]@{
        sourceUrl = $definition.SourceUrl
        retrievedAtUtc = [DateTime]::UtcNow.ToString('O')
        openApiVersion = $snapshotFacts.OpenApiVersion
        byteLength = $snapshotFacts.ByteLength
        sha256 = $snapshotFacts.Sha256
    }

    $manifest | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $candidateManifestPath -Encoding utf8
}

function Write-Coverage([object] $snapshotFacts, [string] $candidateCoveragePath) {
    $coverage = foreach ($operationKey in $snapshotFacts.OperationKeys) {
        [pscustomobject][ordered]@{
            sourceId = 'main'
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

    [string] $sourceId = if ($null -eq $journal.PSObject.Properties['sourceId']) { 'main' } else { [string] $journal.sourceId }
    $definition = Get-SourceDefinitionById $sourceId
    $journal | Add-Member -NotePropertyName Definition -NotePropertyValue $definition -Force
    return $journal
}

function Get-CoverageExpectation() {
    $coverageExists = Test-Path -LiteralPath $coveragePath
    return [pscustomobject]@{
        Exists = $coverageExists
        Sha256 = if ($coverageExists) { (Get-FileHash -LiteralPath $coveragePath -Algorithm SHA256).Hash } else { $null }
    }
}

function Test-CoverageExpectation([object] $transaction) {
    if (-not [bool] $transaction.publishCoverage) {
        return
    }

    if ($null -eq $transaction.PSObject.Properties['expectedCoverageExists'] -or
        $null -eq $transaction.PSObject.Properties['expectedCoverageSha256']) {
        throw 'The contract transaction journal does not preserve the expected coverage state.'
    }

    $expectedCoverageExists = [bool] $transaction.expectedCoverageExists
    $coverageExists = Test-Path -LiteralPath $coveragePath
    if ($coverageExists -ne $expectedCoverageExists) {
        throw 'Coverage changed since the transaction began. Refusing to overwrite a reviewed mapping.'
    }

    if ($expectedCoverageExists) {
        [string] $expectedSha256 = [string] $transaction.expectedCoverageSha256
        if ([string]::IsNullOrWhiteSpace($expectedSha256) -or
            (Get-FileHash -LiteralPath $coveragePath -Algorithm SHA256).Hash -cne $expectedSha256) {
            throw 'Coverage changed since the transaction began. Refusing to overwrite a reviewed mapping.'
        }
    }
}

function Get-ContractPaths([object] $definition, [object] $transaction) {
    $durablePaths = Get-DurableSourcePaths $definition
    if ($null -eq $transaction) {
        return $durablePaths
    }

    $candidateDirectory = Join-Path $transactionDirectory 'candidate'
    [string] $effectiveSnapshotPath = $durablePaths.SnapshotPath
    [string] $effectiveManifestPath = $durablePaths.ManifestPath
    if ($definition.Id -ceq $transaction.Definition.Id) {
        $effectiveSnapshotPath = Join-Path $candidateDirectory $definition.SnapshotRelativePath
        $effectiveManifestPath = Join-Path $candidateDirectory $definition.ManifestRelativePath
    }

    $candidateCoveragePath = Join-Path $candidateDirectory 'coverage.json'
    if (-not (Test-Path -LiteralPath $effectiveSnapshotPath) -or
        -not (Test-Path -LiteralPath $effectiveManifestPath) -or
        -not (Test-Path -LiteralPath $candidateCoveragePath)) {
        throw 'The contract transaction candidate is incomplete and cannot be recovered safely.'
    }

    return [pscustomobject]@{
        SnapshotPath = $effectiveSnapshotPath
        ManifestPath = $effectiveManifestPath
        CoveragePath = $candidateCoveragePath
    }
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

function Remove-RetiredContractTransaction() {
    $journalPath = Join-Path $transactionDirectory 'journal.json'
    if (Test-Path -LiteralPath $journalPath) {
        throw 'The active contract transaction journal must be retired before cleanup.'
    }

    if (Test-Path -LiteralPath $transactionDirectory) {
        Remove-Item -LiteralPath $transactionDirectory -Recurse -Force
    }
}

function Retire-ContractTransactionJournal() {
    $journalPath = Join-Path $transactionDirectory 'journal.json'
    if (-not (Test-Path -LiteralPath $journalPath)) {
        throw 'The active contract transaction journal is missing before retirement.'
    }

    $retiredJournalPath = Join-Path $transactionDirectory ("journal.retired.$([Guid]::NewGuid().ToString('N')).json")
    Move-Item -LiteralPath $journalPath -Destination $retiredJournalPath
}

function Complete-ContractTransaction() {
    $transaction = Get-ContractTransaction
    if ($null -eq $transaction) {
        Remove-RetiredContractTransaction
        return
    }

    $definition = $transaction.Definition
    $candidatePaths = Get-ContractPaths $definition $transaction
    [void] (Test-Baseline $definition $candidatePaths.SnapshotPath $candidatePaths.ManifestPath $candidatePaths.CoveragePath)
    Test-CoverageExpectation $transaction

    $durablePaths = Get-DurableSourcePaths $definition
    Publish-File $candidatePaths.SnapshotPath $durablePaths.SnapshotPath
    Publish-File $candidatePaths.ManifestPath $durablePaths.ManifestPath
    if ([bool] $transaction.publishCoverage) {
        Test-CoverageExpectation $transaction
        Publish-File $candidatePaths.CoveragePath $coveragePath
    }

    [void] (Test-Baseline $definition $durablePaths.SnapshotPath $durablePaths.ManifestPath $coveragePath)
    Retire-ContractTransactionJournal
    Remove-RetiredContractTransaction
}

function Publish-ContractTransaction(
    [object] $definition,
    [string] $candidateSnapshotPath,
    [string] $candidateManifestPath,
    [string] $candidateCoveragePath,
    [bool] $publishCoverage,
    [object] $coverageExpectation) {
    if (Test-Path -LiteralPath $transactionDirectory) {
        throw "The contract transaction directory '$transactionDirectory' must be completed before publishing another candidate."
    }

    $stagingDirectory = "$transactionDirectory.$([Guid]::NewGuid().ToString('N')).staging"
    $candidateDirectory = Join-Path $stagingDirectory 'candidate'
    try {
        $stagedSnapshotPath = Join-Path $candidateDirectory $definition.SnapshotRelativePath
        $stagedManifestPath = Join-Path $candidateDirectory $definition.ManifestRelativePath
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $stagedSnapshotPath) | Out-Null
        Copy-Item -LiteralPath $candidateSnapshotPath -Destination $stagedSnapshotPath
        Copy-Item -LiteralPath $candidateManifestPath -Destination $stagedManifestPath
        Copy-Item -LiteralPath $candidateCoveragePath -Destination (Join-Path $candidateDirectory 'coverage.json')
        [void] (Test-Baseline $definition $stagedSnapshotPath $stagedManifestPath (Join-Path $candidateDirectory 'coverage.json'))

        $journal = [ordered]@{
            schemaVersion = 3
            activeGeneration = 'candidate'
            sourceId = $definition.Id
            publishCoverage = $publishCoverage
            expectedCoverageExists = [bool] $coverageExpectation.Exists
            expectedCoverageSha256 = $coverageExpectation.Sha256
        }
        $journal | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $stagingDirectory 'journal.json') -Encoding utf8
        Move-Item -LiteralPath $stagingDirectory -Destination $transactionDirectory
    }
    finally {
        Remove-Item -LiteralPath $stagingDirectory -Recurse -Force -ErrorAction SilentlyContinue
    }

    Complete-ContractTransaction
}

$definition = Get-SourceDefinition $Source
$durablePaths = Get-DurableSourcePaths $definition
$temporarySuffix = [Guid]::NewGuid().ToString('N')
$temporarySnapshotPath = "$($durablePaths.SnapshotPath).$temporarySuffix.tmp"
$temporaryManifestPath = "$($durablePaths.ManifestPath).$temporarySuffix.tmp"
$temporaryCoveragePath = "$coveragePath.$temporarySuffix.tmp"

try {
    if ($Refresh -or $InitializeCoverage) {
        Complete-ContractTransaction
    }

    $coverageExpectation = Get-CoverageExpectation
    if ($InitializeCoverage -and $coverageExpectation.Exists) {
        throw "Coverage already exists at '$coveragePath'. Refusing to overwrite a reviewed mapping."
    }

    if ($Refresh) {
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $durablePaths.SnapshotPath) | Out-Null
        Invoke-WebRequest -Uri $definition.SourceUrl -OutFile $temporarySnapshotPath
        $snapshotFacts = Get-SnapshotFacts $definition $temporarySnapshotPath
        Write-Manifest $definition $snapshotFacts $temporaryManifestPath
    }

    if ($InitializeCoverage) {
        $snapshotFacts = if ($Refresh) {
            Get-SnapshotFacts $definition $temporarySnapshotPath
        }
        else {
            Get-SnapshotFacts $definition $durablePaths.SnapshotPath
        }
        Write-Coverage $snapshotFacts $temporaryCoveragePath
    }

    if ($Refresh -or $InitializeCoverage) {
        $candidateSnapshotPath = if ($Refresh) { $temporarySnapshotPath } else { $durablePaths.SnapshotPath }
        $candidateManifestPath = if ($Refresh) { $temporaryManifestPath } else { $durablePaths.ManifestPath }
        $candidateCoveragePath = if ($InitializeCoverage) { $temporaryCoveragePath } else { $coveragePath }
        [void] (Test-Baseline $definition $candidateSnapshotPath $candidateManifestPath $candidateCoveragePath)
        Publish-ContractTransaction $definition $candidateSnapshotPath $candidateManifestPath $candidateCoveragePath $InitializeCoverage.IsPresent $coverageExpectation
    }

    if ($Validate) {
        $transaction = Get-ContractTransaction
        $effectivePaths = Get-ContractPaths $definition $transaction
        [void] (Test-Baseline $definition $effectivePaths.SnapshotPath $effectivePaths.ManifestPath $effectivePaths.CoveragePath)
    }
}
finally {
    Remove-Item -LiteralPath $temporarySnapshotPath, $temporaryManifestPath, $temporaryCoveragePath -Force -ErrorAction SilentlyContinue
}
