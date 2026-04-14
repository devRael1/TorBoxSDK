namespace TorBoxSDK.Models.General;

/// <summary>Represents the root element of a changelogs RSS 2.0 feed.</summary>
public sealed record ChangelogsRssFeed
{
    /// <summary>Gets the RSS version.</summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>Gets the channel metadata and items, or <see langword="null"/> if not present.</summary>
    public ChangelogsRssChannel? Channel { get; init; }
}
