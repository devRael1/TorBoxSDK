using System.Security.Cryptography;
using System.Text.Json;

namespace TorBoxSDK.V2.ContractTests.Infrastructure;

internal sealed class ContractBaseline
{
    private static readonly HashSet<string> HttpMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "get", "put", "post", "delete", "options", "head", "patch", "trace"
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
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (manifest is null)
        {
            throw new InvalidDataException("The contract manifest cannot be deserialized.");
        }

        using JsonDocument document = JsonDocument.Parse(snapshotBytes);
        IReadOnlySet<string> operationKeys = GetOperationKeys(document.RootElement);
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

    internal sealed record ContractManifest(
        string SourceUrl,
        DateTime RetrievedAtUtc,
        string OpenApiVersion,
        long ByteLength,
        string Sha256);
}
