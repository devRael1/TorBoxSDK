using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Vendors;

/// <summary>
/// Represents a vendor account registered with TorBox.
/// </summary>
public sealed record VendorAccount
{
	/// <summary>
	/// Gets the unique identifier of the vendor account.
	/// </summary>
	[JsonPropertyName("id")]
	public long Id { get; init; }

	/// <summary>
	/// Gets the name of the vendor, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("vendor_name")]
	public string? VendorName { get; init; }

	/// <summary>
	/// Gets the URL of the vendor's website, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("vendor_url")]
	public string? VendorUrl { get; init; }

	/// <summary>
	/// Gets the API key assigned to the vendor, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("api_key")]
	public string? ApiKey { get; init; }

	/// <summary>
	/// Gets the date and time when the vendor account was created,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("created_at")]
	public DateTimeOffset? CreatedAt { get; init; }
}
