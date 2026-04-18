using System;
using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>
/// Represents a changelog entry from TorBox service updates.
/// </summary>
public sealed record Changelog
{
    /// <summary>
    /// Gets the unique identifier of the changelog entry.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

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

    /// <summary>
    /// Gets the Markdown content of the changelog entry, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("markdown")]
    public string? Markdown { get; init; }

    /// <summary>
    /// Gets the link associated with the changelog entry, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("link")]
    public string? Link { get; init; }

    /// <summary>
    /// Gets the creation date of the changelog entry, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}
