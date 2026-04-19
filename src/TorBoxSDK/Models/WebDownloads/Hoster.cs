using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.WebDownloads;

/// <summary>
/// Represents a supported file hoster and its current bandwidth status.
/// </summary>
public sealed record Hoster
{
	/// <summary>
	/// Gets the unique identifier of the hoster.
	/// </summary>
	[JsonPropertyName("id")]
	public int Id { get; init; }

	/// <summary>
	/// Gets the name of the file hoster.
	/// </summary>
	[JsonPropertyName("name")]
	public string Name { get; init; } = string.Empty;

	/// <summary>
	/// Gets the list of domains associated with this hoster.
	/// </summary>
	[JsonPropertyName("domains")]
	public IReadOnlyList<string> Domains { get; init; } = [];

	/// <summary>
	/// Gets the URL of the hoster's website, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("url")]
	public string? Url { get; init; }

	/// <summary>
	/// Gets the URL of the hoster's icon image, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("icon")]
	public string? Icon { get; init; }

	/// <summary>
	/// Gets a value indicating whether the hoster is currently operational.
	/// </summary>
	[JsonPropertyName("status")]
	public bool Status { get; init; }

	/// <summary>
	/// Gets the type of hoster (e.g., "hoster"), or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("type")]
	public string? Type { get; init; }

	/// <summary>
	/// Gets any note about the hoster, or <see langword="null"/> if none.
	/// </summary>
	[JsonPropertyName("note")]
	public string? Note { get; init; }

	/// <summary>
	/// Gets a value indicating whether the hoster hosts NSFW content.
	/// </summary>
	[JsonPropertyName("nsfw")]
	public bool Nsfw { get; init; }

	/// <summary>
	/// Gets the daily link limit for this hoster.
	/// </summary>
	[JsonPropertyName("daily_link_limit")]
	public int DailyLinkLimit { get; init; }

	/// <summary>
	/// Gets the number of daily links already used for this hoster.
	/// </summary>
	[JsonPropertyName("daily_link_used")]
	public int DailyLinkUsed { get; init; }

	/// <summary>
	/// Gets the daily bandwidth limit in bytes for this hoster,
	/// or <see langword="null"/> if no limit is set.
	/// </summary>
	[JsonPropertyName("daily_bandwidth_limit")]
	public long? DailyBandwidthLimit { get; init; }

	/// <summary>
	/// Gets the amount of daily bandwidth already used in bytes for this hoster,
	/// or <see langword="null"/> if not tracked.
	/// </summary>
	[JsonPropertyName("daily_bandwidth_used")]
	public long? DailyBandwidthUsed { get; init; }

	/// <summary>
	/// Gets the per-link size limit in bytes for this hoster,
	/// or <see langword="null"/> if no limit is set.
	/// </summary>
	[JsonPropertyName("per_link_size_limit")]
	public long? PerLinkSizeLimit { get; init; }

	/// <summary>
	/// Gets the regex pattern used to match URLs for this hoster,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("regex")]
	public string? Regex { get; init; }
}
