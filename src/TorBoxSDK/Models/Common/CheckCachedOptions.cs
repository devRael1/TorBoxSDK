using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents the query parameters for checking whether hashes are cached (GET endpoint).
/// Shared by torrents, usenet, and web download cache check endpoints.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record CheckCachedOptions
{
    /// <summary>
    /// Gets the list of hashes to check for cache availability.
    /// </summary>
    [JsonPropertyName("hash")]
    public IReadOnlyList<string> Hashes { get; init; } = [];

    /// <summary>
    /// Gets the optional response format,
    /// or <see langword="null"/> to use the default format.
    /// </summary>
    [JsonPropertyName("format")]
    public string? Format { get; init; }

    /// <summary>
    /// Gets a value indicating whether to include file listings in the response,
    /// or <see langword="null"/> to use the default behavior.
    /// </summary>
    [JsonPropertyName("list_files")]
    public bool? ListFiles { get; init; }
}
