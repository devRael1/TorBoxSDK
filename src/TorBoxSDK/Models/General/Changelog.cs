using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>
/// Represents a changelog entry from TorBox service updates.
/// </summary>
public sealed record Changelog
{
    /// <summary>
    /// Gets the version name of the changelog entry.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the HTML content of the changelog entry, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("html")]
    public string? Html { get; init; }
}
