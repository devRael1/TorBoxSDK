using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Rss;

/// <summary>
/// Represents an individual item from an RSS feed.
/// </summary>
public sealed record RssFeedItem
{
    /// <summary>
    /// Gets the unique identifier of the RSS feed item.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the identifier of the parent RSS feed.
    /// </summary>
    [JsonPropertyName("rss_feed_id")]
    public long RssFeedId { get; init; }

    /// <summary>
    /// Gets the title of the RSS feed item, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Gets the link URL of the RSS feed item, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("link")]
    public string? Link { get; init; }

    /// <summary>
    /// Gets the date and time when the item was published,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("published_at")]
    public DateTimeOffset? PublishedAt { get; init; }
}
