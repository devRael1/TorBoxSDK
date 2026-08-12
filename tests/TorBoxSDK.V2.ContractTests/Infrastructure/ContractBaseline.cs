using System.Security.Cryptography;
using System.Text.Json;

namespace TorBoxSDK.V2.ContractTests.Infrastructure;

internal sealed class ContractBaseline
{
    private const string OfficialSourceUrl = "https://api.torbox.app/openapi.json";

    private static readonly HashSet<string> HttpMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "get", "put", "post", "delete", "options", "head", "patch", "trace"
    };

    private static readonly JsonSerializerOptions ManifestSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private ContractBaseline(byte[] snapshotBytes, ContractManifest manifest, IReadOnlySet<string> operationKeys)
    {
        SnapshotBytes = snapshotBytes;
        Manifest = manifest;
        OperationKeys = operationKeys;
    }

    internal byte[] SnapshotBytes { get; }

    internal ContractManifest Manifest { get; }

    internal IReadOnlySet<string> OperationKeys { get; }

    internal static ContractBaseline Load(string baselineDirectory)
    {
        string snapshotPath = Path.Combine(baselineDirectory, "openapi.json");
        string manifestPath = Path.Combine(baselineDirectory, "manifest.json");

        if (!File.Exists(snapshotPath))
        {
            throw new FileNotFoundException("The contract snapshot is missing.", snapshotPath);
        }

        if (!File.Exists(manifestPath))
        {
            throw new FileNotFoundException("The contract manifest is missing.", manifestPath);
        }

        byte[] snapshotBytes = File.ReadAllBytes(snapshotPath);
        ContractManifest? manifest = JsonSerializer.Deserialize<ContractManifest>(
            File.ReadAllText(manifestPath),
            ManifestSerializerOptions);
        if (manifest is null)
        {
            throw new InvalidDataException("The contract manifest cannot be deserialized.");
        }

        using JsonDocument document = JsonDocument.Parse(snapshotBytes);
        IReadOnlySet<string> operationKeys = GetOperationKeys(document.RootElement);
        ValidateManifest(manifest, snapshotBytes, document.RootElement);
        ValidateCoverage(baselineDirectory, operationKeys);
        return new ContractBaseline(snapshotBytes, manifest, operationKeys);
    }

    internal string CalculateSha256()
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(SnapshotBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static IReadOnlySet<string> GetOperationKeys(JsonElement root)
    {
        if (!root.TryGetProperty("paths", out JsonElement paths) || paths.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("The OpenAPI document does not contain an object-valued paths property.");
        }

        HashSet<string> operationKeys = new(StringComparer.Ordinal);
        foreach (JsonProperty path in paths.EnumerateObject())
        {
            if (path.Value.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            foreach (JsonProperty operation in path.Value.EnumerateObject())
            {
                if (operation.Value.ValueKind == JsonValueKind.Object && HttpMethods.Contains(operation.Name))
                {
                    operationKeys.Add($"{operation.Name.ToUpperInvariant()} {path.Name}");
                }
            }
        }

        return operationKeys;
    }

    private static void ValidateManifest(ContractManifest manifest, byte[] snapshotBytes, JsonElement root)
    {
        if (!string.Equals(manifest.SourceUrl, OfficialSourceUrl, StringComparison.Ordinal))
        {
            throw new InvalidDataException("The contract manifest source URL is not the official TorBox OpenAPI URL.");
        }

        if (manifest.RetrievedAtUtc == default || manifest.RetrievedAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new InvalidDataException("The contract manifest retrieval timestamp must be a UTC value.");
        }

        if (string.IsNullOrWhiteSpace(manifest.OpenApiVersion))
        {
            throw new InvalidDataException("The contract manifest OpenAPI version is required.");
        }

        if (!root.TryGetProperty("info", out JsonElement info) ||
            !info.TryGetProperty("version", out JsonElement version) ||
            version.ValueKind != JsonValueKind.String ||
            !string.Equals(manifest.OpenApiVersion, version.GetString(), StringComparison.Ordinal))
        {
            throw new InvalidDataException("The contract manifest OpenAPI version does not match the snapshot.");
        }

        if (manifest.ByteLength != snapshotBytes.LongLength)
        {
            throw new InvalidDataException("The contract manifest byte length does not match the snapshot.");
        }

        if (!string.Equals(manifest.Sha256, CalculateSha256(snapshotBytes), StringComparison.Ordinal))
        {
            throw new InvalidDataException("The contract manifest SHA-256 does not match the snapshot.");
        }
    }

    private static void ValidateCoverage(string baselineDirectory, IReadOnlySet<string> operationKeys)
    {
        DirectoryInfo? baselineParent = Directory.GetParent(baselineDirectory);
        if (baselineParent is null)
        {
            throw new InvalidDataException("The contract baseline directory does not have a parent directory.");
        }

        string coveragePath = Path.Combine(baselineParent.FullName, "coverage.json");
        if (!File.Exists(coveragePath))
        {
            throw new FileNotFoundException("The contract coverage inventory is missing.", coveragePath);
        }

        using JsonDocument coverageDocument = JsonDocument.Parse(File.ReadAllBytes(coveragePath));
        if (coverageDocument.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException("The contract coverage inventory must be a JSON array.");
        }

        HashSet<string> coverageOperationKeys = new(StringComparer.Ordinal);
        foreach (JsonElement coverageRecord in coverageDocument.RootElement.EnumerateArray())
        {
            if (coverageRecord.ValueKind != JsonValueKind.Object ||
                !coverageRecord.TryGetProperty("operationKey", out JsonElement operationKey) ||
                operationKey.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(operationKey.GetString()))
            {
                throw new InvalidDataException("Every contract coverage record must have a non-empty operationKey.");
            }

            string key = operationKey.GetString() ?? throw new InvalidDataException("The contract coverage operationKey must be a string.");
            if (!coverageOperationKeys.Add(key))
            {
                throw new InvalidDataException($"The contract coverage inventory contains duplicate operationKey '{key}'.");
            }
        }

        if (!coverageOperationKeys.SetEquals(operationKeys))
        {
            throw new InvalidDataException("The contract coverage inventory must contain exactly the snapshot METHOD path operation keys.");
        }
    }

    private static string CalculateSha256(byte[] bytes)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    internal sealed record ContractManifest(
        string SourceUrl,
        DateTime RetrievedAtUtc,
        string OpenApiVersion,
        long ByteLength,
        string Sha256);
}
