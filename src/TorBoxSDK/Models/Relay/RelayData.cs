using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Relay;

/// <summary>
/// Represents the data payload of the TorBox Relay status response.
/// </summary>
public sealed record RelayData
{
	/// <summary>
	/// Gets the number of currently online connections.
	/// </summary>
	[JsonPropertyName("current_online")]
	public int CurrentOnline { get; init; }

	/// <summary>
	/// Gets the number of currently active workers.
	/// </summary>
	[JsonPropertyName("current_workers")]
	public int CurrentWorkers { get; init; }
}
