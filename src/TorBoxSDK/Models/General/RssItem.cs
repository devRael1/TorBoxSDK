namespace TorBoxSDK.Models.General;

/// <summary>Represents a single item in an RSS 2.0 feed.</summary>
public sealed record RssItem
{
    /// <summary>Gets the item title.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Gets the item link.</summary>
    public string Link { get; init; } = string.Empty;

    /// <summary>Gets the item description.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Gets the publication date as a string.</summary>
    public string PubDate { get; init; } = string.Empty;

    /// <summary>Gets the content:encoded element value, or <see langword="null"/> if not present.</summary>
    public string? ContentEncoded { get; init; }
}
