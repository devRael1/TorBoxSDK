using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>
/// Represents the API uptime status response from the TorBox root endpoint.
/// </summary>
public sealed record UpStatus
{
    /// <summary>
    /// Gets the status message, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the uptime data, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("uptime")]
    public object? Uptime { get; init; }
}
