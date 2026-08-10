[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $Tag,

    [string] $GitHubOutput
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$tagPattern = '^v(?<version>(?<major>2)\.(?<minor>0|[1-9][0-9]*)\.(?<patch>0|[1-9][0-9]*)(?:-(?<prerelease>[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?)$'

if ($Tag -notmatch $tagPattern) {
    throw "Release tag '$Tag' must be v2.<minor>.<patch> with an optional NuGet-compatible prerelease suffix."
}

$version = $Matches['version']
$major = $Matches['major']
$minor = $Matches['minor']
$patch = $Matches['patch']
$prerelease = $Matches['prerelease']

if (-not [string]::IsNullOrEmpty($prerelease)) {
    foreach ($identifier in $prerelease.Split('.')) {
        if ([System.Text.RegularExpressions.Regex]::IsMatch($identifier, '^[0-9]+$') -and ($identifier.Length -gt 1) -and $identifier.StartsWith('0', [System.StringComparison]::Ordinal)) {
            throw "Release tag '$Tag' contains a numeric prerelease identifier with a leading zero."
        }
    }
}

$result = [pscustomobject]@{
    Tag = $Tag
    Version = $version
    FileVersion = "$major.$minor.$patch.0"
    IsPrerelease = -not [string]::IsNullOrEmpty($prerelease)
}

if (-not [string]::IsNullOrWhiteSpace($GitHubOutput)) {
    Add-Content -LiteralPath $GitHubOutput -Value @(
        "version=$($result.Version)",
        "file_version=$($result.FileVersion)",
        "is_prerelease=$($result.IsPrerelease.ToString().ToLowerInvariant())"
    ) -Encoding utf8NoBOM
}

$result | ConvertTo-Json -Compress
