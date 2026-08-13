namespace TorBoxSDK.V2.ContractTests.Infrastructure;

internal static class ContractTestPaths
{
    internal static string ContractDirectory => Path.Combine(AppContext.BaseDirectory, "contracts", "torbox");

    internal static string BaselineDirectory => Path.Combine(ContractDirectory, "baseline");

    internal static string SourcesPath => Path.Combine(ContractDirectory, "sources.json");

    internal static string CoveragePath => Path.Combine(ContractDirectory, "coverage.json");

    internal static string DivergencesPath => Path.Combine(ContractDirectory, "divergences.json");
}
