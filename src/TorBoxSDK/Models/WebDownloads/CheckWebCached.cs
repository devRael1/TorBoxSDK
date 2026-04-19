using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.WebDownloads;

/// <summary>
/// Represents the response from checking whether web downloads are cached on TorBox.
/// </summary>
public sealed record CheckWebCached
{
	/// <summary>
	/// Gets the cache status data, where each key is a hash and value indicates cache status.
	/// The value can be an integer (number of cached chunks) or a hash info object depending on format.
	/// </summary>
	[JsonExtensionData]
	public IDictionary<string, object?> Data { get; init; } = new Dictionary<string, object?>();
}
