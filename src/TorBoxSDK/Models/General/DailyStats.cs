using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>
/// Represents a daily statistics snapshot of the TorBox platform.
/// </summary>
public sealed record DailyStats
{
	/// <summary>
	/// Gets the total number of downloads on the platform for this snapshot.
	/// </summary>
	[JsonPropertyName("total_downloads")]
	public long TotalDownloads { get; init; }

	/// <summary>
	/// Gets the total number of registered users at the time of this snapshot.
	/// </summary>
	[JsonPropertyName("total_users")]
	public long TotalUsers { get; init; }

	/// <summary>
	/// Gets the total bytes downloaded across the platform.
	/// </summary>
	[JsonPropertyName("total_bytes_downloaded")]
	public long TotalBytesDownloaded { get; init; }

	/// <summary>
	/// Gets the total bytes uploaded across the platform.
	/// </summary>
	[JsonPropertyName("total_bytes_uploaded")]
	public long TotalBytesUploaded { get; init; }

	/// <summary>
	/// Gets the total bytes egressed across the platform.
	/// </summary>
	[JsonPropertyName("total_bytes_egressed")]
	public long TotalBytesEgressed { get; init; }

	/// <summary>
	/// Gets the total number of servers available at the time of this snapshot.
	/// </summary>
	[JsonPropertyName("total_servers")]
	public int TotalServers { get; init; }

	/// <summary>
	/// Gets the date and time when this snapshot was created,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("created_at")]
	public DateTimeOffset? CreatedAt { get; init; }
}
