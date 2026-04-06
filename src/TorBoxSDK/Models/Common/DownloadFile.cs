using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents a file within a download item (torrent, usenet, or web download).
/// </summary>
public sealed record DownloadFile
{
    /// <summary>
    /// Gets the unique identifier of the file.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the MD5 hash of the file, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("md5")]
    public string? Md5 { get; init; }

    /// <summary>
    /// Gets the MIME type of the file, or <see langword="null"/> if not determined.
    /// </summary>
    [JsonPropertyName("mimetype")]
    public string? Mimetype { get; init; }

    /// <summary>
    /// Gets the full name (path) of the file within the download.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the S3 storage path of the file, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("s3_path")]
    public string? S3Path { get; init; }

    /// <summary>
    /// Gets the shortened display name of the file, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("short_name")]
    public string? ShortName { get; init; }

    /// <summary>
    /// Gets the size of the file in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; init; }

    /// <summary>
    /// Gets the torrent hash this file belongs to, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; init; }

    /// <summary>
    /// Gets a value indicating whether this file is zipped.
    /// </summary>
    [JsonPropertyName("zipped")]
    public bool Zipped { get; init; }

    /// <summary>
    /// Gets a value indicating whether this file has been flagged as infected.
    /// </summary>
    [JsonPropertyName("infected")]
    public bool Infected { get; init; }

    /// <summary>
    /// Gets the absolute path of the file on the server, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("absolute_path")]
    public string? AbsolutePath { get; init; }

    /// <summary>
    /// Gets the OpenSubtitles hash for the file, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("opensubtitles_hash")]
    public string? OpensubtitlesHash { get; init; }
}
