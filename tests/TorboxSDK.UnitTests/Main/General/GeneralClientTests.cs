using TorBoxSDK.Main.General;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;
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
                "total_users": 479317,
                "total_servers": 59
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

    private const string DailyStatsJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "total_downloads": 17639196,
                    "total_users": 431645,
                    "total_bytes_downloaded": 41726332351147853,
                    "total_bytes_uploaded": 41013999126964857,
                    "total_bytes_egressed": 322387277112545389,
                    "total_servers": 59,
                    "created_at": "2026-03-07T21:11:13Z"
                }
            ]
        }
        """;

    private const string SpeedtestJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "region": "ceur",
                    "name": "nexus-067",
                    "domain": "https://nexus-067.ceur.tb-cdn.st",
                    "path": "/dld/100MB.bin",
                    "url": "https://nexus-067.ceur.tb-cdn.st/dld/100MB.bin",
                    "closest": true,
                    "coordinates": {
                        "lat": 48.5833,
                        "lng": 7.75
                    }
                }
            ]
        }
        """;

    private const string ChangelogsJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "name": "v8.4.3",
                    "html": "Major update"
                }
            ]
        }
        """;

    // --- GetUpStatusAsync ---

    [Fact]
    public async Task GetUpStatusAsync_WithNoParameters_SendsGetRequest()
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
    public async Task GetStatsAsync_WithNoParameters_SendsGetRequest()
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
    public async Task Get30DayStatsAsync_WithNoParameters_SendsGetRequest()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(DailyStatsJson);

        // Act
        TorBoxResponse<IReadOnlyList<DailyStats>> result = await client.Get30DayStatsAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("stats/30days", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    // --- GetSpeedtestFilesAsync ---

    [Fact]
    public async Task GetSpeedtestFilesAsync_WithNoOptions_SendsGetRequest()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(SpeedtestJson);

        // Act
        TorBoxResponse<IReadOnlyList<SpeedtestServer>> result = await client.GetSpeedtestFilesAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("speedtest", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task GetSpeedtestFilesAsync_WithOptions_IncludesInQueryString()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(SpeedtestJson);
        SpeedtestOptions options = new() { UserIp = "1.2.3.4", Region = "us-east", TestLength = "short" };

        // Act
        await client.GetSpeedtestFilesAsync(options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("user_ip=1.2.3.4", url);
        Assert.Contains("region=us-east", url);
        Assert.Contains("test_length=short", url);
    }

    [Fact]
    public async Task GetSpeedtestFilesAsync_WithNullOptions_OmitsQueryParams()
    {
        // Arrange
        (GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(SpeedtestJson);

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
    public async Task GetChangelogsRssAsync_WithNoParameters_SendsGetRequest()
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
    public async Task GetChangelogsJsonAsync_WithNoParameters_SendsGetRequest()
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
