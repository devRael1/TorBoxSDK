namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

internal sealed class ContractBaseline(
	string rootDirectory,
	IReadOnlyDictionary<string, ContractSnapshotDescriptor> descriptors,
	IReadOnlyDictionary<string, ContractSnapshot> snapshots)
{
	internal string RootDirectory { get; } = rootDirectory;

	internal IReadOnlyDictionary<string, ContractSnapshotDescriptor> Descriptors { get; } = descriptors;

	internal IReadOnlyDictionary<string, ContractSnapshot> Snapshots { get; } = snapshots;

	internal ContractSnapshot GetSnapshot(string id)
	{
		if (!Snapshots.TryGetValue(id, out ContractSnapshot? snapshot))
		{
			throw new KeyNotFoundException($"Captured contract snapshot '{id}' was not found.");
		}

		return snapshot;
	}
}
