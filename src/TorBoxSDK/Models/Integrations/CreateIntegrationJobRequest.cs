using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents a request to create a cloud integration job (e.g., Google Drive, Dropbox).
/// </summary>
public sealed record CreateIntegrationJobRequest
{
	/// <summary>
	/// Gets the identifier of the download to integrate,
	/// or <see langword="null"/> if not applicable.
	/// </summary>
	[JsonPropertyName("id")]
	public long? DownloadId { get; init; }

	/// <summary>
	/// Gets the type of download to integrate (e.g., "torrent", "usenet"),
	/// or <see langword="null"/> if not applicable.
	/// </summary>
	[JsonPropertyName("type")]
	public string? DownloadType { get; init; }

	/// <summary>
	/// Gets the file identifier within the download to integrate,
	/// or <see langword="null"/> to integrate all files.
	/// </summary>
	[JsonPropertyName("file_id")]
	public long? FileId { get; init; }

	/// <summary>
	/// Gets a value indicating whether to zip the files before integration,
	/// or <see langword="null"/> to use the default behavior.
	/// </summary>
	[JsonPropertyName("zip")]
	public bool? Zip { get; init; }
}
