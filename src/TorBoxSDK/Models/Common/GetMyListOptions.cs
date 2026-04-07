using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents the query parameters shared by list endpoints (torrents, usenet, web downloads).
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record GetMyListOptions
{
    /// <summary>
    /// Gets the optional item ID to retrieve a single item,
    /// or <see langword="null"/> to retrieve the full list.
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; init; }

    /// <summary>
    /// Gets the optional offset for pagination,
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; init; }

    /// <summary>
    /// Gets the optional limit for pagination,
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; init; }

    /// <summary>
    /// Gets a value indicating whether to bypass the cache,
    /// or <see langword="null"/> to use the default caching behavior.
    /// </summary>
    [JsonPropertyName("bypass_cache")]
    public bool? BypassCache { get; init; }
}
