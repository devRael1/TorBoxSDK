using System.Text;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

internal sealed class ContractSnapshot(
	ContractSnapshotDescriptor descriptor,
	string fullPath,
	byte[] content)
{
	internal ContractSnapshotDescriptor Descriptor { get; } = descriptor;

	internal string FullPath { get; } = fullPath;

	internal string ReadUtf8Text() => Encoding.UTF8.GetString(content);
}
