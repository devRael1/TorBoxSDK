using TorBoxSDK.Models.Notifications;

namespace TorboxSDK.UnitTests.Models.Notifications;

public sealed class NotificationRssItemTests
{
	[Fact]
	public void NotificationRssItem_Create_WithAllProperties_PopulatesCorrectly()
	{
		// Arrange
		DateTimeOffset pubDate = new(2026, 4, 17, 12, 0, 0, TimeSpan.Zero);

		// Act
		NotificationRssItem item = new()
		{
			Title = "Download Complete",
			Description = "Your torrent has finished downloading.",
			Guid = "notif-12345-abcde",
			PubDate = pubDate,
		};

		// Assert
		Assert.Equal("Download Complete", item.Title);
		Assert.Equal("Your torrent has finished downloading.", item.Description);
		Assert.Equal("notif-12345-abcde", item.Guid);
		Assert.Equal(pubDate, item.PubDate);
	}

	[Fact]
	public void NotificationRssItem_Create_WithDefaults_HasNullProperties()
	{
		// Arrange & Act
		NotificationRssItem item = new();

		// Assert
		Assert.Null(item.Title);
		Assert.Null(item.Description);
		Assert.Null(item.Guid);
		Assert.Null(item.PubDate);
	}
}
