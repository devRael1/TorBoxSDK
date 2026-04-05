using System.Text.Json;

using TorBoxSDK.Http;
using TorBoxSDK.Models.Torrents;

namespace TorboxSDK.UnitTests.Models.Torrents;

public sealed class CreateTorrentRequestTests
{
    [Fact]
    public void Serialize_WithNewProperties_ProducesExpectedJson()
    {
        // Arrange
        var request = new CreateTorrentRequest
        {
            Magnet = "magnet:?xt=urn:btih:abc123",
            Name = "ubuntu",
            AsQueued = true,
            AddOnlyIfCached = false,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("magnet:?xt=urn:btih:abc123", root.GetProperty("magnet").GetString());
        Assert.Equal("ubuntu", root.GetProperty("name").GetString());
        Assert.True(root.GetProperty("as_queued").GetBoolean());
        Assert.False(root.GetProperty("add_only_if_cached").GetBoolean());
    }

    [Fact]
    public void Serialize_WithNullNewProperties_OmitsThem()
    {
        // Arrange
        var request = new CreateTorrentRequest
        {
            Magnet = "magnet:?xt=urn:btih:abc123",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("as_queued", out _));
        Assert.False(root.TryGetProperty("add_only_if_cached", out _));
    }

    [Fact]
    public void Serialize_FileProperty_IsExcludedFromJson()
    {
        // Arrange
        var request = new CreateTorrentRequest
        {
            File = [0x01, 0x02],
            Magnet = "magnet:?xt=urn:btih:abc123",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("file", out _));
        Assert.Equal("magnet:?xt=urn:btih:abc123", root.GetProperty("magnet").GetString());
    }
}
