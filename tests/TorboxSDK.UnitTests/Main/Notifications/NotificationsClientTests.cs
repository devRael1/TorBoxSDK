using TorBoxSDK.Main.Notifications;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;
using TorboxSDK.UnitTests.Helpers;

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

    private const string StringDataJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": "https://torbox.app/notifications/rss/abc123"
        }
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
        (NotificationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<NotificationsClient>(StringDataJson);

        // Act
        TorBoxResponse<string> result = await client.GetNotificationRssAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("notifications/rss", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
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
}
