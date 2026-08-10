[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $OutputPath,

    [string] $BaselineDirectory = (Join-Path $PSScriptRoot '..\contracts\baseline'),

    [switch] $AllowNetwork,

    [switch] $FailOnDrift
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Test-PathWithinDirectory {
    param(
        [Parameter(Mandatory)]
        [string] $Path,

        [Parameter(Mandatory)]
        [string] $Directory
    )

    $fullDirectory = [System.IO.Path]::GetFullPath($Directory).TrimEnd(
        [System.IO.Path]::DirectorySeparatorChar,
        [System.IO.Path]::AltDirectorySeparatorChar)
    $fullPath = [System.IO.Path]::GetFullPath($Path)
    $directoryPrefix = $fullDirectory + [System.IO.Path]::DirectorySeparatorChar

    return $fullPath.StartsWith($directoryPrefix, [System.StringComparison]::Ordinal)
}

if (-not $AllowNetwork) {
    throw 'Remote monitoring is opt-in. Re-run with -AllowNetwork to perform public GET requests.'
}

$fullBaselineDirectory = [System.IO.Path]::GetFullPath($BaselineDirectory)
$manifestPath = Join-Path $fullBaselineDirectory 'manifest.json'
if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
    throw "Contract baseline manifest was not found: $manifestPath"
}

$fullOutputPath = [System.IO.Path]::GetFullPath($OutputPath)
if (Test-PathWithinDirectory -Path $fullOutputPath -Directory $fullBaselineDirectory) {
    throw 'The monitor report must be written outside contracts/baseline so it cannot replace a snapshot.'
}

if (Test-Path -LiteralPath $fullOutputPath) {
    throw "Refusing to overwrite an existing monitor report: $fullOutputPath"
}

$outputDirectory = Split-Path -Parent $fullOutputPath
if (-not [System.IO.Directory]::Exists($outputDirectory)) {
    [System.IO.Directory]::CreateDirectory($outputDirectory) | Out-Null
}

$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$capturedSources = @($manifest.sources | Where-Object { $_.availability -eq 'captured' })
$results = [System.Collections.Generic.List[object]]::new()
$client = [System.Net.Http.HttpClient]::new()

try {
    foreach ($source in $capturedSources) {
        $request = [System.Net.Http.HttpRequestMessage]::new(
            [System.Net.Http.HttpMethod]::Get,
            [System.Uri] $source.sourceUrl)

        if ($null -ne $source.requestHeaders) {
            foreach ($header in $source.requestHeaders.PSObject.Properties) {
                [void] $request.Headers.TryAddWithoutValidation($header.Name, [string] $header.Value)
            }
        }

        try {
            $response = $client.SendAsync($request).GetAwaiter().GetResult()
            try {
                $bytes = $response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult()
                $sha256 = [System.Convert]::ToHexString(
                    [System.Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant()
                $status = [int] $response.StatusCode
                $contentType = if ($null -eq $response.Content.Headers.ContentType) {
                    $null
                }
                else {
                    $response.Content.Headers.ContentType.ToString()
                }
                $etag = if ($null -eq $response.Headers.ETag) {
                    $null
                }
                else {
                    $response.Headers.ETag.Tag
                }
                $lastModified = if ($null -eq $response.Content.Headers.LastModified) {
                    $null
                }
                else {
                    $response.Content.Headers.LastModified.Value.ToUniversalTime().ToString('O')
                }
                $hasDrift = $status -ne [int] $source.responseStatus -or
                    $bytes.LongLength -ne [long] $source.contentLength -or
                    -not [string]::Equals($sha256, [string] $source.sha256, [System.StringComparison]::OrdinalIgnoreCase)

                $results.Add([ordered]@{
                    id = $source.id
                    sourceUrl = $source.sourceUrl
                    effectiveUrl = $response.RequestMessage.RequestUri.AbsoluteUri
                    observedAtUtc = [System.DateTimeOffset]::UtcNow.ToString('O')
                    responseStatus = $status
                    contentType = $contentType
                    etag = $etag
                    lastModified = $lastModified
                    contentLength = $bytes.LongLength
                    sha256 = $sha256
                    hasDrift = $hasDrift
                    error = $null
                })
            }
            finally {
                $response.Dispose()
            }
        }
        catch {
            $results.Add([ordered]@{
                id = $source.id
                sourceUrl = $source.sourceUrl
                effectiveUrl = $null
                observedAtUtc = [System.DateTimeOffset]::UtcNow.ToString('O')
                responseStatus = $null
                contentType = $null
                etag = $null
                lastModified = $null
                contentLength = $null
                sha256 = $null
                hasDrift = $true
                error = $_.Exception.Message
            })
        }
        finally {
            $request.Dispose()
        }
    }
}
finally {
    $client.Dispose()
}

$report = [ordered]@{
    schemaVersion = 1
    monitor = 'manual-opt-in'
    generatedAtUtc = [System.DateTimeOffset]::UtcNow.ToString('O')
    baselineDirectory = $fullBaselineDirectory
    baselineId = $manifest.baselineId
    snapshotsModified = $false
    capturedSources = $results
    unavailableSources = @($manifest.sources | Where-Object { $_.availability -eq 'unavailable' } | ForEach-Object {
        [ordered]@{
            id = $_.id
            family = $_.family
            format = $_.format
            sourceUrl = $_.sourceUrl
            reason = $_.reason
        }
    })
}

[System.IO.File]::WriteAllText(
    $fullOutputPath,
    ($report | ConvertTo-Json -Depth 8),
    [System.Text.UTF8Encoding]::new($false))

$hasDrift = @($results | Where-Object { $_.hasDrift }).Count -gt 0
Write-Output "Contract monitor report: $fullOutputPath"
if ($FailOnDrift -and $hasDrift) {
    throw 'Remote contract drift was detected. Inspect the report; snapshots were not modified.'
}
