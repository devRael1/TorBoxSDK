using System.Text.Json;
using System.Text.Json.Nodes;
using TorBoxSDK.V2.ContractTests.Infrastructure;

namespace TorBoxSDK.V2.ContractTests;

public sealed class ContractSnapshotFileTests
{
    [Fact]
    public void ContractArtifacts_WhenCopiedToTestOutput_AllExist()
    {
        // Arrange
        string snapshotPath = Path.Combine(ContractTestPaths.BaselineDirectory, "openapi.json");
        string manifestPath = Path.Combine(ContractTestPaths.BaselineDirectory, "manifest.json");

        // Act
        bool snapshotExists = File.Exists(snapshotPath);
        bool manifestExists = File.Exists(manifestPath);
        bool coverageExists = File.Exists(ContractTestPaths.CoveragePath);
        bool divergencesExist = File.Exists(ContractTestPaths.DivergencesPath);

        // Assert
        Assert.True(snapshotExists);
        Assert.True(manifestExists);
        Assert.True(coverageExists);
        Assert.True(divergencesExist);
    }

    [Fact]
    public void BaselineManifest_WhenSnapshotChanges_RejectsTheHashMismatch()
    {
        // Arrange
        ContractBaseline baseline = ContractBaseline.Load(ContractTestPaths.BaselineDirectory);

        // Act
        string actualSha256 = baseline.CalculateSha256();
        int actualByteLength = baseline.SnapshotBytes.Length;

        // Assert
        Assert.Equal(baseline.Manifest.Sha256, actualSha256);
        Assert.Equal(baseline.Manifest.ByteLength, actualByteLength);
    }

    [Fact]
    public void BaselineManifest_WhenSourceIsNotOfficial_RejectsManifest()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        JsonObject manifest = ReadJsonObject(temporaryContract.ManifestPath);
        manifest["sourceUrl"] = "https://example.invalid/openapi.json";
        WriteJson(temporaryContract.ManifestPath, manifest);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.BaselineDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void BaselineManifest_WhenRetrievedAtUtcIsNotUtc_RejectsManifest()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        JsonObject manifest = ReadJsonObject(temporaryContract.ManifestPath);
        manifest["retrievedAtUtc"] = "2026-08-12T21:27:10.4501753+02:00";
        WriteJson(temporaryContract.ManifestPath, manifest);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.BaselineDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void BaselineManifest_WhenRetrievedAtUtcHasNoUtcMarker_RejectsManifest()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        JsonObject manifest = ReadJsonObject(temporaryContract.ManifestPath);
        manifest["retrievedAtUtc"] = "2026-08-12T21:27:10.4501753";
        WriteJson(temporaryContract.ManifestPath, manifest);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.BaselineDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void BaselineManifest_WhenRequiredOpenApiVersionIsMissing_RejectsManifest()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        JsonObject manifest = ReadJsonObject(temporaryContract.ManifestPath);
        manifest.Remove("openApiVersion");
        WriteJson(temporaryContract.ManifestPath, manifest);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.BaselineDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void Coverage_WhenSnapshotOperationIsMissing_RejectsInventory()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        JsonArray coverage = ReadJsonArray(temporaryContract.CoveragePath);
        coverage.RemoveAt(0);
        WriteJson(temporaryContract.CoveragePath, coverage);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.BaselineDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void Coverage_WhenOperationKeyIsDuplicated_RejectsInventory()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        JsonArray coverage = ReadJsonArray(temporaryContract.CoveragePath);
        JsonNode? firstCoverageRecord = coverage[0];
        JsonNode? duplicateCoverageRecord = firstCoverageRecord is null
            ? null
            : JsonNode.Parse(firstCoverageRecord.ToJsonString());
        coverage.Add(duplicateCoverageRecord ?? throw new InvalidDataException("The copied coverage fixture is empty."));
        WriteJson(temporaryContract.CoveragePath, coverage);

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.BaselineDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void ContractTransaction_WhenDurableFilesAreMixed_LoadsCompleteCandidate()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        string expectedSha256 = ContractBaseline.Load(temporaryContract.BaselineDirectory).CalculateSha256();
        temporaryContract.CreateCandidateTransaction();
        File.WriteAllText(Path.Combine(temporaryContract.BaselineDirectory, "openapi.json"), "{}");

