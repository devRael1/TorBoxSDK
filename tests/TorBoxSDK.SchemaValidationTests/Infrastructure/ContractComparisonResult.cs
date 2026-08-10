namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

internal sealed class ContractComparisonResult(IReadOnlyList<ContractDifference> differences)
{
	internal IReadOnlyList<ContractDifference> Differences { get; } = differences;

	internal bool IsEquivalent => Differences.Count == 0;
}
