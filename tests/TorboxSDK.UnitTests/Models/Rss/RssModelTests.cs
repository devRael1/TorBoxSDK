using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Rss;

namespace TorboxSDK.UnitTests.Models.Rss;

public sealed class RssModelTests
{
	// ──── RssFeed ────

	[Fact]
	public void RssFeed_Deserialize_PopulatesAllProperties()
	{
		// Arrange
		string json = """
            {
                "id": 15,
                "name": "Ubuntu Releases",
                "url": "https://ubuntu.com/rss.xml",
                "scan_interval": 60,
                "rss_type": "torrent",
                "active": true,
                "created_at": "2025-01-01T00:00:00Z",
                "do_regex": "^ubuntu.*\\.iso$",
                "dont_regex": "\\.exe$"
            }
            """;

		// Act
		RssFeed? result = JsonSerializer.Deserialize<RssFeed>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(15L, result.Id);
		Assert.Equal("Ubuntu Releases", result.Name);
		Assert.Equal("https://ubuntu.com/rss.xml", result.Url);
		Assert.Equal(60, result.ScanInterval);
		Assert.Equal("torrent", result.RssType);
		Assert.True(result.Active);
		Assert.NotNull(result.CreatedAt);
		Assert.Equal(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), result.CreatedAt);
		Assert.Equal("^ubuntu.*\\.iso$", result.RegexFilter);
		Assert.Equal("\\.exe$", result.RegexFilterExclude);
	}

	[Fact]
	public void RssFeed_Deserialize_WithNullOptionals_ReturnsNulls()
	{
		// Arrange
		string json = """
            {
                "id": 1,
                "scan_interval": 30,
                "active": false
            }
            """;

		// Act
		RssFeed? result = JsonSerializer.Deserialize<RssFeed>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(1L, result.Id);
		Assert.Equal(30, result.ScanInterval);
		Assert.False(result.Active);
		Assert.Null(result.Name);
		Assert.Null(result.Url);
		Assert.Null(result.RssType);
		Assert.Null(result.CreatedAt);
		Assert.Null(result.RegexFilter);
		Assert.Null(result.RegexFilterExclude);
	}

	// ──── RssFeedItem ────

	[Fact]
	public void RssFeedItem_Deserialize_PopulatesAllProperties()
	{
		// Arrange
		string json = """
            {
                "id": 500,
                "rss_feed_id": 15,
                "title": "Ubuntu 24.04 LTS Desktop",
                "link": "https://ubuntu.com/releases/24.04/ubuntu-24.04-desktop-amd64.iso.torrent",
                "published_at": "2025-04-17T12:00:00Z"
            }
            """;

		// Act
		RssFeedItem? result = JsonSerializer.Deserialize<RssFeedItem>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(500L, result.Id);
		Assert.Equal(15L, result.RssFeedId);
		Assert.Equal("Ubuntu 24.04 LTS Desktop", result.Title);
		Assert.Equal("https://ubuntu.com/releases/24.04/ubuntu-24.04-desktop-amd64.iso.torrent", result.Link);
		Assert.NotNull(result.PublishedAt);
		Assert.Equal(new DateTimeOffset(2025, 4, 17, 12, 0, 0, TimeSpan.Zero), result.PublishedAt);
	}

	[Fact]
	public void RssFeedItem_Deserialize_WithNullOptionals_ReturnsNulls()
	{
		// Arrange
		string json = """
            {
                "id": 1,
                "rss_feed_id": 2
            }
            """;

		// Act
		RssFeedItem? result = JsonSerializer.Deserialize<RssFeedItem>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(1L, result.Id);
		Assert.Equal(2L, result.RssFeedId);
		Assert.Null(result.Title);
		Assert.Null(result.Link);
		Assert.Null(result.PublishedAt);
	}

	// ──── ControlRssRequest ────

	[Fact]
	public void ControlRssRequest_Serialize_WithAllProperties_ProducesExpectedJson()
	{
		// Arrange
		ControlRssRequest request = new()
		{
			RssFeedId = 15,
			Operation = ControlOperation.Delete,
			All = false,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal(15, root.GetProperty("rss_feed_id").GetInt64());
		Assert.True(root.TryGetProperty("operation", out _));
		Assert.False(root.GetProperty("all").GetBoolean());
	}

	[Fact]
	public void ControlRssRequest_Serialize_WithNullOptionals_OmitsNullProperties()
	{
		// Arrange
		ControlRssRequest request = new()
		{
			Operation = ControlOperation.Pause,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.False(root.TryGetProperty("rss_feed_id", out _));
		Assert.False(root.TryGetProperty("all", out _));
		Assert.True(root.TryGetProperty("operation", out _));
	}

	// ──── ModifyRssRequest ────

	[Fact]
	public void ModifyRssRequest_Serialize_WithAllProperties_ProducesExpectedJson()
	{
		// Arrange
		ModifyRssRequest request = new()
		{
			RssFeedId = 15,
			Name = "Updated Feed Name",
			DoRegex = "^include.*",
			DontRegex = "^exclude.*",
			ScanInterval = 120,
			DontOlderThan = 7,
			RssType = "torrent",
			TorrentSeeding = 1,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal(15, root.GetProperty("rss_feed_id").GetInt64());
		Assert.Equal("Updated Feed Name", root.GetProperty("name").GetString());
		Assert.Equal("^include.*", root.GetProperty("do_regex").GetString());
		Assert.Equal("^exclude.*", root.GetProperty("dont_regex").GetString());
		Assert.Equal(120, root.GetProperty("scan_interval").GetInt32());
		Assert.Equal(7, root.GetProperty("dont_older_than").GetInt32());
		Assert.Equal("torrent", root.GetProperty("rss_type").GetString());
		Assert.Equal(1, root.GetProperty("torrent_seeding").GetInt32());
	}

	[Fact]
	public void ModifyRssRequest_Serialize_WithNullOptionals_OmitsNullProperties()
	{
		// Arrange
		ModifyRssRequest request = new()
		{
			RssFeedId = 5,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal(5, root.GetProperty("rss_feed_id").GetInt64());
		Assert.False(root.TryGetProperty("name", out _));
		Assert.False(root.TryGetProperty("do_regex", out _));
		Assert.False(root.TryGetProperty("dont_regex", out _));
		Assert.False(root.TryGetProperty("scan_interval", out _));
		Assert.False(root.TryGetProperty("dont_older_than", out _));
		Assert.False(root.TryGetProperty("rss_type", out _));
		Assert.False(root.TryGetProperty("torrent_seeding", out _));
	}
}
