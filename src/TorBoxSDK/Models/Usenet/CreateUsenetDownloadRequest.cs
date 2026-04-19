using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Usenet;

/// <summary>
/// Represents a request to create a new Usenet download.
/// </summary>
/// <remarks>
/// Either <see cref="Link"/> or <see cref="File"/> must be provided.
/// The <see cref="File"/> property is excluded from JSON serialization because this
/// request is sent as multipart form data.
/// </remarks>
public sealed record CreateUsenetDownloadRequest
{
	/// <summary>
	/// Gets the NZB link for the Usenet download,
	/// or <see langword="null"/> if a file is provided instead.
	/// </summary>
	[JsonPropertyName("link")]
	public string? Link { get; init; }

	/// <summary>
	/// Gets the raw NZB file bytes, or <see langword="null"/> if a link is provided instead.
	/// </summary>
	/// <remarks>
	/// This property is excluded from JSON serialization and is handled separately
	/// as a file upload in multipart form data requests.
	/// </remarks>
	[JsonIgnore]
	public byte[]? File { get; init; }

	/// <summary>
	/// Gets an optional override name for the download,
	/// or <see langword="null"/> to use the default name.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>
	/// Gets the password for the Usenet archive,
	/// or <see langword="null"/> if no password is needed.
	/// </summary>
	[JsonPropertyName("password")]
	public string? Password { get; init; }

	/// <summary>
	/// Gets the post-processing flag for the download,
	/// or <see langword="null"/> to use the default behavior (defaults to -1).
	/// </summary>
	[JsonPropertyName("post_processing")]
	public int? PostProcessing { get; init; }

	/// <summary>
	/// Gets a value indicating whether to add the download as a queued download,
	/// or <see langword="null"/> to start immediately.
	/// </summary>
	[JsonPropertyName("as_queued")]
	public bool? AsQueued { get; init; }

	/// <summary>
	/// Gets a value indicating whether to only add the download if it is already cached,
	/// or <see langword="null"/> to use the default behavior.
	/// </summary>
	[JsonPropertyName("add_only_if_cached")]
	public bool? AddOnlyIfCached { get; init; }
}
