using System.Text.Json;
using System.Text.Json.Nodes;
using TorBoxSDK.V2.ContractTests.Infrastructure;

namespace TorBoxSDK.V2.ContractTests;

public sealed class ContractSnapshotFileTests
{
    private static readonly string[] RequiredRelativePaths =
    [
        "sources.json",
        "baseline/openapi.json",
        "baseline/manifest.json",
        "relay/openapi.json",
        "relay/manifest.json",
        "search/operations.json",
        "search/manifest.json",
        "coverage.json",
        "divergences.json"
    ];

    [Fact]
    public void ContractArtifacts_WhenCopiedToTestOutput_AllExist()
    {
        // Arrange
        IReadOnlyList<string> artifactPaths = RequiredRelativePaths
            .Select(relativePath => Path.Combine(ContractTestPaths.ContractDirectory, relativePath))
            .ToArray();

        // Act
        IReadOnlyList<string> missingPaths = artifactPaths.Where(static path => !File.Exists(path)).ToArray();

        // Assert
        Assert.Empty(missingPaths);
    }

    [Fact]
    public void ContractBaseline_LoadsTheExactThreeSourceUnion()
    {
        // Arrange
        string contractDirectory = ContractTestPaths.ContractDirectory;

        // Act
        ContractBaseline baseline = ContractBaseline.Load(contractDirectory);

        // Assert
        Assert.Equal(["main", "relay", "search"], baseline.Sources.Select(static source => source.Id));
        Assert.Equal(102, baseline.Operations.Count);
        Assert.Equal(93, baseline.Operations.Count(static operation => operation.SourceId == "main"));
        Assert.Equal(2, baseline.Operations.Count(static operation => operation.SourceId == "relay"));
        Assert.Equal(7, baseline.Operations.Count(static operation => operation.SourceId == "search"));
        Assert.Equal(
            ["relay:GET /", "relay:GET /v1/inactivecheck/torrent/{auth_id}/{torrent_id}"],
            baseline.Operations.Where(static operation => operation.SourceId == "relay").Select(static operation => operation.Identity).OrderBy(static identity => identity, StringComparer.Ordinal));
        Assert.Equal(
            [
                "search:GET /meta/search/{query}",
                "search:GET /meta/{id_type}:{id}",
                "search:GET /search/{search_query}",
                "search:GET /torrents/search/{query}",
                "search:GET /torrents/{id_type}:{id}",
                "search:GET /usenet/search/{query}",
                "search:GET /usenet/{id_type}:{id}"
            ],
            baseline.Operations.Where(static operation => operation.SourceId == "search").Select(static operation => operation.Identity).OrderBy(static identity => identity, StringComparer.Ordinal));
    }

    [Fact]
    public void SourceRegistry_WhenKindIsUnknown_RejectsRegistry()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        JsonObject registry = ReadJsonObject(temporaryContract.SourcesPath);
        registry["sources"]!.AsArray()[1]!["kind"] = "unknown";
        WriteJson(temporaryContract.SourcesPath, registry);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.ContractDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void RelayManifest_WhenByteLengthChanges_RejectsManifest()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        string manifestPath = Path.Combine(temporaryContract.ContractDirectory, "relay", "manifest.json");
        JsonObject manifest = ReadJsonObject(manifestPath);
        manifest["byteLength"] = manifest["byteLength"]!.GetValue<long>() + 1;
        WriteJson(manifestPath, manifest);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.ContractDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void SearchManifest_WhenHashChanges_RejectsManifest()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        string manifestPath = Path.Combine(temporaryContract.ContractDirectory, "search", "manifest.json");
        JsonObject manifest = ReadJsonObject(manifestPath);
        manifest["sha256"] = new string('0', 64);
        WriteJson(manifestPath, manifest);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.ContractDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void SourceManifest_WhenRetrievedAtUtcIsNotUtc_RejectsManifest()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        string manifestPath = Path.Combine(temporaryContract.ContractDirectory, "search", "manifest.json");
        JsonObject manifest = ReadJsonObject(manifestPath);
        manifest["retrievedAtUtc"] = "2026-08-13T02:00:00+02:00";
        WriteJson(manifestPath, manifest);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.ContractDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void SourceManifest_WhenRetrievedAtUtcHasNoUtcMarker_RejectsManifest()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        string manifestPath = Path.Combine(temporaryContract.ContractDirectory, "baseline", "manifest.json");
        JsonObject manifest = ReadJsonObject(manifestPath);
        manifest["retrievedAtUtc"] = "2026-08-13T00:00:00";
        WriteJson(manifestPath, manifest);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.ContractDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void ContractTransaction_WhenRelayDurableFilesAreMixed_LoadsCompleteRelayCandidate()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        string expectedRelayHash = ContractBaseline.Load(temporaryContract.ContractDirectory)
            .Sources.Single(static source => source.Id == "relay").Manifest.Sha256;
        temporaryContract.CreateCandidateTransaction("relay");
        File.WriteAllText(Path.Combine(temporaryContract.ContractDirectory, "relay", "openapi.json"), "{}");

