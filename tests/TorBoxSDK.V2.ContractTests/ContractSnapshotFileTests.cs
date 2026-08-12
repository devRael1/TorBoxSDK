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
}
