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
    [JsonPropertyName("regex_filter")]
    public string? RegexFilter { get; init; }

    /// <summary>
    /// Gets the new regex pattern to exclude items,
    /// or <see langword="null"/> to leave the exclude filter unchanged.
    /// </summary>
    [JsonPropertyName("regex_filter_exclude")]
    public string? RegexFilterExclude { get; init; }

    /// <summary>
    /// Gets the new scan interval in minutes,
    /// or <see langword="null"/> to leave the interval unchanged.
    /// </summary>
    [JsonPropertyName("scan_interval")]
    public int? ScanInterval { get; init; }
}
