using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Represents the parameters for downloading a usenet NZB file.
/// </summary>
/// <remarks>
/// These values are used as URL path segments for the download endpoint.
/// </remarks>
public sealed record DownloadUsenetOptions
{
    /// <summary>
    /// Gets the unique identifier of the usenet article.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the GUID of the NZB file to download.
    /// </summary>
    [JsonPropertyName("guid")]
    public string Guid { get; init; } = string.Empty;
}
