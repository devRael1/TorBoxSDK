using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a search engine configured in the user's settings.
/// </summary>
public sealed record SearchEngine
{
    /// <summary>
    /// Gets the name of the search engine, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets a value indicating whether the search engine is currently enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }
}
