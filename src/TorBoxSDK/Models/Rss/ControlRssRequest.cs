using System.Text.Json.Serialization;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Models.Rss;

/// <summary>
/// Represents a request to perform a control operation on one or more RSS feeds.
/// </summary>
public sealed record ControlRssRequest
{
    /// <summary>
    /// Gets the identifier of the RSS feed to control,
    /// or <see langword="null"/> when <see cref="All"/> is <see langword="true"/>.
    /// </summary>
    [JsonPropertyName("rss_feed_id")]
    public long? RssFeedId { get; init; }

    /// <summary>
    /// Gets the control operation to perform on the RSS feed.
    /// </summary>
    [JsonPropertyName("operation")]
    public ControlOperation Operation { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation should apply to all RSS feeds,
    /// or <see langword="null"/> to apply to a single feed identified by <see cref="RssFeedId"/>.
    /// </summary>
    [JsonPropertyName("all")]
    public bool? All { get; init; }
}
