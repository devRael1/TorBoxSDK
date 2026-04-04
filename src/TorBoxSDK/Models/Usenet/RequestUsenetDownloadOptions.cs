using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Usenet;

/// <summary>
/// Represents the query parameters for requesting a download link for a Usenet download.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record RequestUsenetDownloadOptions
{
    /// <summary>
    /// Gets the unique identifier of the Usenet download.
    /// </summary>
    [JsonPropertyName("usenet_id")]
    public long UsenetId { get; init; }

    /// <summary>
    /// Gets the identifier of a specific file within the download,
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
}
