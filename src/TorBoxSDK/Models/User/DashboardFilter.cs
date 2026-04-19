using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents the dashboard filter configuration for a TorBox user.
/// </summary>
public sealed record DashboardFilter
{
	/// <summary>
	/// Gets a value indicating whether active items are shown,
	/// or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("active")]
	public bool? Active { get; init; }

	/// <summary>
	/// Gets a value indicating whether cached items are shown,
	/// or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("cached")]
	public bool? Cached { get; init; }

	/// <summary>
	/// Gets a value indicating whether queued items are shown,
	/// or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("queued")]
	public bool? Queued { get; init; }

	/// <summary>
	/// Gets a value indicating whether private items are shown,
	/// or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("private")]
	public bool? Private { get; init; }

	/// <summary>
	/// Gets a value indicating whether borrowed items are shown,
	/// or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("borrowed")]
	public bool? Borrowed { get; init; }
}
