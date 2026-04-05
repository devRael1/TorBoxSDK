using TorBoxSDK.Main.General;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;
using TorBoxSDK.Models.Notifications;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Main.General;

public sealed class GeneralClientTests
{
    private const string ObjectJson = """
        {
            "success": true,
            "error": null,
            "detail": "OK.",
            "data": {}
        }
        """;

    private const string StatsJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {
                "active_torrents": 5,
                "active_usenet": 2,
                "active_web_downloads": 1,
                "total_torrents_downloaded": 100,
                "total_usenet_downloaded": 50,
                "total_web_downloads_downloaded": 25,
                "total_bytes_downloaded": 107374182400,
                "total_bytes_uploaded": 53687091200
            }
        }
        """;

    private const string StringDataJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": "https://torbox.app/changelogs/rss"
        }
        """;

    private const string ChangelogsJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "title": "v2.0 Release",
                    "body": "Major update",
                    "created_at": "2024-01-01T00:00:00Z"
                }
            ]
        }
        """;

    // --- GetUpStatusAsync ---

    [Fact]
    public async Task GetUpStatusAsync_SendsGetRequest()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(ObjectJson);

        // Act
        TorBoxResponse<object> result = await client.GetUpStatusAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.True(result.Success);
    }

    // --- GetStatsAsync ---

    [Fact]
    public async Task GetStatsAsync_SendsGetRequest()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(StatsJson);

        // Act
        TorBoxResponse<Stats> result = await client.GetStatsAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("stats", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- Get30DayStatsAsync ---

    [Fact]
    public async Task Get30DayStatsAsync_SendsGetRequest()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(StatsJson);

        // Act
        TorBoxResponse<Stats> result = await client.Get30DayStatsAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("stats/30days", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetSpeedtestFilesAsync ---

    [Fact]
    public async Task GetSpeedtestFilesAsync_WithNoOptions_SendsGetRequest()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(ObjectJson);

        // Act
        await client.GetSpeedtestFilesAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("speedtest", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetSpeedtestFilesAsync_WithOptions_IncludesInQueryString()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(ObjectJson);
        SpeedtestOptions options = new() { UserIp = "1.2.3.4", Region = "us-east", TestLength = 10 };

        // Act
        await client.GetSpeedtestFilesAsync(options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("user_ip=1.2.3.4", url);
        Assert.Contains("region=us-east", url);
        Assert.Contains("test_length=10", url);
    }

    [Fact]
    public async Task GetSpeedtestFilesAsync_WithNullOptions_OmitsQueryParams()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(ObjectJson);

        // Act
        await client.GetSpeedtestFilesAsync(null);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.DoesNotContain("user_ip", url);
        Assert.DoesNotContain("region", url);
        Assert.DoesNotContain("test_length", url);
    }

    // --- GetChangelogsRssAsync ---

    [Fact]
    public async Task GetChangelogsRssAsync_SendsGetRequest()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(StringDataJson);

        // Act
        TorBoxResponse<string> result = await client.GetChangelogsRssAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("changelogs/rss", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetChangelogsJsonAsync ---

    [Fact]
    public async Task GetChangelogsJsonAsync_SendsGetRequest()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(ChangelogsJson);

        // Act
        TorBoxResponse<IReadOnlyList<Changelog>> result = await client.GetChangelogsJsonAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("changelogs/json", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }
}
