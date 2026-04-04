using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Vendors;

/// <summary>
/// Represents a request to update an existing vendor account.
/// </summary>
public sealed record UpdateVendorAccountRequest
{
    /// <summary>
    /// Gets the new name for the vendor,
    /// or <see langword="null"/> to leave the name unchanged.
    /// </summary>
    [JsonPropertyName("vendor_name")]
    public string? VendorName { get; init; }

    /// <summary>
    /// Gets the new URL for the vendor's website,
    /// or <see langword="null"/> to leave the URL unchanged.
    /// </summary>
    [JsonPropertyName("vendor_url")]
    public string? VendorUrl { get; init; }
}
