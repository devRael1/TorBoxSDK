param(
    [string]$Owner = "devRael1",
    [string]$Repository = "TorBoxSDK",
    [string]$Branch = "main"
)

$ErrorActionPreference = "Stop"

$docsRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$repositoryRoot = (Resolve-Path (Join-Path $docsRoot "..")).Path
$sourcePath = Join-Path $PSScriptRoot "homepage-content.md"
$readmePath = Join-Path $repositoryRoot "README.md"
$docsIndexPath = Join-Path $docsRoot "index.md"

function Convert-MarkdownLinksForDocs {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Content,

        [Parameter(Mandatory = $true)]
        [string]$Owner,

        [Parameter(Mandatory = $true)]
        [string]$Repository,

        [Parameter(Mandatory = $true)]
        [string]$Branch
    )

    $repositoryBaseUrl = "https://github.com/$Owner/$Repository"

    return [System.Text.RegularExpressions.Regex]::Replace(
        $Content,
        "\[(?<text>[^\]]+)\]\((?<target>[^)]+)\)",
        {
            param($match)

            $text = $match.Groups["text"].Value
            $target = $match.Groups["target"].Value

            if (
                $target.StartsWith("http://", [System.StringComparison]::OrdinalIgnoreCase) -or
                $target.StartsWith("https://", [System.StringComparison]::OrdinalIgnoreCase) -or
                $target.StartsWith("#", [System.StringComparison]::Ordinal)
            ) {
                return $match.Value
            }

            if ($target.StartsWith("docs/", [System.StringComparison]::Ordinal)) {
                return "[$text]($($target.Substring(5)))"
            }

            $isDirectory = $target.EndsWith("/", [System.StringComparison]::Ordinal)
            $pathKind = if ($isDirectory) { "tree" } else { "blob" }

            return "[$text]($repositoryBaseUrl/$pathKind/$Branch/$target)"
        })
}

function Write-Utf8File {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter(Mandatory = $true)]
        [string]$Content
    )

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($Path, $Content, $utf8NoBom)
}

$sharedContent = (Get-Content -Path $sourcePath -Raw).TrimEnd()
$readmeContent = @(
    $sharedContent
    ""
) -join "`n"

$docsBody = Convert-MarkdownLinksForDocs -Content $sharedContent -Owner $Owner -Repository $Repository -Branch $Branch
$docsFrontMatter = @(
    "---"
    "uid: index"
    "title: TorBoxSDK Documentation"
    "description: Unofficial C# SDK for the TorBox API with typed clients for Main, Search, and Relay APIs."
    "---"
    ""
) -join "`n"
$docsIndexContent = $docsFrontMatter + $docsBody + "`n"

Write-Utf8File -Path $readmePath -Content $readmeContent
Write-Utf8File -Path $docsIndexPath -Content $docsIndexContent