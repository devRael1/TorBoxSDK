namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

internal sealed record ContractSnapshotDescriptor(
	string Id,
	string Family,
	string Format,
	bool IsCaptured,
	string? ArtifactPath,
	string? SourceUrl,
	long? ContentLength,
	string? Sha256,
	string? Reason);
