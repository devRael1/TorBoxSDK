using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents the query parameters for requesting a download link for a torrent.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// The <see cref="JsonPropertyNameAttribute"/> attributes are provided for consistent
/// naming conventions with the API.
/// </remarks>
public sealed record RequestDownloadOptions
{
    /// <summary>
    /// Gets the unique identifier of the torrent to download.
    /// </summary>
    [JsonPropertyName("torrent_id")]
    public long TorrentId { get; init; }

    /// <summary>
    /// Gets the identifier of a specific file within the torrent to download,
    /// or <see langword="null"/> to download all files.
    /// </summary>
    [JsonPropertyName("file_id")]
    public long? FileId { get; init; }

    /// <summary>
    /// Gets a value indicating whether to return a zip download link,
    /// or <see langword="null"/> to use the default behavior.
    /// </summary>
    [JsonPropertyName("zip_link")]
    public bool? ZipLink { get; init; }

    /// <summary>
    /// Gets the IP address of the user requesting the download,
    /// or <see langword="null"/> to omit.
    /// </summary>
    [JsonPropertyName("user_ip")]
    public string? UserIp { get; init; }

    /// <summary>
    /// Gets a value indicating whether to redirect to the download URL,
    /// or <see langword="null"/> to return the URL in the response body.
    /// </summary>
    [JsonPropertyName("redirect")]
    public bool? Redirect { get; init; }
}
