using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents an integration job in TorBox (e.g., a cloud sync or import operation).
/// </summary>
public sealed record IntegrationJob
{
	/// <summary>
	/// Gets the unique identifier of the integration job.
	/// </summary>
	[JsonPropertyName("id")]
	public long Id { get; init; }

	/// <summary>
	/// Gets the authentication identifier of the user who owns this job,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("auth_id")]
	public string? AuthId { get; init; }

	/// <summary>
	/// Gets the type of integration job, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("job_type")]
	public string? JobType { get; init; }

	/// <summary>
	/// Gets the current status of the job, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>
	/// Gets the progress of the job as a value between 0.0 and 1.0.
	/// </summary>
	[JsonPropertyName("progress")]
	public double Progress { get; init; }

	/// <summary>
	/// Gets additional detail about the job, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("detail")]
	public string? Detail { get; init; }

	/// <summary>
	/// Gets the date and time when the job was created,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("created_at")]
	public DateTimeOffset? CreatedAt { get; init; }

	/// <summary>
	/// Gets the hash of the associated download, or <see langword="null"/> if not applicable.
	/// </summary>
	[JsonPropertyName("hash")]
	public string? Hash { get; init; }

	/// <summary>
	/// Gets the identifier of the associated download,
	/// or <see langword="null"/> if not applicable.
	/// </summary>
	[JsonPropertyName("download_id")]
	public long? DownloadId { get; init; }

	/// <summary>
	/// Gets the type of the associated download (e.g., torrent, usenet),
	/// or <see langword="null"/> if not applicable.
	/// </summary>
	[JsonPropertyName("download_type")]
	public string? DownloadType { get; init; }
}
