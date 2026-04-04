using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to modify the user's search engine configuration.
/// </summary>
public sealed record ModifySearchEnginesRequest
{
    /// <summary>
    /// Gets the list of search engine names to set.
    /// </summary>
    [JsonPropertyName("search_engines")]
    public IReadOnlyList<string> SearchEngines { get; init; } = [];
}
