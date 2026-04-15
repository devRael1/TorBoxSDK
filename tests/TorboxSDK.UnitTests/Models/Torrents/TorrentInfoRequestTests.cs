using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Torrents;

namespace TorboxSDK.UnitTests.Models.Torrents;

public sealed class TorrentInfoRequestTests
{
    [Fact]
    public void Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        TorrentInfoRequest request = new()
        {
            File = [0x01, 0x02],
            FileName = "test.torrent",
            Magnet = "magnet:?xt=urn:btih:abc123",
            Hash = "abc123def456",
            Timeout = 30,
            UseCacheLookup = true,
            PeersOnly = false,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("magnet:?xt=urn:btih:abc123", root.GetProperty("magnet").GetString());
        Assert.Equal("abc123def456", root.GetProperty("hash").GetString());
        Assert.Equal(30, root.GetProperty("timeout").GetInt32());
        Assert.True(root.GetProperty("use_cache_lookup").GetBoolean());
        Assert.False(root.GetProperty("peers_only").GetBoolean());
    }

    [Fact]
    public void Serialize_FileAndFileName_AreExcludedFromJson()
    {
        // Arrange
        TorrentInfoRequest request = new()
        {
            File = [0x01, 0x02, 0x03],
            FileName = "my-torrent.torrent",
            Hash = "abc123",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("file", out _));
        Assert.False(root.TryGetProperty("file_name", out _));
        Assert.Equal("abc123", root.GetProperty("hash").GetString());
    }

    [Fact]
    public void Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        TorrentInfoRequest request = new()
        {
            Magnet = "magnet:?xt=urn:btih:abc123",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("magnet:?xt=urn:btih:abc123", root.GetProperty("magnet").GetString());
        Assert.False(root.TryGetProperty("hash", out _));
        Assert.False(root.TryGetProperty("timeout", out _));
        Assert.False(root.TryGetProperty("use_cache_lookup", out _));
        Assert.False(root.TryGetProperty("peers_only", out _));
    }

    [Fact]
    public void Deserialize_WithAllProperties_PopulatesRequest()
    {
        // Arrange
        string json = """
            {
                "magnet": "magnet:?xt=urn:btih:def456",
                "hash": "def456ghi789",
                "timeout": 60,
                "use_cache_lookup": false,
                "peers_only": true
            }
            """;

        // Act
        TorrentInfoRequest? result = JsonSerializer.Deserialize<TorrentInfoRequest>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("magnet:?xt=urn:btih:def456", result.Magnet);
        Assert.Equal("def456ghi789", result.Hash);
        Assert.Equal(60, result.Timeout);
        Assert.False(result.UseCacheLookup);
        Assert.True(result.PeersOnly);
    }
}
