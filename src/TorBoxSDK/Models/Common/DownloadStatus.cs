using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Enumerates the possible states of a download item.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DownloadStatus
{
    /// <summary>An unknown or unmapped download status was returned.</summary>
    Unknown = 0,

    /// <summary>The item is currently being downloaded.</summary>
    Downloading,

    /// <summary>The item is currently being uploaded (seeded).</summary>
    Uploading,

    /// <summary>The download is stalled with no active transfer.</summary>
    Stalled,

    /// <summary>The download has been paused by the user.</summary>
    Paused,

    /// <summary>The download has completed successfully.</summary>
    Completed,

    /// <summary>The item is cached and available for immediate download.</summary>
    Cached,

    /// <summary>The torrent metadata is being downloaded.</summary>
    Metadl,

    /// <summary>The downloaded data is being verified.</summary>
    Checkingdl,

    /// <summary>An error occurred during the download.</summary>
    Error,

    /// <summary>The item is queued and waiting to start.</summary>
    Queued,
}
