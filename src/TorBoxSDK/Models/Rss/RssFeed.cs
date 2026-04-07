using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Rss;

/// <summary>
/// Represents an RSS feed configured in TorBox.
/// </summary>
public sealed record RssFeed
{
    /// <summary>
    /// Gets the unique identifier of the RSS feed.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the display name of the RSS feed, or <see langword="null"/> if not set.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the URL of the RSS feed, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the scan interval in minutes for checking the feed for new items.
    /// </summary>
    [JsonPropertyName("scan_interval")]
    public int ScanInterval { get; init; }

    /// <summary>
    /// Gets the type of the RSS feed (e.g., torrent, usenet),
    /// or <see langword="null"/> if not specified.
    /// </summary>
    [JsonPropertyName("rss_type")]
    public string? RssType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the RSS feed is currently active.
    /// </summary>
    [JsonPropertyName("active")]
    public bool Active { get; init; }

    /// <summary>
    /// Gets the date and time when the RSS feed was created,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the regex pattern used to filter included items,
    /// or <see langword="null"/> if no include filter is set.
    /// </summary>
    [JsonPropertyName("do_regex")]
    public string? RegexFilter { get; init; }

    /// <summary>
    /// Gets the regex pattern used to exclude items,
    /// or <see langword="null"/> if no exclude filter is set.
    /// </summary>
    [JsonPropertyName("dont_regex")]
    public string? RegexFilterExclude { get; init; }
}
