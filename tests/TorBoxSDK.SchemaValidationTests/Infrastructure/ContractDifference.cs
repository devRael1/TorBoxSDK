namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

internal sealed record ContractDifference(
	ContractDifferenceKind Kind,
	string Category,
	string Subject,
	string? BaselineValue,
	string? CandidateValue);
