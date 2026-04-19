using System.Globalization;
using TorboxSDK.UnitTests.Helpers;
using TorBoxSDK.Main.General;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;

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

	private const string ChangelogsRssXml = """
        <?xml version="1.0" ?>
        <rss version="2.0" xmlns:atom="http://www.w3.org/2005/Atom">
          <channel>
            <title>TorBox Changelogs</title>
            <link>https://torbox.app/changelogs</link>
            <description>TorBox Changelog RSS Feed</description>
            <language>en-us</language>
            <lastBuildDate>Wed, 10 Jul 2024 07:12:36 GMT</lastBuildDate>
            <item>
              <title>v2.9 - Feature Release</title>
              <description>Major update with new features.</description>
              <pubDate>Wed, 10 Jul 2024 07:12:36 GMT</pubDate>
            </item>
          </channel>
        </rss>
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
                    "id": "1",
                    "name": "v8.4.3",
                    "html": "<p>Major update</p>",
                    "markdown": "**Major update**",
                    "link": "https://torbox.app/changelogs/v8.4.3",
                    "created_at": "2024-07-10T07:12:36Z"
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
		TorBoxResponse<UpStatus> result = await client.GetUpStatusAsync();

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
		(GeneralClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<GeneralClient>(ChangelogsRssXml);

		// Act
		TorBoxResponse<ChangelogsRssFeed> result = await client.GetChangelogsRssAsync();

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
		Assert.Contains("changelogs/rss", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
		Assert.NotNull(result.Data);
		Assert.NotNull(result.Data.Channel);
		Assert.Equal("TorBox Changelogs", result.Data.Channel.Title);
		Assert.NotEmpty(result.Data.Channel.Items);
		Assert.Equal("v2.9 - Feature Release", result.Data.Channel.Items[0].Title);
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
		Changelog first = result.Data[0];
		Assert.Equal("1", first.Id);
		Assert.Equal("v8.4.3", first.Name);
		Assert.Equal("<p>Major update</p>", first.Html);
		Assert.Equal("**Major update**", first.Markdown);
		Assert.Equal("https://torbox.app/changelogs/v8.4.3", first.Link);
		Assert.Equal(DateTimeOffset.Parse("2024-07-10T07:12:36Z", CultureInfo.InvariantCulture), first.CreatedAt);
	}
}
