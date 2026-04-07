using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Rss;

/// <summary>
/// Represents a request to modify the properties of an existing RSS feed.
/// </summary>
public sealed record ModifyRssRequest
{
    /// <summary>
    /// Gets the unique identifier of the RSS feed to modify.
    /// </summary>
    [JsonPropertyName("rss_feed_id")]
    public long RssFeedId { get; init; }

    /// <summary>
    /// Gets the new display name for the RSS feed,
    /// or <see langword="null"/> to leave the name unchanged.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the new regex pattern to filter included items,
    /// or <see langword="null"/> to leave the filter unchanged.
    /// </summary>
    [JsonPropertyName("do_regex")]
    public string? DoRegex { get; init; }

    /// <summary>
    /// Gets the new regex pattern to exclude items,
    /// or <see langword="null"/> to leave the exclude filter unchanged.
    /// </summary>
    [JsonPropertyName("dont_regex")]
    public string? DontRegex { get; init; }

    /// <summary>
    /// Gets the new scan interval in minutes,
    /// or <see langword="null"/> to leave the interval unchanged.
    /// </summary>
    [JsonPropertyName("scan_interval")]
    public int? ScanInterval { get; init; }

    /// <summary>
    /// Gets the maximum age in days for items to be included,
    /// or <see langword="null"/> to leave the value unchanged.
    /// </summary>
    [JsonPropertyName("dont_older_than")]
    public int? DontOlderThan { get; init; }

    /// <summary>
    /// Gets the RSS feed type (e.g., "torrent"),
    /// or <see langword="null"/> to leave the value unchanged.
    /// </summary>
    [JsonPropertyName("rss_type")]
    public string? RssType { get; init; }

    /// <summary>
    /// Gets the torrent seeding preference for items from this feed,
    /// or <see langword="null"/> to leave the value unchanged.
    /// </summary>
    [JsonPropertyName("torrent_seeding")]
    public int? TorrentSeeding { get; init; }
}
