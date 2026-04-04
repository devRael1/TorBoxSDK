using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Notifications;

/// <summary>
/// Represents a changelog entry from TorBox service updates.
/// </summary>
public sealed record Changelog
{
    /// <summary>
    /// Gets the unique identifier of the changelog entry.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the title of the changelog entry, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Gets the body content of the changelog entry, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; init; }

    /// <summary>
    /// Gets the date and time when the changelog entry was created,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}
