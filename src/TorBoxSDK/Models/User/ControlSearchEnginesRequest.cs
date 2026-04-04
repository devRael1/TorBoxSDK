using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to control the user's search engine settings.
/// </summary>
public sealed record ControlSearchEnginesRequest
{
    /// <summary>
    /// Gets the operation to perform on the search engines,
    /// or <see langword="null"/> if not specified.
    /// </summary>
    [JsonPropertyName("operation")]
    public string? Operation { get; init; }

    /// <summary>
    /// Gets the list of search engine names to operate on.
    /// </summary>
    [JsonPropertyName("search_engines")]
    public IReadOnlyList<string> SearchEngines { get; init; } = [];
}
