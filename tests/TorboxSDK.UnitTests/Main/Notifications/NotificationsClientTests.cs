using TorboxSDK.UnitTests.Helpers;
using TorBoxSDK.Main.Notifications;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;

namespace TorboxSDK.UnitTests.Main.Notifications;

public sealed class NotificationsClientTests
{
	private const string SuccessJson = """
        {
            "success": true,
            "error": null,
            "detail": "OK."
        }
        """;

	private const string NotificationRssXml = """
        <?xml version="1.0" ?>
        <rss version="2.0" xmlns:atom="http://www.w3.org/2005/Atom">
          <channel>
            <title>TorBox Notifications</title>
            <link>https://api.torbox.app/v1/api/notifications/rss?token=test-token</link>
            <description>TorBox Notifications</description>
            <item>
              <title>Torrent Ready To Download</title>
              <description>Your torrent Test.torrent has finished processing.</description>
              <guid>12345</guid>
              <pubDate>2026-04-07T11:35:58Z</pubDate>
            </item>
          </channel>
        </rss>
        """;

	private const string NotificationsJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "auth_id": "96f722d3-e0e6-41a9-8ed4-3ad7b93582b7",
                    "created_at": "2024-01-01T00:00:00Z",
                    "title": "Download Complete",
                    "message": "Your download has finished.",
                    "action": "url",
                    "action_data": "https://torbox.app/download?id=123&type=torrents",
                    "action_cta": "Download Now"
                }
            ]
        }
        """;

	private const string IntercomHashJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {
                "hash": "abc123def456"
            }
        }
        """;

	// --- GetNotificationRssAsync ---

	[Fact]
	public async Task GetNotificationRssAsync_WithNoParameters_SendsGetRequest()
	{
		// Arrange
		(NotificationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<NotificationsClient>(NotificationRssXml);

		// Act
		TorBoxResponse<NotificationRssFeed> result = await client.GetNotificationRssAsync();

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
		Assert.Contains("notifications/rss", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
		Assert.NotNull(result.Data);
		Assert.Equal("TorBox Notifications", result.Data.Title);
		Assert.NotEmpty(result.Data.Items);
		Assert.Equal("Torrent Ready To Download", result.Data.Items[0].Title);
	}

	// --- GetMyNotificationsAsync ---

	[Fact]
	public async Task GetMyNotificationsAsync_WithNoParameters_SendsGetRequest()
	{
		// Arrange
		(NotificationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<NotificationsClient>(NotificationsJson);

		// Act
		TorBoxResponse<IReadOnlyList<Notification>> result = await client.GetMyNotificationsAsync();

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
		Assert.Contains("notifications/mynotifications", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
		Assert.NotNull(result.Data);
		Assert.NotEmpty(result.Data);
	}

	// --- ClearAllNotificationsAsync ---

	[Fact]
	public async Task ClearAllNotificationsAsync_WithNoParameters_SendsPostRequest()
	{
		// Arrange
		(NotificationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<NotificationsClient>(SuccessJson);

		// Act
		TorBoxResponse result = await client.ClearAllNotificationsAsync();

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("notifications/clear", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
	}

	// --- ClearNotificationAsync ---

	[Fact]
	public async Task ClearNotificationAsync_WithNotificationId_SendsPostRequest()
	{
		// Arrange
		(NotificationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<NotificationsClient>(SuccessJson);

		// Act
		TorBoxResponse result = await client.ClearNotificationAsync(42);

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("notifications/clear/42", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
	}

	// --- SendTestNotificationAsync ---

	[Fact]
	public async Task SendTestNotificationAsync_WithNoParameters_SendsPostRequest()
	{
		// Arrange
		(NotificationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<NotificationsClient>(SuccessJson);

		// Act
		TorBoxResponse result = await client.SendTestNotificationAsync();

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("notifications/test", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
	}

	// --- GetIntercomHashAsync ---

	[Fact]
	public async Task GetIntercomHashAsync_WithAuthIdAndEmail_SendsGetRequest()
	{
		// Arrange
		(NotificationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<NotificationsClient>(IntercomHashJson);

		// Act
		TorBoxResponse<IntercomHash> result = await client.GetIntercomHashAsync("auth-id", "test@test.com");

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
		Assert.Contains("intercom/hash", handler.LastRequest.RequestUri!.ToString());
		Assert.Contains("auth_id=auth-id", handler.LastRequest.RequestUri!.ToString());
		Assert.Contains("email=test%40test.com", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
	}

	[Fact]
	public async Task GetIntercomHashAsync_WithNullAuthId_ThrowsArgumentNullException()
	{
		// Arrange
		(NotificationsClient client, _) = ClientTestBase.CreateClient<NotificationsClient>(IntercomHashJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetIntercomHashAsync(null!, "test@test.com"));
	}

	[Fact]
	public async Task GetIntercomHashAsync_WithEmptyAuthId_ThrowsArgumentException()
	{
		// Arrange
		(NotificationsClient client, _) = ClientTestBase.CreateClient<NotificationsClient>(IntercomHashJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.GetIntercomHashAsync(string.Empty, "test@test.com"));
	}

	[Fact]
	public async Task GetIntercomHashAsync_WithEmptyEmail_ThrowsArgumentException()
	{
		// Arrange
		(NotificationsClient client, _) = ClientTestBase.CreateClient<NotificationsClient>(IntercomHashJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.GetIntercomHashAsync("auth-id", string.Empty));
	}
}
