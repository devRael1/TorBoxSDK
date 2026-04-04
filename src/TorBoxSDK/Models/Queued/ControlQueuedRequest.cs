using System.Text.Json.Serialization;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Models.Queued;

/// <summary>
/// Represents a request to perform a control operation on queued downloads.
/// </summary>
public sealed record ControlQueuedRequest
{
    /// <summary>
    /// Gets the identifier of the queued download to control,
    /// or <see langword="null"/> when <see cref="All"/> is <see langword="true"/>.
    /// </summary>
    [JsonPropertyName("queued_id")]
    public long? QueuedId { get; init; }

    /// <summary>
    /// Gets the control operation to perform on the queued download.
    /// </summary>
    [JsonPropertyName("operation")]
    public ControlOperation Operation { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation should apply to all queued downloads,
    /// or <see langword="null"/> to apply to a single queued download identified by <see cref="QueuedId"/>.
    /// </summary>
    [JsonPropertyName("all")]
    public bool? All { get; init; }
}
