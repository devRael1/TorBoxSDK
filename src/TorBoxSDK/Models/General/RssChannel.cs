namespace TorBoxSDK.Models.General;

/// <summary>Represents the channel element of an RSS 2.0 feed.</summary>
public sealed record RssChannel
{
    /// <summary>Gets the channel title.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Gets the channel link.</summary>
    public string Link { get; init; } = string.Empty;

    /// <summary>Gets the channel description.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Gets the channel language.</summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>Gets the last build date as a string.</summary>
    public string LastBuildDate { get; init; } = string.Empty;

    /// <summary>Gets the list of RSS items.</summary>
    public IReadOnlyList<RssItem> Items { get; init; } = [];
}
