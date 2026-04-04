using System.Text.Json.Serialization;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Models.WebDownloads;

/// <summary>
/// Represents a request to perform a control operation on one or more web downloads.
/// </summary>
public sealed record ControlWebDownloadRequest
{
    /// <summary>
    /// Gets the identifier of the web download to control,
    /// or <see langword="null"/> when <see cref="All"/> is <see langword="true"/>.
    /// </summary>
    [JsonPropertyName("webdl_id")]
    public long? WebdlId { get; init; }

    /// <summary>
    /// Gets the control operation to perform on the download.
    /// </summary>
    [JsonPropertyName("operation")]
    public ControlOperation Operation { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation should apply to all web downloads,
    /// or <see langword="null"/> to apply to a single download identified by <see cref="WebdlId"/>.
    /// </summary>
    [JsonPropertyName("all")]
    public bool? All { get; init; }
}
