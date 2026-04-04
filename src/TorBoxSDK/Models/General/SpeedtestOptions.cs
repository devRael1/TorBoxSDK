using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>
/// Represents optional parameters for the speedtest files endpoint.
/// </summary>
public sealed record SpeedtestOptions
{
    /// <summary>
    /// Gets the user's IP address for the speedtest,
    /// or <see langword="null"/> to use the caller's IP.
    /// </summary>
    [JsonPropertyName("user_ip")]
    public string? UserIp { get; init; }

    /// <summary>
    /// Gets the region for the speedtest,
    /// or <see langword="null"/> to auto-detect.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; init; }

    /// <summary>
    /// Gets the test length in seconds,
    /// or <see langword="null"/> to use the default.
    /// </summary>
    [JsonPropertyName("test_length")]
    public int? TestLength { get; init; }
}
