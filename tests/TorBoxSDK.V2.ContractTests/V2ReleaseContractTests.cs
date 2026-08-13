using TorBoxSDK.V2.ContractTests.Infrastructure;

namespace TorBoxSDK.V2.ContractTests;

public sealed class V2ReleaseContractTests
{
    [SkippableFact]
    [Trait("Category", "Release")]
    public void AllOperationsAreImplemented()
    {
        // Arrange
        string? releaseContract = Environment.GetEnvironmentVariable("TORBOXSDK_V2_RELEASE_CONTRACT");
        Skip.If(!string.Equals(releaseContract, "1", StringComparison.Ordinal), "TORBOXSDK_V2_RELEASE_CONTRACT must equal 1 to run the V2 release contract.");
        ContractBaseline baseline = ContractBaseline.Load(ContractTestPaths.ContractDirectory);
        IReadOnlyList<CoverageRecord> records = CoverageManifest.Load(ContractTestPaths.CoveragePath).Records;

        // Act
        Action validateSurface = () => ContractSurfaceTests.ValidateImplementedMappings(records);

        // Assert
        Assert.DoesNotContain(baseline.Sources, static source => source.ReleaseEligibility != ContractReleaseEligibility.Eligible);
        Assert.DoesNotContain(records, static record => record.ImplementationState != CoverageImplementationState.Implemented);
        Assert.DoesNotContain(records, static record => record.ResponseMode == CoverageResponseMode.RequiresValidation);
        Assert.DoesNotContain(records, static record => string.IsNullOrWhiteSpace(record.PublicInterface) || string.IsNullOrWhiteSpace(record.PublicMethod) || string.IsNullOrWhiteSpace(record.ResultType));
        validateSurface();
    }
}
