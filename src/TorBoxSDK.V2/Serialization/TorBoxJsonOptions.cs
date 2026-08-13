using System.Text.Json;

namespace TorBoxSDK.Serialization;

internal static class TorBoxJsonOptions
{
	internal static JsonSerializerOptions Default { get; } = new()
	{
		PropertyNameCaseInsensitive = true,
	};
}
