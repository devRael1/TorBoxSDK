namespace TorBoxSDK.Models.Notifications;

/// <summary>
/// Represents the TorBox notifications RSS feed.
/// </summary>
public sealed record NotificationRssFeed
{
	/// <summary>
	/// Gets the title of the RSS channel, or <see langword="null"/> if not available.
	/// </summary>
	public string? Title { get; init; }

	/// <summary>
	/// Gets the self-link URL of the RSS channel, or <see langword="null"/> if not available.
	/// </summary>
	public string? Link { get; init; }

	/// <summary>
	/// Gets the description of the RSS channel, or <see langword="null"/> if not available.
	/// </summary>
	public string? Description { get; init; }

	/// <summary>
	/// Gets the list of notification items in the feed.
	/// </summary>
	public IReadOnlyList<NotificationRssItem> Items { get; init; } = [];
}
