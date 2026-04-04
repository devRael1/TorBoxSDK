using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.WebDownloads;

/// <summary>
/// Represents a request to create a new web download.
/// </summary>
public sealed record CreateWebDownloadRequest
{
    /// <summary>
    /// Gets the URL of the file to download,
    /// or <see langword="null"/> if not provided.
    /// </summary>
    [JsonPropertyName("link")]
    public string? Link { get; init; }

    /// <summary>
    /// Gets the password for the download if the hoster requires one,
    /// or <see langword="null"/> if no password is needed.
    /// </summary>
    [JsonPropertyName("password")]
    public string? Password { get; init; }

    /// <summary>
    /// Gets an optional override name for the download,
    /// or <see langword="null"/> to use the default name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
