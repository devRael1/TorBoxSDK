[CmdletBinding()]
param(
    [switch]$SkipPack,
    [switch]$IncludeReleaseContract
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $repositoryRoot

$solutionPath = 'TorBoxSDK.V2.slnx'
$unitTestProjectPath = 'tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj'
$contractTestProjectPath = 'tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj'
$coreProjectPath = 'src/TorBoxSDK.V2/TorBoxSDK.V2.csproj'
$dependencyInjectionProjectPath = 'src/TorBoxSDK.DependencyInjection.V2/TorBoxSDK.DependencyInjection.V2.csproj'
$prohibitedIntegrationProjectName = 'TorBoxSDK.V2.IntegrationTests.csproj'
$packageOutputPath = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts/v2-packages'))
$repositoryRootWithSeparator = $repositoryRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar

$requiredRelativePaths = @(
    $solutionPath,
    $unitTestProjectPath,
    $contractTestProjectPath,
    $coreProjectPath,
    $dependencyInjectionProjectPath,
    'contracts/torbox/baseline/openapi.json',
    'contracts/torbox/baseline/manifest.json',
    'contracts/torbox/relay/openapi.json',
    'contracts/torbox/relay/manifest.json',
    'contracts/torbox/search/operations.json',
    'contracts/torbox/search/manifest.json',
    'contracts/torbox/sources.json',
    'contracts/torbox/divergences.json',
    'contracts/torbox/coverage.json',
    'src/TorBoxSDK.V2/packages.lock.json',
    'src/TorBoxSDK.DependencyInjection.V2/packages.lock.json',
    'src/TorBoxSDK.V2.Examples/packages.lock.json',
    'tests/TorBoxSDK.V2.Testing/packages.lock.json',
    'tests/TorBoxSDK.V2.UnitTests/packages.lock.json',
    'tests/TorBoxSDK.V2.ContractTests/packages.lock.json'
)

function Assert-RequiredFiles {
    foreach ($relativePath in $requiredRelativePaths) {
        $absolutePath = Join-Path $repositoryRoot $relativePath
        if (-not (Test-Path -LiteralPath $absolutePath -PathType Leaf)) {
            throw "Required V2 validation file is missing: $relativePath"
        }
    }
}

function Get-DeterministicSolutionProjectPaths {
    [xml]$solution = Get-Content -LiteralPath (Join-Path $repositoryRoot $solutionPath) -Raw
    $projectPaths = @(
        $solution.SelectNodes("//*[local-name()='Project']") |
            ForEach-Object { $_.GetAttribute('Path').Replace('\', '/') }
    )
    if ($projectPaths.Count -eq 0) {
        throw "The deterministic V2 solution contains no projects: $solutionPath"
    }

    return $projectPaths
}

function Assert-DeterministicSolutionExcludesIntegrationTests {
    $expectedProjectPaths = @(
        'src/TorBoxSDK.DependencyInjection.V2/TorBoxSDK.DependencyInjection.V2.csproj',
        'src/TorBoxSDK.V2.Examples/TorBoxSDK.V2.Examples.csproj',
        'src/TorBoxSDK.V2/TorBoxSDK.V2.csproj',
        'tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj',
        'tests/TorBoxSDK.V2.Testing/TorBoxSDK.V2.Testing.csproj',
        'tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj'
    )
    $actualProjectPaths = @(Get-DeterministicSolutionProjectPaths | Sort-Object)
    $solutionDifference = @(Compare-Object -ReferenceObject ($expectedProjectPaths | Sort-Object) -DifferenceObject $actualProjectPaths)
    if ($solutionDifference.Count -ne 0) {
        throw "The deterministic V2 solution must contain exactly the approved offline projects: $($solutionDifference | Out-String)"
    }

    $visitedProjectPaths = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $pendingProjectPaths = [System.Collections.Generic.Queue[string]]::new()
    foreach ($relativeProjectPath in $actualProjectPaths) {
        $pendingProjectPaths.Enqueue([System.IO.Path]::GetFullPath((Join-Path $repositoryRoot $relativeProjectPath)))
    }

    while ($pendingProjectPaths.Count -ne 0) {
        $projectPath = $pendingProjectPaths.Dequeue()
        if (-not $visitedProjectPaths.Add($projectPath)) {
            continue
        }

        [xml]$project = Get-Content -LiteralPath $projectPath -Raw
        foreach ($projectReference in @($project.SelectNodes("//*[local-name()='ProjectReference']"))) {
            $referenceInclude = $projectReference.GetAttribute('Include')
            if ([string]::IsNullOrWhiteSpace($referenceInclude)) {
                throw "Project '$projectPath' contains a ProjectReference without Include metadata."
            }

            $referencedProjectPath = [System.IO.Path]::GetFullPath((Join-Path (Split-Path -Parent $projectPath) $referenceInclude))
            if ([System.IO.Path]::GetFileName($referencedProjectPath) -ieq $prohibitedIntegrationProjectName) {
                throw "The deterministic V2 project graph must not reference '$prohibitedIntegrationProjectName': $projectPath"
            }

            if (-not (Test-Path -LiteralPath $referencedProjectPath -PathType Leaf)) {
                throw "Project '$projectPath' references a missing project: $referenceInclude"
            }

            $pendingProjectPaths.Enqueue($referencedProjectPath)
        }
    }
}

function Invoke-DotNet {
    param(
        [Parameter(Mandatory)]
        [string[]]$Arguments,
        [Parameter(Mandatory)]
        [string]$FailureMessage
    )

    Write-Host ("+ dotnet {0}" -f ($Arguments -join ' '))
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw $FailureMessage
    }
}

function Assert-DescendantPackageOutputPath {
    if (-not $packageOutputPath.StartsWith($repositoryRootWithSeparator, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "The package output path must be a descendant of the repository root: $packageOutputPath"
    }

    $relativePackageOutputPath = [System.IO.Path]::GetRelativePath($repositoryRoot, $packageOutputPath)
    $pathSegments = @($relativePackageOutputPath.Split([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $currentPath = $repositoryRoot
    foreach ($pathSegment in $pathSegments) {
        $currentPath = Join-Path $currentPath $pathSegment
        if (Test-Path -LiteralPath $currentPath) {
            $currentItem = Get-Item -LiteralPath $currentPath -Force
            if ($currentItem.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
                throw "The package output path contains a reparse point: $currentPath"
            }
        }
    }

    $packageOutputParentPath = Split-Path -Parent $packageOutputPath
    if (-not (Test-Path -LiteralPath $packageOutputParentPath)) {
        New-Item -ItemType Directory -Path $packageOutputParentPath | Out-Null
    }

    $resolvedRepositoryRoot = (Resolve-Path -LiteralPath $repositoryRoot).Path
    $resolvedRepositoryRootWithSeparator = $resolvedRepositoryRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
    $resolvedPackageOutputParentPath = (Resolve-Path -LiteralPath $packageOutputParentPath).Path
    $resolvedPackageOutputPath = [System.IO.Path]::GetFullPath((Join-Path $resolvedPackageOutputParentPath (Split-Path -Leaf $packageOutputPath)))
    if (-not $resolvedPackageOutputPath.StartsWith($resolvedRepositoryRootWithSeparator, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "The resolved package output path must remain below the repository root: $resolvedPackageOutputPath"
    }

    $currentPath = $repositoryRoot
    foreach ($pathSegment in $pathSegments) {
        $currentPath = Join-Path $currentPath $pathSegment
        if (Test-Path -LiteralPath $currentPath) {
            $currentItem = Get-Item -LiteralPath $currentPath -Force
            if ($currentItem.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
                throw "The package output path contains a reparse point: $currentPath"
            }
        }
    }
}

function ConvertTo-NormalizedTargetFramework {
    param(
        [Parameter(Mandatory)]
        [string]$TargetFramework
    )

    switch -Regex ($TargetFramework.Trim()) {
        '^netstandard2\.0$|^\.NETStandard2\.0$' { return 'netstandard2.0' }
        '^(?:net|\.NETCoreApp)(?<major>6|7|8|9|10)\.0$' { return "net$($Matches['major']).0" }
        default { throw "Package dependency group uses an unsupported target framework '$TargetFramework'." }
    }
}

function Get-PackageMetadata {
    param(
        [Parameter(Mandatory)]
        [string]$PackagePath
    )

    $archive = [System.IO.Compression.ZipFile]::OpenRead($PackagePath)
    try {
        $entries = @($archive.Entries)
        $nuspecEntries = @($entries | Where-Object {
            $_.FullName.EndsWith('.nuspec', [System.StringComparison]::OrdinalIgnoreCase)
        })
        if ($nuspecEntries.Count -ne 1) {
            throw "Package '$PackagePath' must contain exactly one nuspec file."
        }

        $reader = [System.IO.StreamReader]::new($nuspecEntries[0].Open())
        try {
            [xml]$nuspec = $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }

        $metadata = $nuspec.SelectSingleNode("/*[local-name()='package']/*[local-name()='metadata']")
        if ($null -eq $metadata) {
            throw "Package '$PackagePath' has no nuspec metadata."
        }

        $dependenciesElement = $metadata.SelectSingleNode("./*[local-name()='dependencies']")
        $dependencyGroups = @()
        if ($null -ne $dependenciesElement) {
            $groupNodes = @($dependenciesElement.SelectNodes("./*[local-name()='group']"))
            $ungroupedDependencies = @($dependenciesElement.SelectNodes("./*[local-name()='dependency']"))
            if ($ungroupedDependencies.Count -ne 0) {
                throw "Package '$PackagePath' must declare dependencies in target-framework groups."
            }

            $seenTargetFrameworks = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
            foreach ($groupNode in $groupNodes) {
                $targetFramework = ConvertTo-NormalizedTargetFramework $groupNode.GetAttribute('targetFramework')
                if (-not $seenTargetFrameworks.Add($targetFramework)) {
                    throw "Package '$PackagePath' contains duplicate '$targetFramework' dependency groups."
                }

                $dependencies = @()
                $seenDependencyIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
                foreach ($dependencyNode in @($groupNode.SelectNodes("./*[local-name()='dependency']"))) {
                    $dependencyId = [string]$dependencyNode.GetAttribute('id')
                    $dependencyVersion = [string]$dependencyNode.GetAttribute('version')
                    if ([string]::IsNullOrWhiteSpace($dependencyId) -or [string]::IsNullOrWhiteSpace($dependencyVersion)) {
                        throw "Package '$PackagePath' has a dependency without an id or version in '$targetFramework'."
                    }

                    if (-not $seenDependencyIds.Add($dependencyId)) {
                        throw "Package '$PackagePath' contains duplicate dependency '$dependencyId' in '$targetFramework'."
                    }

                    $dependencies += [pscustomobject]@{
                        Id = $dependencyId
                        Version = $dependencyVersion
                    }
                }

                $dependencyGroups += [pscustomobject]@{
                    TargetFramework = $targetFramework
                    Dependencies = @($dependencies)
                }
            }
        }

        return [pscustomobject]@{
            Id = [string]$metadata.SelectSingleNode("./*[local-name()='id']").InnerText
            Version = [string]$metadata.SelectSingleNode("./*[local-name()='version']").InnerText
            DependencyGroups = @($dependencyGroups)
            EntryNames = @($entries | ForEach-Object { $_.FullName })
        }
    }
    finally {
        $archive.Dispose()
    }
}

function Assert-PackageContents {
    param(
        [Parameter(Mandatory)]
        [object]$Package,
        [Parameter(Mandatory)]
        [string]$ExpectedId,
        [Parameter(Mandatory)]
        [string]$ExpectedAssemblyName
    )

    if ($Package.Id -cne $ExpectedId -or [string]::IsNullOrWhiteSpace($Package.Version)) {
        throw "Unexpected package identity '$($Package.Id) $($Package.Version)'."
    }

    foreach ($targetFramework in @('netstandard2.0', 'net6.0', 'net7.0', 'net8.0', 'net9.0', 'net10.0')) {
        $requiredEntry = "lib/$targetFramework/$ExpectedAssemblyName.dll"
        if ($Package.EntryNames -cnotcontains $requiredEntry) {
            throw "Package '$ExpectedId' is missing '$requiredEntry'."
        }
    }
}

function Assert-DependencyIdSet {
    param(
        [Parameter(Mandatory)]
        [string]$PackageId,
        [Parameter(Mandatory)]
        [string]$TargetFramework,
        [AllowEmptyCollection()]
        [object[]]$Dependencies = @(),
        [AllowEmptyCollection()]
        [string[]]$ExpectedDependencyIds = @()
    )

    $actualDependencyIds = @($Dependencies | ForEach-Object { [string]$_.Id } | Sort-Object -Unique)
    $unexpectedDependencyIds = @($actualDependencyIds | Where-Object { $ExpectedDependencyIds -notcontains $_ })
    if ($unexpectedDependencyIds.Count -ne 0) {
        throw "Package '$PackageId' has unexpected dependencies in '$TargetFramework': $($unexpectedDependencyIds -join ', ')."
    }

    $missingDependencyIds = @($ExpectedDependencyIds | Where-Object { $actualDependencyIds -notcontains $_ })
    if ($missingDependencyIds.Count -ne 0) {
        throw "Package '$PackageId' is missing required dependencies in '$TargetFramework': $($missingDependencyIds -join ', ')."
    }
}

function Assert-MicrosoftExtensionsVersionPolicy {
    param(
        [Parameter(Mandatory)]
        [string]$TargetFramework,
        [Parameter(Mandatory)]
        [object[]]$Dependencies
    )

    $expectedMajorByTargetFramework = @{
        'netstandard2.0' = 10
        'net6.0' = 8
        'net7.0' = 8
        'net8.0' = 10
        'net9.0' = 10
        'net10.0' = 10
    }
    $expectedMajor = [int]$expectedMajorByTargetFramework[$TargetFramework]
    foreach ($dependency in $Dependencies) {
        if ($dependency.Version -notmatch '^(?<major>[0-9]+)\.[0-9]+\.[0-9]+(?:-[0-9A-Za-z.-]+)?$') {
            throw "Microsoft.Extensions dependency '$($dependency.Id)' in '$TargetFramework' must use a plain semantic version, not '$($dependency.Version)'."
        }

        if ([int]$Matches['major'] -ne $expectedMajor) {
            throw "Microsoft.Extensions dependency '$($dependency.Id)' in '$TargetFramework' must use major version '$expectedMajor', not '$($dependency.Version)'."
        }
    }
}

function Assert-PackageDependencyGroups {
    param(
        [Parameter(Mandatory)]
        [object]$Package,
        [Parameter(Mandatory)]
        [ValidateSet('Core', 'DependencyInjection')]
        [string]$PackageKind,
        [Parameter(Mandatory)]
        [string]$ExpectedCoreVersion
    )

    $expectedTargetFrameworks = @('netstandard2.0', 'net6.0', 'net7.0', 'net8.0', 'net9.0', 'net10.0')
    $actualTargetFrameworks = @($Package.DependencyGroups | ForEach-Object { [string]$_.TargetFramework } | Sort-Object -Unique)
    $unexpectedTargetFrameworks = @($actualTargetFrameworks | Where-Object { $expectedTargetFrameworks -notcontains $_ })
    $missingTargetFrameworks = @($expectedTargetFrameworks | Where-Object { $actualTargetFrameworks -notcontains $_ })
    if ($unexpectedTargetFrameworks.Count -ne 0 -or $missingTargetFrameworks.Count -ne 0) {
        throw "Package '$($Package.Id)' dependency groups are invalid. Missing: $($missingTargetFrameworks -join ', '); unexpected: $($unexpectedTargetFrameworks -join ', ')."
    }

    $groupsByTargetFramework = @{}
    foreach ($dependencyGroup in $Package.DependencyGroups) {
        $groupsByTargetFramework[$dependencyGroup.TargetFramework] = $dependencyGroup
    }

    foreach ($targetFramework in $expectedTargetFrameworks) {
        $dependencies = @($groupsByTargetFramework[$targetFramework].Dependencies)
        if ($PackageKind -ceq 'Core') {
            [string[]]$expectedDependencyIds = @()
            if ($targetFramework -ceq 'netstandard2.0') {
                $expectedDependencyIds = @('System.Text.Json')
            }

            Assert-DependencyIdSet -PackageId $Package.Id -TargetFramework $targetFramework -Dependencies $dependencies -ExpectedDependencyIds $expectedDependencyIds
            continue
        }

        $expectedDependencyIds = @(
            'TorBoxSDK',
            'Microsoft.Extensions.Configuration.Abstractions',
            'Microsoft.Extensions.DependencyInjection.Abstractions',
            'Microsoft.Extensions.Http',
            'Microsoft.Extensions.Options',
            'Microsoft.Extensions.Options.ConfigurationExtensions'
        )
        Assert-DependencyIdSet $Package.Id $targetFramework $dependencies $expectedDependencyIds

        $coreDependency = @($dependencies | Where-Object { $_.Id -ceq 'TorBoxSDK' })
        if ($coreDependency.Count -ne 1 -or $coreDependency[0].Version -cne "[$ExpectedCoreVersion]") {
            throw "Package '$($Package.Id)' must depend on TorBoxSDK as exact range '[$ExpectedCoreVersion]' in '$targetFramework'."
        }

        $microsoftExtensionsDependencies = @($dependencies | Where-Object { $_.Id -like 'Microsoft.Extensions.*' })
        Assert-MicrosoftExtensionsVersionPolicy $targetFramework $microsoftExtensionsDependencies
    }
}

Assert-RequiredFiles
Assert-DeterministicSolutionExcludesIntegrationTests

Invoke-DotNet @('restore', $solutionPath, '--locked-mode') 'V2 locked restore failed.'
Invoke-DotNet @('build', $solutionPath, '--configuration', 'Release', '--no-restore') 'V2 Release build failed.'
Invoke-DotNet @('test', $unitTestProjectPath, '--configuration', 'Release', '--no-build', '--no-restore') 'V2 unit tests failed.'
Invoke-DotNet @('test', $contractTestProjectPath, '--configuration', 'Release', '--no-build', '--no-restore', '--filter', 'Category!=Release') 'V2 non-release contract tests failed.'

if ($IncludeReleaseContract) {
    Invoke-DotNet @('test', $contractTestProjectPath, '--configuration', 'Release', '--no-build', '--no-restore', '--filter', 'Category=Release') 'V2 release contract tests failed.'
}
else {
    Write-Host 'V2 release contract tests are excluded by default. Re-run with -IncludeReleaseContract after cutover eligibility is declared.'
}

if ($SkipPack) {
    Write-Host 'V2 package validation was skipped.'
    return
}

Assert-DescendantPackageOutputPath
if (Test-Path -LiteralPath $packageOutputPath) {
    Remove-Item -LiteralPath $packageOutputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $packageOutputPath | Out-Null

Invoke-DotNet @('pack', $coreProjectPath, '--configuration', 'Release', '--no-build', '--no-restore', '--output', $packageOutputPath) 'V2 core package creation failed.'
Invoke-DotNet @('pack', $dependencyInjectionProjectPath, '--configuration', 'Release', '--no-build', '--no-restore', '--output', $packageOutputPath) 'V2 dependency-injection package creation failed.'

$packageArtifacts = @(
    Get-ChildItem -LiteralPath $packageOutputPath -File -Filter '*.nupkg' |
        ForEach-Object {
            [pscustomobject]@{
                Path = $_.FullName
                Metadata = Get-PackageMetadata $_.FullName
            }
        }
)
$expectedPackageIds = @('TorBoxSDK', 'TorBoxSDK.DependencyInjection')
$actualPackageIds = @($packageArtifacts | ForEach-Object { $_.Metadata.Id } | Sort-Object -Unique)
$unexpectedPackageIds = @($actualPackageIds | Where-Object { $expectedPackageIds -notcontains $_ })
$missingPackageIds = @($expectedPackageIds | Where-Object { $actualPackageIds -notcontains $_ })
if ($packageArtifacts.Count -ne 2 -or $unexpectedPackageIds.Count -ne 0 -or $missingPackageIds.Count -ne 0) {
    throw "V2 package output must contain only the core and DI packages. Missing: $($missingPackageIds -join ', '); unexpected: $($unexpectedPackageIds -join ', ')."
}

$corePackageArtifact = @($packageArtifacts | Where-Object { $_.Metadata.Id -ceq 'TorBoxSDK' })
$dependencyInjectionPackageArtifact = @($packageArtifacts | Where-Object { $_.Metadata.Id -ceq 'TorBoxSDK.DependencyInjection' })
if ($corePackageArtifact.Count -ne 1 -or $dependencyInjectionPackageArtifact.Count -ne 1) {
    throw 'V2 package output must contain exactly one core package and one dependency-injection package.'
}

$corePackage = $corePackageArtifact[0].Metadata
$dependencyInjectionPackage = $dependencyInjectionPackageArtifact[0].Metadata
Assert-PackageContents $corePackage 'TorBoxSDK' 'TorBoxSDK'
Assert-PackageContents $dependencyInjectionPackage 'TorBoxSDK.DependencyInjection' 'TorBoxSDK.DependencyInjection'
Assert-PackageDependencyGroups $corePackage 'Core' $corePackage.Version
Assert-PackageDependencyGroups $dependencyInjectionPackage 'DependencyInjection' $corePackage.Version

Write-Host "V2 packages were validated in '$packageOutputPath'."
