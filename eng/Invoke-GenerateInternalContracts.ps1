[CmdletBinding()]
param(
    [switch] $Verify
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$settingsPath = Join-Path $repositoryRoot 'contracts/generation/kiota.settings.json'
$toolManifestPath = Join-Path $repositoryRoot '.config/dotnet-tools.json'
$baselineDirectory = Join-Path $repositoryRoot 'contracts/baseline'
$baselineManifestPath = Join-Path $baselineDirectory 'manifest.json'
$intermediateDirectory = Join-Path $repositoryRoot 'artifacts/contract-generation'

function Assert-Condition {
    param(
        [Parameter(Mandatory)]
        [bool] $Condition,

        [Parameter(Mandatory)]
        [string] $Message
    )

    if (-not $Condition) {
        throw $Message
    }
}

function Get-JsonHashtable {
    param(
        [Parameter(Mandatory)]
        [string] $Path
    )

    [string] $content = Get-Content -LiteralPath $Path -Raw
    return $content | ConvertFrom-Json -AsHashtable -Depth 100
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

function Get-BaselineSource {
    param(
        [Parameter(Mandatory)]
        [System.Collections.IDictionary] $Manifest,

        [Parameter(Mandatory)]
        [string] $SourceId
    )

    $matchingSources = @($Manifest.sources | Where-Object { $_.id -eq $SourceId })
    Assert-Condition -Condition ($matchingSources.Count -eq 1) -Message "Expected exactly one baseline source named '$SourceId'."

    return $matchingSources[0]
}

function Get-VerifiedBaselineSourcePath {
    param(
        [Parameter(Mandatory)]
        [System.Collections.IDictionary] $Source
    )

    Assert-Condition -Condition ($Source.availability -eq 'captured') -Message "Baseline source '$($Source.id)' must be captured."
    Assert-Condition -Condition ($Source.format -eq 'openapi') -Message "Baseline source '$($Source.id)' must be an OpenAPI artifact."

    [string] $artifactPath = Join-Path $baselineDirectory ([string] $Source.artifactPath)
    Assert-Condition -Condition (Test-Path -LiteralPath $artifactPath -PathType Leaf) -Message "Baseline artifact '$artifactPath' was not found."

    [System.IO.FileInfo] $artifact = Get-Item -LiteralPath $artifactPath
    Assert-Condition -Condition ($artifact.Length -eq [long] $Source.contentLength) -Message "Baseline artifact '$($Source.id)' has an unexpected length."

    [string] $actualHash = (Get-FileHash -LiteralPath $artifactPath -Algorithm SHA256).Hash.ToLowerInvariant()
    [string] $expectedHash = ([string] $Source.sha256).ToLowerInvariant()
    Assert-Condition -Condition ($actualHash -eq $expectedHash) -Message "Baseline artifact '$($Source.id)' does not match its manifest SHA-256."

    return [System.IO.Path]::GetFullPath($artifactPath)
}

function Normalize-OpenApiForKiota {
    param(
        [Parameter(Mandatory)]
        [System.Collections.IDictionary] $Document,

        [AllowEmptyCollection()]
        [object[]] $ResponsePropertyRemovals = @()
    )

    [int] $removalCount = 0

    foreach ($pathItem in $Document.paths.Values) {
        foreach ($httpMethod in @('delete', 'get', 'head', 'options', 'patch', 'post', 'put', 'trace')) {
            if (-not $pathItem.Contains($httpMethod)) {
                continue
            }

            $operation = $pathItem[$httpMethod]

            if (-not $operation.Contains('responses')) {
                continue
            }

            $responses = $operation.responses

            foreach ($removal in $ResponsePropertyRemovals) {
                [string] $statusCode = [string] $removal.statusCode

                if (-not $responses.Contains($statusCode)) {
                    continue
                }

                $response = $responses[$statusCode]

                foreach ($propertyName in $removal.properties) {
                    [string] $property = [string] $propertyName

                    if ($response.Contains($property)) {
                        $null = $response.Remove($property)
                        $removalCount++
                    }
                }
            }
        }
    }

    return $removalCount
}

function Invoke-KiotaGeneration {
    param(
        [Parameter(Mandatory)]
        [System.Collections.IDictionary] $Settings,

        [Parameter(Mandatory)]
        [System.Collections.IDictionary] $Contract,

        [Parameter(Mandatory)]
        [string] $OpenApiPath
    )

    [string] $outputDirectory = Join-Path $repositoryRoot ([string] $Contract.outputDirectory)
    $arguments = @(
        'tool', 'run', [string] $Settings.kiota.command, 'generate',
        '--openapi', $OpenApiPath,
        '--language', 'CSharp',
        '--output', $outputDirectory,
        '--class-name', [string] $Contract.clientClassName,
        '--namespace-name', [string] $Contract.namespaceName,
        '--type-access-modifier', [string] $Settings.kiota.typeAccessModifier,
        '--exclude-backward-compatible',
        '--additional-data',
        '--clean-output',
        '--log-level', 'Warning'
    )

    foreach ($serializer in $Settings.kiota.serializers) {
        $arguments += @('--serializer', [string] $serializer)
    }

    foreach ($deserializer in $Settings.kiota.deserializers) {
        $arguments += @('--deserializer', [string] $deserializer)
    }

    foreach ($structuredMimeType in $Settings.kiota.structuredMimeTypes) {
        $arguments += @('--structured-mime-types', [string] $structuredMimeType)
    }

    $commandOutput = @(& dotnet @arguments 2>&1 | ForEach-Object { $_.ToString() })
    [int] $exitCode = $LASTEXITCODE

    foreach ($line in $commandOutput) {
        Write-Host $line
    }

    if ($exitCode -ne 0) {
        throw "Kiota generation for '$($Contract.baselineSourceId)' failed with exit code $exitCode."
    }

    $openApiErrors = @($commandOutput | Where-Object { $_ -match 'OpenAPI error' })

    if ($openApiErrors.Count -gt 0) {
        throw "Kiota reported $($openApiErrors.Count) OpenAPI error(s) for '$($Contract.baselineSourceId)'."
    }

    [string] $lockPath = Join-Path $outputDirectory 'kiota-lock.json'
    Assert-Condition -Condition (Test-Path -LiteralPath $lockPath -PathType Leaf) -Message "Kiota did not create '$lockPath'."

    $lock = Get-JsonHashtable -Path $lockPath
    Assert-Condition -Condition ($lock.kiotaVersion -eq $Settings.kiota.version) -Message "Kiota lock version does not match '$($Settings.kiota.version)'."
    Assert-Condition -Condition ($lock.typeAccessModifier -eq $Settings.kiota.typeAccessModifier) -Message 'Kiota output must use the Internal type access modifier.'
    Assert-Condition -Condition ($lock.includeAdditionalData -eq $Settings.kiota.includeAdditionalData) -Message 'Kiota output AdditionalData configuration is unexpected.'
}

function Remove-GeneratedSourceTrailingWhitespace {
    param(
        [Parameter(Mandatory)]
        [string] $OutputDirectory
    )

    [int] $normalizedFileCount = 0
    $utf8WithoutBom = [System.Text.UTF8Encoding]::new($false, $true)

    foreach ($sourceFile in Get-ChildItem -LiteralPath $OutputDirectory -Filter '*.cs' -File -Recurse) {
        [string] $source = [System.IO.File]::ReadAllText($sourceFile.FullName, $utf8WithoutBom)
        [string] $normalizedSource = [System.Text.RegularExpressions.Regex]::Replace(
            $source,
            '[ \t]+(?=\r?$)',
            '',
            [System.Text.RegularExpressions.RegexOptions]::Multiline)

        if ($source -eq $normalizedSource) {
            continue
        }

        [System.IO.File]::WriteAllText($sourceFile.FullName, $normalizedSource, $utf8WithoutBom)
        $normalizedFileCount++
    }

    if ($normalizedFileCount -gt 0) {
        Write-Host "Removed trailing whitespace from $normalizedFileCount generated source file(s)."
    }
}

function Remove-GeneratedDiagnosticLog {
    param(
        [Parameter(Mandatory)]
        [string] $OutputDirectory
    )

    [string] $diagnosticLogPath = Join-Path $OutputDirectory '.kiota.log'

    if (Test-Path -LiteralPath $diagnosticLogPath -PathType Leaf) {
        Remove-Item -LiteralPath $diagnosticLogPath -Force
    }
}

Assert-Condition -Condition (Test-Path -LiteralPath $settingsPath -PathType Leaf) -Message "Generation settings '$settingsPath' were not found."
Assert-Condition -Condition (Test-Path -LiteralPath $toolManifestPath -PathType Leaf) -Message "Tool manifest '$toolManifestPath' was not found."
Assert-Condition -Condition (Test-Path -LiteralPath $baselineManifestPath -PathType Leaf) -Message "Baseline manifest '$baselineManifestPath' was not found."

$settings = Get-JsonHashtable -Path $settingsPath
$toolManifest = Get-JsonHashtable -Path $toolManifestPath
$baselineManifest = Get-JsonHashtable -Path $baselineManifestPath
$kiotaTool = $toolManifest.tools[$settings.kiota.packageId]

Assert-Condition -Condition ($null -ne $kiotaTool) -Message "Tool manifest does not contain '$($settings.kiota.packageId)'."
Assert-Condition -Condition ($settings.schemaVersion -eq 1) -Message 'Unsupported internal contract generation settings schema version.'
Assert-Condition -Condition ($kiotaTool.version -eq $settings.kiota.version) -Message 'Kiota tool manifest and generation settings must use the same version.'
Assert-Condition -Condition ($settings.kiota.typeAccessModifier -eq 'Internal') -Message 'Generated contract types must remain Internal.'
Assert-Condition -Condition ($settings.kiota.includeAdditionalData -eq $true) -Message 'Generated contract AdditionalData must remain internal and enabled.'
Assert-Condition -Condition ($settings.kiota.excludeBackwardCompatible -eq $true) -Message 'Generated contract output must exclude backward-compatible Kiota artifacts.'

Invoke-DotNet -Arguments @('tool', 'restore')
[System.IO.Directory]::CreateDirectory($intermediateDirectory) | Out-Null

foreach ($contract in $settings.contracts) {
    $source = Get-BaselineSource -Manifest $baselineManifest -SourceId ([string] $contract.baselineSourceId)
    [string] $sourcePath = Get-VerifiedBaselineSourcePath -Source $source
    $document = Get-JsonHashtable -Path $sourcePath
    $removals = @($contract.normalization.responsePropertyRemovals)
    [int] $removalCount = Normalize-OpenApiForKiota -Document $document -ResponsePropertyRemovals $removals
    Assert-Condition -Condition ($removalCount -eq [int] $contract.normalization.expectedRemovalCount) -Message "Normalization for '$($contract.baselineSourceId)' removed $removalCount properties instead of $($contract.normalization.expectedRemovalCount)."

    [string] $normalizedPath = Join-Path $intermediateDirectory "$($contract.baselineSourceId).normalized.openapi.json"
    [string] $normalizedJson = $document | ConvertTo-Json -Depth 100
    [System.IO.File]::WriteAllText($normalizedPath, "$normalizedJson$([Environment]::NewLine)", [System.Text.UTF8Encoding]::new($false))

    Invoke-KiotaGeneration -Settings $settings -Contract $contract -OpenApiPath $normalizedPath
    [string] $outputDirectory = Join-Path $repositoryRoot ([string] $contract.outputDirectory)
    Remove-GeneratedDiagnosticLog -OutputDirectory $outputDirectory
    Remove-GeneratedSourceTrailingWhitespace -OutputDirectory $outputDirectory
}

if ($Verify) {
    $outputDirectories = @($settings.contracts | ForEach-Object { [string] $_.outputDirectory })
    $generatedFiles = @(
        foreach ($outputDirectory in $outputDirectories) {
            [string] $fullOutputDirectory = Join-Path $repositoryRoot $outputDirectory

            Get-ChildItem -LiteralPath $fullOutputDirectory -File -Recurse |
                ForEach-Object {
                    [System.IO.Path]::GetRelativePath($repositoryRoot, $_.FullName).Replace('\', '/')
                }
        }
    )
    $trackedFiles = @(& git ls-files -- $outputDirectories)
    $untrackedFiles = @($generatedFiles | Where-Object { $_ -notin $trackedFiles })

    if ($untrackedFiles.Count -gt 0) {
        $untrackedFiles | ForEach-Object { Write-Host $_ }
        throw 'Generated internal contracts are not tracked. Add the reviewed output before running verification.'
    }

    & git diff --quiet -- $outputDirectories

    if ($LASTEXITCODE -ne 0) {
        & git diff -- $outputDirectories
        throw 'Generated internal contracts are not current. Run eng/Invoke-GenerateInternalContracts.ps1 and commit the reviewed output.'
    }
}
