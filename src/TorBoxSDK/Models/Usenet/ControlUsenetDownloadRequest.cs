using System.Text.Json.Serialization;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Models.Usenet;

/// <summary>
/// Represents a request to perform a control operation on one or more Usenet downloads.
/// </summary>
public sealed record ControlUsenetDownloadRequest
{
    /// <summary>
    /// Gets the identifier of the Usenet download to control,
    /// or <see langword="null"/> when <see cref="All"/> is <see langword="true"/>.
    /// </summary>
    [JsonPropertyName("usenet_id")]
    public long? UsenetId { get; init; }

    /// <summary>
    /// Gets the control operation to perform on the download.
    /// </summary>
    [JsonPropertyName("operation")]
    public ControlOperation Operation { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation should apply to all Usenet downloads,
    /// or <see langword="null"/> to apply to a single download identified by <see cref="UsenetId"/>.
    /// </summary>
    [JsonPropertyName("all")]
    public bool? All { get; init; }
}
