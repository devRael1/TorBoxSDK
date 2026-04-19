using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Relay;

/// <summary>
/// Represents the result of an inactivity check for a torrent on the TorBox Relay API.
/// </summary>
public sealed record InactiveCheckResult
{
	/// <summary>
	/// Gets the status message of the inactivity check, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>
	/// Gets a value indicating whether the torrent is currently inactive.
	/// </summary>
	[JsonPropertyName("is_inactive")]
	public bool IsInactive { get; init; }

	/// <summary>
	/// Gets the date and time when the torrent was last active,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("last_active")]
	public DateTimeOffset? LastActive { get; init; }
}
