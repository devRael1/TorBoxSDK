using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Vendors;

/// <summary>
/// Represents a request to register a new vendor with TorBox.
/// </summary>
public sealed record RegisterVendorRequest
{
	/// <summary>
	/// Gets the name of the vendor to register.
	/// </summary>
	[JsonPropertyName("vendor_name")]
	public string VendorName { get; init; } = string.Empty;

	/// <summary>
	/// Gets the URL of the vendor's website,
	/// or <see langword="null"/> if not provided.
	/// </summary>
	[JsonPropertyName("vendor_url")]
	public string? VendorUrl { get; init; }
}
