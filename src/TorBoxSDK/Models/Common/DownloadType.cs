using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Enumerates the supported download types in TorBox.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DownloadType
{
	/// <summary>An unknown or unmapped download type was returned.</summary>
	Unknown = 0,

	/// <summary>A torrent download.</summary>
	Torrent,

	/// <summary>A Usenet download.</summary>
	Usenet,

	/// <summary>A web (direct link / hoster) download.</summary>
	WebDownload,
}
