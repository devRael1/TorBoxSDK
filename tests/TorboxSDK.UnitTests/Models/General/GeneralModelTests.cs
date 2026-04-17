using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.General;

namespace TorboxSDK.UnitTests.Models.General;

public sealed class GeneralModelTests
{
    // ──── Stats ────

    [Fact]
    public void Stats_Deserialize_PopulatesProperties()
    {
        // Arrange
        string json = """
            {
                "total_users": 250000,
                "total_servers": 42
            }
            """;

        // Act
        Stats? result = JsonSerializer.Deserialize<Stats>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(250000L, result.TotalUsers);
        Assert.Equal(42, result.TotalServers);
    }

    // ──── DailyStats ────

    [Fact]
    public void DailyStats_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "total_downloads": 1500000,
                "total_users": 250000,
                "total_bytes_downloaded": 9876543210,
                "total_bytes_uploaded": 1234567890,
                "total_bytes_egressed": 5555555555,
                "total_servers": 42,
                "created_at": "2025-01-15T10:30:00Z"
            }
            """;

        // Act
        DailyStats? result = JsonSerializer.Deserialize<DailyStats>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1500000L, result.TotalDownloads);
        Assert.Equal(250000L, result.TotalUsers);
        Assert.Equal(9876543210L, result.TotalBytesDownloaded);
        Assert.Equal(1234567890L, result.TotalBytesUploaded);
        Assert.Equal(5555555555L, result.TotalBytesEgressed);
        Assert.Equal(42, result.TotalServers);
        Assert.NotNull(result.CreatedAt);
        Assert.Equal(new DateTimeOffset(2025, 1, 15, 10, 30, 0, TimeSpan.Zero), result.CreatedAt);
    }

    [Fact]
    public void DailyStats_Deserialize_WithNullCreatedAt_ReturnsNull()
    {
        // Arrange
        string json = """
            {
                "total_downloads": 100,
                "total_users": 50,
                "total_bytes_downloaded": 0,
                "total_bytes_uploaded": 0,
                "total_bytes_egressed": 0,
                "total_servers": 1
            }
            """;

        // Act
        DailyStats? result = JsonSerializer.Deserialize<DailyStats>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.CreatedAt);
    }

    // ──── SpeedtestServer / ServerCoordinates ────

    [Fact]
    public void SpeedtestServer_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "region": "us-east-1",
                "name": "US East",
                "domain": "speedtest.torbox.app",
                "path": "/test/100mb.bin",
                "url": "https://speedtest.torbox.app/test/100mb.bin",
                "closest": true,
                "coordinates": {
                    "lat": 39.0438,
                    "lng": -77.4874
                }
            }
            """;

        // Act
        SpeedtestServer? result = JsonSerializer.Deserialize<SpeedtestServer>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("us-east-1", result.Region);
        Assert.Equal("US East", result.Name);
        Assert.Equal("speedtest.torbox.app", result.Domain);
        Assert.Equal("/test/100mb.bin", result.Path);
        Assert.Equal("https://speedtest.torbox.app/test/100mb.bin", result.Url);
        Assert.True(result.Closest);
        Assert.NotNull(result.Coordinates);
        Assert.Equal(39.0438, result.Coordinates.Lat);
        Assert.Equal(-77.4874, result.Coordinates.Lng);
    }

    [Fact]
    public void SpeedtestServer_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {
                "closest": false
            }
            """;

        // Act
        SpeedtestServer? result = JsonSerializer.Deserialize<SpeedtestServer>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Region);
        Assert.Null(result.Name);
        Assert.Null(result.Domain);
        Assert.Null(result.Path);
        Assert.Null(result.Url);
        Assert.False(result.Closest);
        Assert.Null(result.Coordinates);
    }

    // ──── Changelog ────

    [Fact]
    public void Changelog_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": 7,
                "name": "v2.5.0",
                "html": "<p>Bug fixes and improvements</p>",
                "markdown": "# Bug fixes and improvements",
                "link": "https://torbox.app/changelog/v2.5.0",
                "created_at": "2025-03-01T14:00:00Z"
            }
            """;

        // Act
        Changelog? result = JsonSerializer.Deserialize<Changelog>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        Assert.Equal("v2.5.0", result.Name);
        Assert.Equal("<p>Bug fixes and improvements</p>", result.Html);
        Assert.Equal("# Bug fixes and improvements", result.Markdown);
        Assert.Equal("https://torbox.app/changelog/v2.5.0", result.Link);
        Assert.NotNull(result.CreatedAt);
        Assert.Equal(new DateTimeOffset(2025, 3, 1, 14, 0, 0, TimeSpan.Zero), result.CreatedAt);
    }

    [Fact]
    public void Changelog_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {
                "id": 1,
                "name": "v1.0.0"
            }
            """;

        // Act
        Changelog? result = JsonSerializer.Deserialize<Changelog>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("v1.0.0", result.Name);
        Assert.Null(result.Html);
        Assert.Null(result.Markdown);
        Assert.Null(result.Link);
        Assert.Null(result.CreatedAt);
    }

    // ──── SpeedtestOptions ────

    [Fact]
    public void SpeedtestOptions_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        SpeedtestOptions options = new()
        {
            UserIp = "203.0.113.42",
            Region = "eu-west-1",
            TestLength = "short",
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("203.0.113.42", root.GetProperty("user_ip").GetString());
        Assert.Equal("eu-west-1", root.GetProperty("region").GetString());
        Assert.Equal("short", root.GetProperty("test_length").GetString());
    }

    [Fact]
    public void SpeedtestOptions_Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        SpeedtestOptions options = new();

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("user_ip", out _));
        Assert.False(root.TryGetProperty("region", out _));
        Assert.False(root.TryGetProperty("test_length", out _));
    }
}
