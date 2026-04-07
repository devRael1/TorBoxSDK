using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>
/// Represents a speedtest server returned by the TorBox speedtest endpoint.
/// </summary>
public sealed record SpeedtestServer
{
    /// <summary>
    /// Gets the server's region identifier,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; init; }

    /// <summary>
    /// Gets the server's name,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the server's base domain URL,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; init; }

    /// <summary>
    /// Gets the path to the test file on the server,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("path")]
    public string? Path { get; init; }

    /// <summary>
    /// Gets the full URL to the speedtest file,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is the closest server to the requester.
    /// </summary>
    [JsonPropertyName("closest")]
    public bool Closest { get; init; }

    /// <summary>
    /// Gets the geographic coordinates of the server,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("coordinates")]
    public ServerCoordinates? Coordinates { get; init; }
}
