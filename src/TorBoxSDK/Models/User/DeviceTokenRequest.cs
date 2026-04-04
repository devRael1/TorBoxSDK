using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to exchange a device code for an access token.
/// </summary>
public sealed record DeviceTokenRequest
{
    /// <summary>
    /// Gets the device code obtained from the device authorization flow.
    /// </summary>
    [JsonPropertyName("device_code")]
    public string DeviceCode { get; init; } = string.Empty;
}
