using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to add search engines to the user's settings.
/// </summary>
public sealed record AddSearchEnginesRequest
{
    /// <summary>
    /// Gets the list of search engine names to add.
    /// </summary>
    [JsonPropertyName("search_engines")]
    public IReadOnlyList<string> SearchEngines { get; init; } = [];
}
