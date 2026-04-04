using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents the response from a device code authorization request.
/// </summary>
public sealed record DeviceCodeResponse
{
    /// <summary>
    /// Gets the device code used to poll for authorization,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("device_code")]
    public string? DeviceCode { get; init; }

    /// <summary>
    /// Gets the user-facing code to enter on the verification page,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("user_code")]
    public string? UserCode { get; init; }

    /// <summary>
    /// Gets the URL where the user should enter the code,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("verification_url")]
    public string? VerificationUrl { get; init; }

    /// <summary>
    /// Gets the number of seconds before the device code expires.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    /// <summary>
    /// Gets the polling interval in seconds for checking authorization status.
    /// </summary>
    [JsonPropertyName("interval")]
    public int Interval { get; init; }
}
