using System.Text.Json;

using TorBoxSDK.Http;
using TorBoxSDK.Models.Rss;

namespace TorboxSDK.UnitTests.Models.Rss;

public sealed class AddRssRequestTests
{
    [Fact]
    public void Serialize_WithRenamedJsonProperties_UsesCorrectNames()
    {
        // Arrange
        var request = new AddRssRequest
        {
            Url = "https://example.com/rss",
            Name = "My Feed",
            RegexFilter = @"^ubuntu.*\.iso$",
            RegexFilterExclude = @"\.exe$",
            ScanInterval = 60,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("https://example.com/rss", root.GetProperty("url").GetString());
        Assert.Equal("My Feed", root.GetProperty("name").GetString());
        Assert.Equal(@"^ubuntu.*\.iso$", root.GetProperty("do_regex").GetString());
        Assert.Equal(@"\.exe$", root.GetProperty("dont_regex").GetString());
        Assert.Equal(60, root.GetProperty("scan_interval").GetInt32());
    }

    [Fact]
    public void Serialize_DoesNotUseOldPropertyNames()
    {
        // Arrange
        var request = new AddRssRequest
        {
            Url = "https://example.com/rss",
            RegexFilter = "test",
            RegexFilterExclude = "exclude",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("regex_filter", out _));
        Assert.False(root.TryGetProperty("regex_filter_exclude", out _));
        Assert.True(root.TryGetProperty("do_regex", out _));
        Assert.True(root.TryGetProperty("dont_regex", out _));
    }

    [Fact]
    public void Serialize_WithNewProperties_ProducesExpectedJson()
    {
        // Arrange
        var request = new AddRssRequest
        {
            Url = "https://example.com/rss",
            DontOlderThan = 7,
            PassCheck = true,
            TorrentSeeding = 1,
            RssType = "torrent",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(7, root.GetProperty("dont_older_than").GetInt32());
        Assert.True(root.GetProperty("pass_check").GetBoolean());
        Assert.Equal(1, root.GetProperty("torrent_seeding").GetInt32());
        Assert.Equal("torrent", root.GetProperty("rss_type").GetString());
    }

    [Fact]
    public void Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        var request = new AddRssRequest
        {
            Url = "https://example.com/rss",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("https://example.com/rss", root.GetProperty("url").GetString());
        Assert.False(root.TryGetProperty("name", out _));
        Assert.False(root.TryGetProperty("do_regex", out _));
        Assert.False(root.TryGetProperty("dont_regex", out _));
        Assert.False(root.TryGetProperty("scan_interval", out _));
        Assert.False(root.TryGetProperty("dont_older_than", out _));
        Assert.False(root.TryGetProperty("pass_check", out _));
        Assert.False(root.TryGetProperty("torrent_seeding", out _));
    }

    [Fact]
    public void Deserialize_WithNewJsonNames_PopulatesProperties()
    {
        // Arrange
        string json = """
            {
                "url": "https://example.com/rss",
                "name": "Feed",
                "do_regex": "include-pattern",
                "dont_regex": "exclude-pattern",
                "scan_interval": 120,
                "rss_type": "usenet",
                "dont_older_than": 14,
                "pass_check": false,
                "torrent_seeding": 2
            }
            """;

        // Act
        AddRssRequest? result = JsonSerializer.Deserialize<AddRssRequest>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("https://example.com/rss", result.Url);
        Assert.Equal("Feed", result.Name);
        Assert.Equal("include-pattern", result.RegexFilter);
        Assert.Equal("exclude-pattern", result.RegexFilterExclude);
        Assert.Equal(120, result.ScanInterval);
        Assert.Equal("usenet", result.RssType);
        Assert.Equal(14, result.DontOlderThan);
        Assert.False(result.PassCheck);
        Assert.Equal(2, result.TorrentSeeding);
    }
}
