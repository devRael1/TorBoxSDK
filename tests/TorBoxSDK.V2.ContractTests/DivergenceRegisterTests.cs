using TorBoxSDK.V2.ContractTests.Infrastructure;

namespace TorBoxSDK.V2.ContractTests;

public sealed class DivergenceRegisterTests
{
    [Fact]
    public void CoverageManifest_WithUnknownDivergenceId_ThrowsInvalidDataException()
    {
        // Arrange
        string coveragePath = CreateTemporaryFile("coverage.json", """
            [
              { "sourceId": "main", "operationKey": "GET /v1/api/stats", "family": "Main", "resource": "General", "publicInterface": null, "publicMethod": null, "parameterTypes": [], "requestType": null, "resultType": null, "responseMode": "requires-validation", "implementationState": "Planned", "divergenceIds": ["DIV-UNKNOWN"] }
            ]
            """);
        CoverageManifest coverage = CoverageManifest.Load(coveragePath);
        DivergenceRegister divergences = DivergenceRegister.Load(ContractTestPaths.DivergencesPath);

        // Act
        Action validate = () => coverage.EnsureDivergenceIdsAreRegistered(divergences);

        // Assert
        Assert.Throws<InvalidDataException>(validate);
    }

    [Fact]
    public void DivergenceRegister_WithDuplicateId_ThrowsInvalidDataException()
    {
        // Arrange
        const string record = """{ "id": "DIV-001", "operationKey": "GET /", "source": "Controlled live validation", "observedAtUtc": "2026-08-13T00:00:00Z", "contractBehavior": "Contract", "v2Behavior": "V2", "evidence": "fixture.json" }""";
        string path = CreateTemporaryFile("divergences.json", $"[{record},{record}]");

        // Act
        Action load = () => DivergenceRegister.Load(path);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void DivergenceRegister_WithNonUtcTimestamp_ThrowsInvalidDataException()
    {
        // Arrange
        string path = CreateTemporaryFile("divergences.json", """
            [{ "id": "DIV-001", "operationKey": "GET /", "source": "Controlled live validation", "observedAtUtc": "2026-08-13T02:00:00+02:00", "contractBehavior": "Contract", "v2Behavior": "V2", "evidence": "fixture.json" }]
            """);

        // Act
        Action load = () => DivergenceRegister.Load(path);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void DivergenceRegister_WithMalformedJson_ThrowsInvalidDataException()
    {
        // Arrange
        string path = CreateTemporaryFile("divergences.json", "[");

        // Act
        Action load = () => DivergenceRegister.Load(path);

        // Assert
        Assert.Throws<InvalidDataException>(load);
    }

    [Fact]
    public void CoverageManifest_ReferencesOnlyRegisteredDivergences()
    {
        // Arrange
        CoverageManifest coverage = CoverageManifest.Load(ContractTestPaths.CoveragePath);
        DivergenceRegister divergences = DivergenceRegister.Load(ContractTestPaths.DivergencesPath);

        // Act
        Action validate = () => coverage.EnsureDivergenceIdsAreRegistered(divergences);

        // Assert
        validate();
    }

    private static string CreateTemporaryFile(string fileName, string content)
    {
        string directory = Path.Combine(Path.GetTempPath(), "TorBoxSDK.V2.ContractTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, fileName);
        File.WriteAllText(path, content);
        return path;
    }
}
