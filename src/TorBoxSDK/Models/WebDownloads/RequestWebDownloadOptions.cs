using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.WebDownloads;

/// <summary>
/// Represents the query parameters for requesting a download link for a web download.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record RequestWebDownloadOptions
{
    /// <summary>
    /// Gets the unique identifier of the web download.
    /// </summary>
    [JsonPropertyName("web_id")]
    public long WebId { get; init; }

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

    /// <summary>
    /// Gets the API token to use for authentication,
    /// or <see langword="null"/> to use the client's default token.
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; init; }

    /// <summary>
    /// Gets a value indicating whether to redirect to the download URL,
    /// or <see langword="null"/> to return the URL in the response body.
    /// </summary>
    [JsonPropertyName("redirect")]
    public bool? Redirect { get; init; }

    /// <summary>
    /// Gets a value indicating whether to append the file name to the download URL,
    /// or <see langword="null"/> to use the default behavior.
    /// </summary>
    [JsonPropertyName("append_name")]
    public bool? AppendName { get; init; }
}
