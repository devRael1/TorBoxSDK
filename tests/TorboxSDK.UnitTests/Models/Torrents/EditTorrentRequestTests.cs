using System.Text.Json;
using TorBoxSDK.Http;
using TorBoxSDK.Models.Torrents;

namespace TorboxSDK.UnitTests.Models.Torrents;

public sealed class EditTorrentRequestTests
{
    [Fact]
    public void Serialize_WithAlternativeHashes_ProducesExpectedJson()
    {
        // Arrange
        EditTorrentRequest request = new()
        {
            TorrentId = 42,
            Name = "updated-name",
            Tags = ["linux", "ubuntu"],
            AlternativeHashes = ["hash1", "hash2", "hash3"],
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(42, root.GetProperty("torrent_id").GetInt64());
        Assert.Equal("updated-name", root.GetProperty("name").GetString());

        JsonElement hashes = root.GetProperty("alternative_hashes");
        Assert.Equal(3, hashes.GetArrayLength());
        Assert.Equal("hash1", hashes[0].GetString());
        Assert.Equal("hash2", hashes[1].GetString());
        Assert.Equal("hash3", hashes[2].GetString());
    }

    [Fact]
    public void Serialize_WithNullAlternativeHashes_OmitsProperty()
    {
        // Arrange
        EditTorrentRequest request = new()
        {
            TorrentId = 42,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("alternative_hashes", out _));
    }

    [Fact]
    public void Deserialize_WithAlternativeHashes_PopulatesProperty()
    {
        // Arrange
        string json = """
            {
                "torrent_id": 10,
                "name": "test",
                "tags": ["tag1"],
                "alternative_hashes": ["altHash1", "altHash2"]
            }
            """;

        // Act
        EditTorrentRequest? result = JsonSerializer.Deserialize<EditTorrentRequest>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.TorrentId);
        Assert.NotNull(result.AlternativeHashes);
        Assert.Equal(2, result.AlternativeHashes.Count);
        Assert.Equal("altHash1", result.AlternativeHashes[0]);
        Assert.Equal("altHash2", result.AlternativeHashes[1]);
    }
}
