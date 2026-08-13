using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;

namespace TorBoxSDK.V2.ContractTests.Infrastructure;

internal sealed class ContractBaseline
{
    private const string TransactionDirectoryName = ".update-transaction";
    private const string CandidateGenerationName = "candidate";
    private const string DocumentedOperationsFormat = "torbox-sdk/documented-operations/v1";

    private static readonly HashSet<string> HttpMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "get", "put", "post", "delete", "options", "head", "patch", "trace"
    };

    private static readonly IReadOnlyDictionary<string, ExpectedSource> ExpectedSources =
        new Dictionary<string, ExpectedSource>(StringComparer.Ordinal)
        {
            ["main"] = new("openapi", "baseline/openapi.json", "baseline/manifest.json", "https://api.torbox.app/openapi.json", ContractReleaseEligibility.Eligible),
            ["relay"] = new("openapi", "relay/openapi.json", "relay/manifest.json", "https://relay.torbox.app/openapi.json", ContractReleaseEligibility.Eligible),
            ["search"] = new("documented-operations", "search/operations.json", "search/manifest.json", "https://www.postman.com/torbox/torbox-api/documentation/u47iwao/search-api", ContractReleaseEligibility.RequiresLiveValidation)
        };

    private ContractBaseline(IReadOnlyList<ContractSource> sources, IReadOnlyList<ContractOperation> operations)
    {
        Sources = sources;
        Operations = operations;
        OperationIdentities = operations.Select(static operation => operation.Identity).ToHashSet(StringComparer.Ordinal);
        RequestBodyOperationIdentities = operations
            .Where(static operation => operation.HasRequestBody)
            .Select(static operation => operation.Identity)
            .ToHashSet(StringComparer.Ordinal);
    }

    internal IReadOnlyList<ContractSource> Sources { get; }

    internal IReadOnlyList<ContractOperation> Operations { get; }

    internal IReadOnlySet<string> OperationIdentities { get; }

    internal IReadOnlySet<string> RequestBodyOperationIdentities { get; }

    internal static ContractBaseline Load(string directory)
    {
        string contractDirectory = GetContractDirectory(directory);
        IReadOnlyList<SourceRegistryEntry> registry = LoadRegistry(contractDirectory);
        ActiveTransaction? transaction = LoadActiveTransaction(contractDirectory, registry);

        List<ContractSource> sources = [];
        List<ContractOperation> operations = [];
        foreach (SourceRegistryEntry entry in registry)
        {
            string sourceRoot = transaction is not null && string.Equals(transaction.SourceId, entry.Id, StringComparison.Ordinal)
                ? transaction.CandidateDirectory
                : contractDirectory;
            string snapshotPath = ResolveRelativePath(sourceRoot, entry.SnapshotPath);
            string manifestPath = ResolveRelativePath(sourceRoot, entry.ManifestPath);
            ContractSource source = LoadSource(entry, snapshotPath, manifestPath);
            sources.Add(source);
            operations.AddRange(source.Operations);
        }

        ContractBaseline baseline = new(sources, operations);
        string coveragePath = transaction?.CoveragePath ?? Path.Combine(contractDirectory, "coverage.json");
        IReadOnlyList<CoverageRecord> coverage = CoverageManifest.Load(coveragePath).Records;
        CoverageManifest.EnsureOperationSetMatches(coverage, baseline);
        CoverageManifest.EnsureRecordShapesMatchSources(coverage, baseline);
        return baseline;
    }

    private static string GetContractDirectory(string directory)
    {
        string fullPath = Path.GetFullPath(directory);
        if (string.Equals(Path.GetFileName(fullPath), "baseline", StringComparison.OrdinalIgnoreCase))
        {
            DirectoryInfo? parent = Directory.GetParent(fullPath);
            return parent?.FullName ?? throw new InvalidDataException("The contract baseline directory does not have a parent directory.");
        }

        return fullPath;
    }

    private static IReadOnlyList<SourceRegistryEntry> LoadRegistry(string contractDirectory)
    {
        string registryPath = Path.Combine(contractDirectory, "sources.json");
        if (!File.Exists(registryPath))
        {
            throw new FileNotFoundException("The contract source registry is missing.", registryPath);
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllBytes(registryPath));
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object || GetRequiredInt32(root, "schemaVersion") != 1 ||
                !root.TryGetProperty("sources", out JsonElement sourceElements) || sourceElements.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidDataException("The contract source registry must use schema version 1 and contain a sources array.");
            }

            List<SourceRegistryEntry> sources = [];
            HashSet<string> ids = new(StringComparer.Ordinal);
            foreach (JsonElement sourceElement in sourceElements.EnumerateArray())
            {
                string id = GetRequiredString(sourceElement, "id", "source registry");
                string kind = GetRequiredString(sourceElement, "kind", "source registry");
                string snapshotPath = NormalizeRelativePath(GetRequiredString(sourceElement, "snapshotPath", "source registry"));
                string manifestPath = NormalizeRelativePath(GetRequiredString(sourceElement, "manifestPath", "source registry"));
                string eligibilityText = GetRequiredString(sourceElement, "releaseEligibility", "source registry");
                ContractReleaseEligibility eligibility = eligibilityText switch
                {
                    "eligible" => ContractReleaseEligibility.Eligible,
                    "requires-live-validation" => ContractReleaseEligibility.RequiresLiveValidation,
                    _ => throw new InvalidDataException($"Contract source '{id}' has an invalid release eligibility '{eligibilityText}'.")
                };

                if (!ids.Add(id))
                {
                    throw new InvalidDataException($"The contract source registry contains duplicate source ID '{id}'.");
                }

                if (!ExpectedSources.TryGetValue(id, out ExpectedSource? expected) ||
                    !string.Equals(kind, expected.Kind, StringComparison.Ordinal) ||
                    !string.Equals(snapshotPath, expected.SnapshotPath, StringComparison.Ordinal) ||
                    !string.Equals(manifestPath, expected.ManifestPath, StringComparison.Ordinal) ||
                    eligibility != expected.ReleaseEligibility)
                {
                    throw new InvalidDataException($"Contract source '{id}' does not match the approved source registry.");
                }

                sources.Add(new SourceRegistryEntry(id, kind, snapshotPath, manifestPath, eligibility));
            }

            if (sources.Count != ExpectedSources.Count || !ids.SetEquals(ExpectedSources.Keys))
            {
                throw new InvalidDataException("The contract source registry must contain exactly main, relay, and search.");
            }

            return sources;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("The contract source registry contains malformed JSON.", exception);
        }
    }

    private static ActiveTransaction? LoadActiveTransaction(string contractDirectory, IReadOnlyList<SourceRegistryEntry> registry)
    {
        string transactionDirectory = Path.Combine(contractDirectory, TransactionDirectoryName);
        string journalPath = Path.Combine(transactionDirectory, "journal.json");
        if (!File.Exists(journalPath))
        {
            return null;
        }

        try
        {
            using JsonDocument journal = JsonDocument.Parse(File.ReadAllBytes(journalPath));
            JsonElement root = journal.RootElement;
            string activeGeneration = GetRequiredString(root, "activeGeneration", "contract transaction journal");
            if (!string.Equals(activeGeneration, CandidateGenerationName, StringComparison.Ordinal))
            {
                throw new InvalidDataException("The contract transaction journal does not identify a supported complete generation.");
            }

            string sourceId = root.TryGetProperty("sourceId", out JsonElement sourceIdElement) && sourceIdElement.ValueKind == JsonValueKind.String
                ? sourceIdElement.GetString() ?? string.Empty
                : "main";
            SourceRegistryEntry selectedSource = registry.SingleOrDefault(source => string.Equals(source.Id, sourceId, StringComparison.Ordinal))
                ?? throw new InvalidDataException($"The contract transaction selects unknown source '{sourceId}'.");
            if (!root.TryGetProperty("publishCoverage", out JsonElement publishCoverageElement) ||
                publishCoverageElement.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
            {
                throw new InvalidDataException("The contract transaction journal does not state whether coverage must be published.");
            }

            bool publishCoverage = publishCoverageElement.GetBoolean();
            string candidateDirectory = Path.Combine(transactionDirectory, CandidateGenerationName);
            string candidateSnapshotPath = ResolveRelativePath(candidateDirectory, selectedSource.SnapshotPath);
            string candidateManifestPath = ResolveRelativePath(candidateDirectory, selectedSource.ManifestPath);
            string candidateCoveragePath = Path.Combine(candidateDirectory, "coverage.json");
            if (!File.Exists(candidateSnapshotPath) || !File.Exists(candidateManifestPath) ||
                (publishCoverage && !File.Exists(candidateCoveragePath)))
            {
                throw new InvalidDataException("The contract transaction candidate is incomplete and cannot be recovered safely.");
            }

            string effectiveCoveragePath = publishCoverage ? candidateCoveragePath : Path.Combine(contractDirectory, "coverage.json");
            return new ActiveTransaction(sourceId, candidateDirectory, effectiveCoveragePath);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("The contract transaction journal contains malformed JSON.", exception);
        }
    }

    private static ContractSource LoadSource(SourceRegistryEntry entry, string snapshotPath, string manifestPath)
    {
        if (!File.Exists(snapshotPath))
        {
            throw new FileNotFoundException($"The '{entry.Id}' contract snapshot is missing.", snapshotPath);
        }

        if (!File.Exists(manifestPath))
        {
            throw new FileNotFoundException($"The '{entry.Id}' contract manifest is missing.", manifestPath);
        }

        byte[] snapshotBytes = File.ReadAllBytes(snapshotPath);
        using JsonDocument snapshot = ParseJson(snapshotBytes, $"The '{entry.Id}' contract snapshot contains malformed JSON.");
        using JsonDocument manifestDocument = ParseJson(File.ReadAllBytes(manifestPath), $"The '{entry.Id}' contract manifest contains malformed JSON.");
        ContractManifest manifest = ParseManifest(entry, manifestDocument.RootElement);
        ValidateCommonManifest(entry, manifest, snapshotBytes);

        IReadOnlyList<ContractOperation> operations;
        if (string.Equals(entry.Kind, "openapi", StringComparison.Ordinal))
        {
            operations = ParseOpenApiOperations(entry.Id, snapshot.RootElement);
            ValidateOpenApiManifest(entry, manifest, snapshot.RootElement);
        }
        else if (string.Equals(entry.Kind, "documented-operations", StringComparison.Ordinal))
        {
            operations = ParseDocumentedOperations(entry.Id, snapshot.RootElement);
            ValidateDocumentedOperationsManifest(entry, manifest);
        }
        else
        {
            throw new InvalidDataException($"Contract source '{entry.Id}' has unsupported kind '{entry.Kind}'.");
        }

        return new ContractSource(entry.Id, entry.Kind, entry.ReleaseEligibility, snapshotBytes, manifest, operations);
    }

    private static ContractManifest ParseManifest(SourceRegistryEntry entry, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException($"The '{entry.Id}' contract manifest must be a JSON object.");
        }

        string retrievedAtText = GetRequiredString(root, "retrievedAtUtc", $"'{entry.Id}' manifest");
        if (!retrievedAtText.EndsWith("Z", StringComparison.Ordinal) ||
            !DateTimeOffset.TryParse(retrievedAtText, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset retrievedAtUtc) ||
            retrievedAtUtc.Offset != TimeSpan.Zero)
        {
            throw new InvalidDataException($"The '{entry.Id}' contract manifest retrieval timestamp must be a UTC timestamp.");
        }

        return new ContractManifest(
            GetRequiredString(root, "sourceUrl", $"'{entry.Id}' manifest"),
            retrievedAtUtc,
            GetOptionalString(root, "openApiVersion"),
            GetOptionalString(root, "sourceKind"),
            GetOptionalString(root, "releaseEligibility"),
            GetRequiredInt64(root, "byteLength"),
            GetRequiredString(root, "sha256", $"'{entry.Id}' manifest"));
    }

    private static void ValidateCommonManifest(SourceRegistryEntry entry, ContractManifest manifest, byte[] snapshotBytes)
    {
        ExpectedSource expected = ExpectedSources[entry.Id];
        if (!string.Equals(manifest.SourceUrl, expected.SourceUrl, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"The '{entry.Id}' contract manifest source URL is not the approved source URL.");
        }

        if (manifest.ByteLength != snapshotBytes.LongLength)
        {
            throw new InvalidDataException($"The '{entry.Id}' contract manifest byte length does not match the snapshot.");
        }

        string actualHash = CalculateSha256(snapshotBytes);
        if (manifest.Sha256.Length != 64 || !string.Equals(manifest.Sha256, actualHash, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"The '{entry.Id}' contract manifest SHA-256 does not match the snapshot.");
        }
    }

    private static void ValidateOpenApiManifest(SourceRegistryEntry entry, ContractManifest manifest, JsonElement root)
    {
        if (string.IsNullOrWhiteSpace(manifest.OpenApiVersion) ||
            !root.TryGetProperty("info", out JsonElement info) || info.ValueKind != JsonValueKind.Object ||
            !info.TryGetProperty("version", out JsonElement version) || version.ValueKind != JsonValueKind.String ||
            !string.Equals(manifest.OpenApiVersion, version.GetString(), StringComparison.Ordinal))
        {
            throw new InvalidDataException($"The '{entry.Id}' contract manifest OpenAPI version does not match the snapshot.");
        }

        if (manifest.SourceKind is not null || manifest.ReleaseEligibility is not null)
        {
            throw new InvalidDataException($"The '{entry.Id}' OpenAPI manifest contains documentation-only properties.");
        }
    }

    private static void ValidateDocumentedOperationsManifest(SourceRegistryEntry entry, ContractManifest manifest)
    {
        if (!string.Equals(manifest.SourceKind, "postman-documentation", StringComparison.Ordinal) ||
            !string.Equals(manifest.ReleaseEligibility, "requires-live-validation", StringComparison.Ordinal) ||
            manifest.OpenApiVersion is not null)
        {
            throw new InvalidDataException($"The '{entry.Id}' documentation manifest does not match the approved source facts.");
        }
    }

    private static IReadOnlyList<ContractOperation> ParseOpenApiOperations(string sourceId, JsonElement root)
    {
        if (!root.TryGetProperty("paths", out JsonElement paths) || paths.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException($"The '{sourceId}' OpenAPI document does not contain an object-valued paths property.");
        }

        List<ContractOperation> operations = [];
        HashSet<string> keys = new(StringComparer.Ordinal);
        foreach (JsonProperty path in paths.EnumerateObject())
        {
            if (path.Value.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            foreach (JsonProperty operation in path.Value.EnumerateObject())
            {
                if (!HttpMethods.Contains(operation.Name) || operation.Value.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                string operationKey = $"{operation.Name.ToUpperInvariant()} {path.Name}";
                if (!keys.Add(operationKey))
                {
                    throw new InvalidDataException($"The '{sourceId}' contract contains duplicate operation '{operationKey}'.");
                }

                bool hasRequestBody = operation.Value.TryGetProperty("requestBody", out JsonElement requestBody) && requestBody.ValueKind == JsonValueKind.Object;
                operations.Add(new ContractOperation(sourceId, operationKey, hasRequestBody));
            }
        }

        return operations;
    }

    private static IReadOnlyList<ContractOperation> ParseDocumentedOperations(string sourceId, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty("format", out JsonElement format) || format.ValueKind != JsonValueKind.String ||
            !string.Equals(format.GetString(), DocumentedOperationsFormat, StringComparison.Ordinal) ||
            !root.TryGetProperty("operations", out JsonElement operationElements) || operationElements.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException($"The '{sourceId}' documented operation inventory has an unsupported format.");
        }

        List<ContractOperation> operations = [];
        HashSet<string> keys = new(StringComparer.Ordinal);
        foreach (JsonElement operationElement in operationElements.EnumerateArray())
        {
            string method = GetRequiredString(operationElement, "method", $"'{sourceId}' operation").ToUpperInvariant();
            string path = GetRequiredString(operationElement, "path", $"'{sourceId}' operation");
            if (!HttpMethods.Contains(method) || !path.StartsWith("/", StringComparison.Ordinal))
            {
                throw new InvalidDataException($"The '{sourceId}' documented operation must contain a supported METHOD path.");
            }

            string operationKey = $"{method} {path}";
            if (!keys.Add(operationKey))
            {
                throw new InvalidDataException($"The '{sourceId}' contract contains duplicate operation '{operationKey}'.");
            }

            operations.Add(new ContractOperation(sourceId, operationKey, false));
        }

        return operations;
    }

    private static JsonDocument ParseJson(byte[] bytes, string errorMessage)
    {
        try
        {
            return JsonDocument.Parse(bytes);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(errorMessage, exception);
        }
    }

    private static string ResolveRelativePath(string rootDirectory, string relativePath)
    {
        string root = Path.GetFullPath(rootDirectory);
        string candidate = Path.GetFullPath(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        string prefix = root.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            ? root
            : root + Path.DirectorySeparatorChar;
        if (!candidate.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"Contract source path '{relativePath}' escapes the contract directory.");
        }

        return candidate;
    }

    private static string NormalizeRelativePath(string path)
    {
        if (Path.IsPathRooted(path) || path.Contains("..", StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Contract source path '{path}' must be a safe relative path.");
        }

        return path.Replace('\\', '/');
    }

    private static string GetRequiredString(JsonElement element, string propertyName, string context)
    {
        if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(propertyName, out JsonElement property) ||
            property.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new InvalidDataException($"The {context} property '{propertyName}' must be a non-empty string.");
        }

        return property.GetString() ?? throw new InvalidDataException($"The {context} property '{propertyName}' must be a string.");
    }

    private static string? GetOptionalString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property))
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new InvalidDataException($"Manifest property '{propertyName}' must be a non-empty string when present.");
        }

        return property.GetString();
    }

    private static int GetRequiredInt32(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property) || !property.TryGetInt32(out int value))
        {
            throw new InvalidDataException($"Property '{propertyName}' must be an integer.");
        }

        return value;
    }

    private static long GetRequiredInt64(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property) || !property.TryGetInt64(out long value) || value < 0)
        {
            throw new InvalidDataException($"Property '{propertyName}' must be a non-negative integer.");
        }

        return value;
    }

    internal static string CalculateSha256(byte[] bytes)
    {
        using SHA256 sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(bytes)).ToLowerInvariant();
    }

    private sealed record ExpectedSource(
        string Kind,
        string SnapshotPath,
        string ManifestPath,
        string SourceUrl,
        ContractReleaseEligibility ReleaseEligibility);

    private sealed record SourceRegistryEntry(
        string Id,
        string Kind,
        string SnapshotPath,
        string ManifestPath,
        ContractReleaseEligibility ReleaseEligibility);

    private sealed record ActiveTransaction(string SourceId, string CandidateDirectory, string CoveragePath);
}

internal sealed record ContractSource(
    string Id,
    string Kind,
    ContractReleaseEligibility ReleaseEligibility,
    byte[] SnapshotBytes,
    ContractManifest Manifest,
    IReadOnlyList<ContractOperation> Operations)
{
    internal string CalculateSha256() => ContractBaseline.CalculateSha256(SnapshotBytes);
}

internal sealed record ContractManifest(
    string SourceUrl,
    DateTimeOffset RetrievedAtUtc,
    string? OpenApiVersion,
    string? SourceKind,
    string? ReleaseEligibility,
    long ByteLength,
    string Sha256);

internal sealed record ContractOperation(string SourceId, string OperationKey, bool HasRequestBody)
{
    internal string Identity => $"{SourceId}:{OperationKey}";
}

internal enum ContractReleaseEligibility
{
    Eligible,
    RequiresLiveValidation
}
