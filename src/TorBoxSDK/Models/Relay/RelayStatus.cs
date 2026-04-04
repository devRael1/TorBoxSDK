using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Relay;

/// <summary>
/// Represents the status response from the TorBox Relay API.
/// </summary>
public sealed record RelayStatus
{
    /// <summary>
    /// Gets the current status of the relay service, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}
