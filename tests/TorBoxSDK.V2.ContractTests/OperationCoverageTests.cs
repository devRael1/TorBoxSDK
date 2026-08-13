using TorBoxSDK.V2.ContractTests.Infrastructure;

namespace TorBoxSDK.V2.ContractTests;

public sealed class OperationCoverageTests
{
    [Fact]
    public void CoverageManifest_ContainsExactlyOneRecordForEverySourceOperation()
    {
        // Arrange
        ContractBaseline baseline = ContractBaseline.Load(ContractTestPaths.ContractDirectory);
        IReadOnlyList<CoverageRecord> coverage = CoverageManifest.Load(ContractTestPaths.CoveragePath).Records;

        // Act
        CoverageManifest.EnsureOperationSetMatches(coverage, baseline);

        // Assert
        Assert.Equal(102, coverage.Count);
        Assert.Equal(93, coverage.Count(static record => record.SourceId == "main"));
        Assert.Equal(2, coverage.Count(static record => record.SourceId == "relay"));
        Assert.Equal(7, coverage.Count(static record => record.SourceId == "search"));
    }

    [Fact]
    public void CoverageManifest_WithDuplicateSourceOperationIdentity_ThrowsInvalidDataException()
    {
        // Arrange
        string coveragePath = CreateTemporaryCoverageFile("""
            [
              { "sourceId": "main", "operationKey": "GET /v1/api/stats", "family": "Main", "resource": "General", "publicInterface": null, "publicMethod": null, "parameterTypes": [], "requestType": null, "resultType": null, "responseMode": "requires-validation", "implementationState": "Planned", "divergenceIds": [] },
              { "sourceId": "main", "operationKey": "GET /v1/api/stats", "family": "Main", "resource": "General", "publicInterface": null, "publicMethod": null, "parameterTypes": [], "requestType": null, "resultType": null, "responseMode": "requires-validation", "implementationState": "Planned", "divergenceIds": [] }
            ]
            """);

        // Act
        Action load = () => CoverageManifest.Load(coveragePath);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void CoverageManifest_WithSameOperationKeyForDifferentSources_DoesNotTreatThePairAsDuplicate()
    {
        // Arrange
        string coveragePath = CreateTemporaryCoverageFile("""
            [
              { "sourceId": "main", "operationKey": "GET /", "family": "Main", "resource": "General", "publicInterface": null, "publicMethod": null, "parameterTypes": [], "requestType": null, "resultType": null, "responseMode": "requires-validation", "implementationState": "Planned", "divergenceIds": [] },
              { "sourceId": "relay", "operationKey": "GET /", "family": "Relay", "resource": "Relay", "publicInterface": null, "publicMethod": null, "parameterTypes": [], "requestType": null, "resultType": null, "responseMode": "json", "implementationState": "Planned", "divergenceIds": [] }
            ]
            """);

        // Act
        CoverageManifest manifest = CoverageManifest.Load(coveragePath);

        // Assert
        Assert.Equal(2, manifest.Records.Count);
    }

    [Fact]
    public void CoverageManifest_WithUnknownSourceId_ThrowsInvalidDataException()
    {
        // Arrange
        string coveragePath = CreateTemporaryCoverageFile("""
            [
              { "sourceId": "other", "operationKey": "GET /", "family": "Main", "resource": "General", "publicInterface": null, "publicMethod": null, "parameterTypes": [], "requestType": null, "resultType": null, "responseMode": "requires-validation", "implementationState": "Planned", "divergenceIds": [] }
            ]
            """);

        // Act
        Action load = () => CoverageManifest.Load(coveragePath);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void CoverageManifest_WithMissingSourceOperation_ThrowsInvalidDataException()
    {
        // Arrange
        ContractBaseline baseline = ContractBaseline.Load(ContractTestPaths.ContractDirectory);
        IReadOnlyList<CoverageRecord> records = CoverageManifest.Load(ContractTestPaths.CoveragePath).Records.Skip(1).ToArray();

        // Act
        Action validate = () => CoverageManifest.EnsureOperationSetMatches(records, baseline);

        // Assert
        Assert.Throws<InvalidDataException>(validate);
    }

    [Fact]
    public void CoverageManifest_WithStaleSourceOperation_ThrowsInvalidDataException()
    {
        // Arrange
        ContractBaseline baseline = ContractBaseline.Load(ContractTestPaths.ContractDirectory);
        IReadOnlyList<CoverageRecord> records = CoverageManifest.Load(ContractTestPaths.CoveragePath).Records
            .Select(static record => record.Identity == "main:GET /" ? record with { OperationKey = "GET /v1/api/not-in-snapshot" } : record)
            .ToArray();

        // Act
        Action validate = () => CoverageManifest.EnsureOperationSetMatches(records, baseline);

        // Assert
        Assert.Throws<InvalidDataException>(validate);
    }

    [Fact]
    public void CoverageManifest_WithUnapprovedFamilyResourcePair_ThrowsInvalidDataException()
    {
        // Arrange
        string coveragePath = CreateTemporaryCoverageFile("""
            [
              { "sourceId": "main", "operationKey": "GET /v1/api/stats", "family": "Main", "resource": "Unassigned", "publicInterface": null, "publicMethod": null, "parameterTypes": [], "requestType": null, "resultType": null, "responseMode": "requires-validation", "implementationState": "Planned", "divergenceIds": [] }
            ]
            """);

        // Act
        Action load = () => CoverageManifest.Load(coveragePath);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Theory]
    [InlineData("relay", "Relay", "Relay", "stream")]
    [InlineData("relay", "Relay", "Relay", "redirect")]
    [InlineData("relay", "Relay", "Relay", "requires-validation")]
    [InlineData("search", "Search", "Search", "stream")]
    [InlineData("search", "Search", "Search", "redirect")]
    [InlineData("search", "Search", "Search", "requires-validation")]
    public void CoverageManifest_WhenSearchOrRelayResponseModeIsNotJson_ThrowsInvalidDataException(
        string sourceId,
        string family,
        string resource,
        string responseMode)
    {
        // Arrange
        string coveragePath = CreateTemporaryCoverageFile($$"""
            [
              { "sourceId": "{{sourceId}}", "operationKey": "GET /", "family": "{{family}}", "resource": "{{resource}}", "publicInterface": null, "publicMethod": null, "parameterTypes": [], "requestType": null, "resultType": null, "responseMode": "{{responseMode}}", "implementationState": "Planned", "divergenceIds": [] }
            ]
            """);

        // Act
        Action load = () => CoverageManifest.Load(coveragePath);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void CoverageManifest_WhenPlannedRecordDeclaresPublicMapping_ThrowsInvalidDataException()
    {
        // Arrange
        string coveragePath = CreateTemporaryCoverageFile("""
            [
              { "sourceId": "main", "operationKey": "GET /v1/api/stats", "family": "Main", "resource": "General", "publicInterface": "TorBoxSDK.Main.IGeneralResource", "publicMethod": null, "parameterTypes": [], "requestType": null, "resultType": null, "responseMode": "json", "implementationState": "Planned", "divergenceIds": [] }
            ]
            """);

        // Act
        Action load = () => CoverageManifest.Load(coveragePath);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void CoverageManifest_WhenImplementedRecordHasNoExactTaskResult_ThrowsInvalidDataException()
    {
        // Arrange
        string coveragePath = CreateTemporaryCoverageFile("""
            [
              { "sourceId": "main", "operationKey": "GET /v1/api/stats", "family": "Main", "resource": "General", "publicInterface": "TorBoxSDK.Main.IGeneralResource", "publicMethod": "GetStatsAsync", "parameterTypes": [], "requestType": null, "resultType": "System.String", "responseMode": "json", "implementationState": "Implemented", "divergenceIds": [] }
            ]
            """);

        // Act
        Action load = () => CoverageManifest.Load(coveragePath);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void CoverageManifest_WhenImplementedRecordUsesTaskLikeResultName_ThrowsInvalidDataException()
    {
        // Arrange
        string coveragePath = CreateTemporaryCoverageFile("""
            [
              { "sourceId": "main", "operationKey": "GET /v1/api/stats", "family": "Main", "resource": "General", "publicInterface": "TorBoxSDK.Main.IGeneralResource", "publicMethod": "GetStatsAsync", "parameterTypes": [], "requestType": null, "resultType": "System.Threading.Tasks.TaskFake", "responseMode": "json", "implementationState": "Implemented", "divergenceIds": [] }
            ]
            """);

        // Act
        Action load = () => CoverageManifest.Load(coveragePath);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void CoverageManifest_WithMalformedJson_ThrowsInvalidDataException()
    {
        // Arrange
        string coveragePath = CreateTemporaryCoverageFile("[");

        // Act
        Action load = () => CoverageManifest.Load(coveragePath);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void CoverageManifest_RecordsHaveRequiredShape()
    {
        // Arrange
        ContractBaseline baseline = ContractBaseline.Load(ContractTestPaths.ContractDirectory);
        IReadOnlyList<CoverageRecord> records = CoverageManifest.Load(ContractTestPaths.CoveragePath).Records;

        // Act
        Action validate = () => CoverageManifest.EnsureRecordShapesMatchSources(records, baseline);

        // Assert
        validate();
    }

    private static string CreateTemporaryCoverageFile(string content)
    {
        string directory = Path.Combine(Path.GetTempPath(), "TorBoxSDK.V2.ContractTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "coverage.json");
        File.WriteAllText(path, content);
        return path;
    }
}
