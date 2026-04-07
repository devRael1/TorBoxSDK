using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Represents the query parameters for searching the Newznab API.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record SearchNewznabOptions
{
    /// <summary>
    /// Gets the search query string.
    /// </summary>
    [JsonPropertyName("q")]
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional API key override for the Newznab endpoint,
    /// or <see langword="null"/> to use the default key.
    /// </summary>
    [JsonPropertyName("apikey")]
    public string? ApiKey { get; init; }
}
