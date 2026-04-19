using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Usenet;

/// <summary>
/// Represents a request to edit the properties of an existing Usenet download.
/// </summary>
public sealed record EditUsenetDownloadRequest
{
	/// <summary>
	/// Gets the unique identifier of the Usenet download to edit.
	/// </summary>
	[JsonPropertyName("usenet_download_id")]
	public long UsenetDownloadId { get; init; }

	/// <summary>
	/// Gets the new name for the download,
	/// or <see langword="null"/> to leave the name unchanged.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>
	/// Gets the tags to apply to the download,
	/// or <see langword="null"/> to leave tags unchanged.
	/// </summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<string>? Tags { get; init; }

	/// <summary>
	/// Gets the alternative hashes for the download,
	/// or <see langword="null"/> to leave alternative hashes unchanged.
	/// </summary>
	[JsonPropertyName("alternative_hashes")]
	public IReadOnlyList<string>? AlternativeHashes { get; init; }
}
