using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Enumerates the control operations that can be performed on a download item.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ControlOperation
{
    /// <summary>An unknown or unmapped control operation.</summary>
    Unknown = 0,

    /// <summary>Reannounce the item to trackers.</summary>
    Reannounce,

    /// <summary>Delete the item.</summary>
    Delete,

    /// <summary>Resume a paused item.</summary>
    Resume,

    /// <summary>Pause an active item.</summary>
    Pause,

    /// <summary>Recheck the item's data integrity.</summary>
    Recheck,
}