        // Act
        ContractBaseline baseline = ContractBaseline.Load(temporaryContract.ContractDirectory);

        // Assert
        Assert.Equal(expectedRelayHash, baseline.Sources.Single(static source => source.Id == "relay").CalculateSha256());
        Assert.Equal(102, baseline.Operations.Count);
    }

    [Fact]
    public void ContractTransaction_WhenSelectedCandidateIsIncomplete_RejectsTransaction()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        temporaryContract.CreateCandidateTransaction("relay");
        File.Delete(Path.Combine(temporaryContract.TransactionCandidateDirectory, "relay", "manifest.json"));

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.ContractDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void ContractTransaction_WhenJournalIsRetired_LoadsDurableSources()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        temporaryContract.CreateCandidateTransaction("relay");
        File.Move(temporaryContract.TransactionJournalPath, temporaryContract.RetiredTransactionJournalPath);
        Directory.Delete(temporaryContract.TransactionCandidateDirectory, recursive: true);

        // Act
        ContractBaseline baseline = ContractBaseline.Load(temporaryContract.ContractDirectory);

        // Assert
        Assert.Equal(102, baseline.Operations.Count);
    }

    private static JsonObject ReadJsonObject(string path)
    {
        JsonNode? node = JsonNode.Parse(File.ReadAllText(path));
        return node?.AsObject() ?? throw new InvalidDataException("The JSON fixture must be an object.");
    }

    private static void WriteJson(string path, JsonNode node)
    {
        File.WriteAllText(path, node.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    private sealed class TemporaryContractDirectory : IDisposable
    {
        private TemporaryContractDirectory(string rootDirectory, string contractDirectory)
        {
            RootDirectory = rootDirectory;
            ContractDirectory = contractDirectory;
        }

        internal string RootDirectory { get; }

        internal string ContractDirectory { get; }

        internal string SourcesPath => Path.Combine(ContractDirectory, "sources.json");

        internal string TransactionDirectory => Path.Combine(ContractDirectory, ".update-transaction");

        internal string TransactionCandidateDirectory => Path.Combine(TransactionDirectory, "candidate");

        internal string TransactionJournalPath => Path.Combine(TransactionDirectory, "journal.json");

        internal string RetiredTransactionJournalPath => Path.Combine(TransactionDirectory, "journal.retired.test.json");

        internal static TemporaryContractDirectory Create()
        {
            string rootDirectory = Path.Combine(Path.GetTempPath(), "TorBoxSDK.V2.ContractTests", Guid.NewGuid().ToString("N"));
            string contractDirectory = Path.Combine(rootDirectory, "contracts", "torbox");
            CopyDirectory(ContractTestPaths.ContractDirectory, contractDirectory);
            return new TemporaryContractDirectory(rootDirectory, contractDirectory);
        }

        internal void CreateCandidateTransaction(string sourceId)
        {
            string sourceDirectory = sourceId == "main" ? "baseline" : sourceId;
            string candidateSourceDirectory = Path.Combine(TransactionCandidateDirectory, sourceDirectory);
            Directory.CreateDirectory(candidateSourceDirectory);
            File.Copy(Path.Combine(ContractDirectory, sourceDirectory, "openapi.json"), Path.Combine(candidateSourceDirectory, "openapi.json"));
            File.Copy(Path.Combine(ContractDirectory, sourceDirectory, "manifest.json"), Path.Combine(candidateSourceDirectory, "manifest.json"));
            File.Copy(Path.Combine(ContractDirectory, "coverage.json"), Path.Combine(TransactionCandidateDirectory, "coverage.json"));

            JsonObject journal = new()
            {
                ["schemaVersion"] = 3,
                ["activeGeneration"] = "candidate",
                ["sourceId"] = sourceId,
                ["publishCoverage"] = false
            };
            WriteJson(TransactionJournalPath, journal);
        }

        public void Dispose()
        {
            Directory.Delete(RootDirectory, recursive: true);
        }

        private static void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            Directory.CreateDirectory(destinationDirectory);
            foreach (string filePath in Directory.GetFiles(sourceDirectory))
            {
                File.Copy(filePath, Path.Combine(destinationDirectory, Path.GetFileName(filePath)));
            }

            foreach (string childDirectory in Directory.GetDirectories(sourceDirectory))
            {
                if (string.Equals(Path.GetFileName(childDirectory), ".update-transaction", StringComparison.Ordinal))
                {
                    continue;
                }

                CopyDirectory(childDirectory, Path.Combine(destinationDirectory, Path.GetFileName(childDirectory)));
            }
        }
    }
}
