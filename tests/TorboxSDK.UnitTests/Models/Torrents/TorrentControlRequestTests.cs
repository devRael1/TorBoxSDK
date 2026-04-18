using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorboxSDK.UnitTests.Models.Torrents;

public sealed class TorrentControlRequestTests
{
    // ──── ControlTorrentRequest ────

    [Fact]
    public void ControlTorrentRequest_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        ControlTorrentRequest request = new()
        {
            TorrentId = 42,
            Operation = ControlOperation.Pause,
            All = false,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(42, root.GetProperty("torrent_id").GetInt64());
        Assert.True(root.TryGetProperty("operation", out _));
        Assert.False(root.GetProperty("all").GetBoolean());
    }

    [Fact]
    public void ControlTorrentRequest_Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        ControlTorrentRequest request = new()
        {
            Operation = ControlOperation.Delete,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("torrent_id", out _));
        Assert.False(root.TryGetProperty("all", out _));
        Assert.True(root.TryGetProperty("operation", out _));
    }

    // ──── CheckCachedRequest ────

    [Fact]
    public void CheckCachedRequest_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        CheckCachedRequest request = new()
        {
            Hashes = ["abc123", "def456", "ghi789"],
            Format = "object",
            ListFiles = true,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        JsonElement hashes = root.GetProperty("hashes");
        Assert.Equal(3, hashes.GetArrayLength());
        Assert.Equal("abc123", hashes[0].GetString());
        Assert.Equal("def456", hashes[1].GetString());
        Assert.Equal("ghi789", hashes[2].GetString());

        Assert.Equal("object", root.GetProperty("format").GetString());
        Assert.True(root.GetProperty("list_files").GetBoolean());
    }

    [Fact]
    public void CheckCachedRequest_Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        CheckCachedRequest request = new()
        {
            Hashes = ["abc123"],
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.True(root.TryGetProperty("hashes", out _));
        Assert.False(root.TryGetProperty("format", out _));
        Assert.False(root.TryGetProperty("list_files", out _));
    }

    // ──── MagnetToFileRequest ────

    [Fact]
    public void MagnetToFileRequest_Serialize_ProducesExpectedJson()
    {
        // Arrange
        MagnetToFileRequest request = new()
        {
            Magnet = "magnet:?xt=urn:btih:abc123def456ghi789",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("magnet:?xt=urn:btih:abc123def456ghi789", root.GetProperty("magnet").GetString());
    }

    // ──── GetTorrentInfoOptions ────

    [Fact]
    public void GetTorrentInfoOptions_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        GetTorrentInfoOptions options = new()
        {
            Timeout = 30,
            UseCacheLookup = true,
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(30, root.GetProperty("timeout").GetInt32());
        Assert.True(root.GetProperty("use_cache_lookup").GetBoolean());
    }

    [Fact]
    public void GetTorrentInfoOptions_Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        GetTorrentInfoOptions options = new();

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("timeout", out _));
        Assert.False(root.TryGetProperty("use_cache_lookup", out _));
    }
}
