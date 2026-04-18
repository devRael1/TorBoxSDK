using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents the response from an OAuth success endpoint.
/// </summary>
public sealed record OAuthSuccess
{
    /// <summary>
    /// Gets the success status or result, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets success-specific data, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("data")]
    public object? Data { get; init; }
}
