namespace TorBoxSDK.Models.Notifications;

/// <summary>
/// Represents a single item in the TorBox notifications RSS feed.
/// </summary>
public sealed record NotificationRssItem
{
	/// <summary>
	/// Gets the title of the notification item, or <see langword="null"/> if not available.
	/// </summary>
	public string? Title { get; init; }

	/// <summary>
	/// Gets the description of the notification item, or <see langword="null"/> if not available.
	/// </summary>
	public string? Description { get; init; }

	/// <summary>
	/// Gets the unique identifier (GUID) of the notification item, or <see langword="null"/> if not available.
	/// </summary>
	public string? Guid { get; init; }

	/// <summary>
	/// Gets the publication date of the notification item, or <see langword="null"/> if not available.
	/// </summary>
	public DateTimeOffset? PubDate { get; init; }
}