        // Act
        ContractBaseline baseline = ContractBaseline.Load(temporaryContract.BaselineDirectory);

        // Assert
        Assert.Equal(expectedSha256, baseline.CalculateSha256());
    }

    [Fact]
    public void ContractTransaction_WhenCandidateIsIncomplete_RejectsUnrecoverableTransaction()
    {
        // Arrange
        using TemporaryContractDirectory temporaryContract = TemporaryContractDirectory.Create();
        temporaryContract.CreateCandidateTransaction();
        File.Delete(Path.Combine(temporaryContract.TransactionCandidateDirectory, "coverage.json"));

        // Act
        Action load = () => ContractBaseline.Load(temporaryContract.BaselineDirectory);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    private static JsonObject ReadJsonObject(string path)
    {
        JsonNode? node = JsonNode.Parse(File.ReadAllText(path));
        return node?.AsObject() ?? throw new InvalidDataException("The JSON fixture must be an object.");
    }

    private static JsonArray ReadJsonArray(string path)
    {
        JsonNode? node = JsonNode.Parse(File.ReadAllText(path));
        return node?.AsArray() ?? throw new InvalidDataException("The JSON fixture must be an array.");
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
            BaselineDirectory = Path.Combine(contractDirectory, "baseline");
            ManifestPath = Path.Combine(BaselineDirectory, "manifest.json");
            CoveragePath = Path.Combine(contractDirectory, "coverage.json");
        }

        internal string RootDirectory { get; }

        internal string BaselineDirectory { get; }

        internal string ManifestPath { get; }

        internal string CoveragePath { get; }

        internal string TransactionCandidateDirectory => Path.Combine(RootDirectory, "contracts", "torbox", ".update-transaction", "candidate");

        internal static TemporaryContractDirectory Create()
        {
            string rootDirectory = Path.Combine(Path.GetTempPath(), "TorBoxSDK.V2.ContractTests", Guid.NewGuid().ToString("N"));
            string contractDirectory = Path.Combine(rootDirectory, "contracts", "torbox");
            string baselineDirectory = Path.Combine(contractDirectory, "baseline");
            Directory.CreateDirectory(baselineDirectory);

            File.Copy(Path.Combine(ContractTestPaths.BaselineDirectory, "openapi.json"), Path.Combine(baselineDirectory, "openapi.json"));
            File.Copy(Path.Combine(ContractTestPaths.BaselineDirectory, "manifest.json"), Path.Combine(baselineDirectory, "manifest.json"));
            File.Copy(ContractTestPaths.CoveragePath, Path.Combine(contractDirectory, "coverage.json"));

            return new TemporaryContractDirectory(rootDirectory, contractDirectory);
        }

        internal void CreateCandidateTransaction()
        {
            string transactionDirectory = Path.Combine(RootDirectory, "contracts", "torbox", ".update-transaction");
            string candidateBaselineDirectory = Path.Combine(TransactionCandidateDirectory, "baseline");
            Directory.CreateDirectory(candidateBaselineDirectory);

            File.Copy(Path.Combine(BaselineDirectory, "openapi.json"), Path.Combine(candidateBaselineDirectory, "openapi.json"));
            File.Copy(Path.Combine(BaselineDirectory, "manifest.json"), Path.Combine(candidateBaselineDirectory, "manifest.json"));
            File.Copy(CoveragePath, Path.Combine(TransactionCandidateDirectory, "coverage.json"));

            JsonObject journal = new()
            {
                ["activeGeneration"] = "candidate",
                ["publishCoverage"] = true
            };
            WriteJson(Path.Combine(transactionDirectory, "journal.json"), journal);
        }

        public void Dispose()
        {
            Directory.Delete(RootDirectory, recursive: true);
        }
    }
}
