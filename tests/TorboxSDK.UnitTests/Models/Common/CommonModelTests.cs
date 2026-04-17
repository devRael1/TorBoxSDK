using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Common;

namespace TorboxSDK.UnitTests.Models.Common;

public sealed class CommonModelTests
{
    // ──── DownloadFile ────

    [Fact]
    public void DownloadFile_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": 1001,
                "md5": "d41d8cd98f00b204e9800998ecf8427e",
                "mimetype": "video/x-matroska",
                "name": "Movie/movie.mkv",
                "s3_path": "downloads/abc123/movie.mkv",
                "short_name": "movie.mkv",
                "size": 1073741824,
                "hash": "abc123def456",
                "zipped": false,
                "infected": false,
                "absolute_path": "/mnt/storage/downloads/movie.mkv",
                "opensubtitles_hash": "8e245d9679d31e12"
            }
            """;

        // Act
        DownloadFile? result = JsonSerializer.Deserialize<DownloadFile>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1001L, result.Id);
        Assert.Equal("d41d8cd98f00b204e9800998ecf8427e", result.Md5);
        Assert.Equal("video/x-matroska", result.Mimetype);
        Assert.Equal("Movie/movie.mkv", result.Name);
        Assert.Equal("downloads/abc123/movie.mkv", result.S3Path);
        Assert.Equal("movie.mkv", result.ShortName);
        Assert.Equal(1073741824L, result.Size);
        Assert.Equal("abc123def456", result.Hash);
        Assert.False(result.Zipped);
        Assert.False(result.Infected);
        Assert.Equal("/mnt/storage/downloads/movie.mkv", result.AbsolutePath);
        Assert.Equal("8e245d9679d31e12", result.OpensubtitlesHash);
    }

    [Fact]
    public void DownloadFile_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {
                "id": 1,
                "name": "file.bin",
                "size": 0,
                "zipped": false,
                "infected": false
            }
            """;

        // Act
        DownloadFile? result = JsonSerializer.Deserialize<DownloadFile>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1L, result.Id);
        Assert.Equal("file.bin", result.Name);
        Assert.Null(result.Md5);
        Assert.Null(result.Mimetype);
        Assert.Null(result.S3Path);
        Assert.Null(result.ShortName);
        Assert.Null(result.Hash);
        Assert.Null(result.AbsolutePath);
        Assert.Null(result.OpensubtitlesHash);
    }

    [Fact]
    public void DownloadFile_Deserialize_InfectedFile_ReturnsTrue()
    {
        // Arrange
        string json = """
            {
                "id": 2,
                "name": "suspicious.exe",
                "size": 512,
                "zipped": true,
                "infected": true
            }
            """;

        // Act
        DownloadFile? result = JsonSerializer.Deserialize<DownloadFile>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Zipped);
        Assert.True(result.Infected);
    }

    // ──── GetMyListOptions ────

    [Fact]
    public void GetMyListOptions_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        GetMyListOptions options = new()
        {
            Id = 42,
            Offset = 10,
            Limit = 25,
            BypassCache = true,
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(42, root.GetProperty("id").GetInt64());
        Assert.Equal(10, root.GetProperty("offset").GetInt32());
        Assert.Equal(25, root.GetProperty("limit").GetInt32());
        Assert.True(root.GetProperty("bypass_cache").GetBoolean());
    }

    [Fact]
    public void GetMyListOptions_Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        GetMyListOptions options = new();

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("id", out _));
        Assert.False(root.TryGetProperty("offset", out _));
        Assert.False(root.TryGetProperty("limit", out _));
        Assert.False(root.TryGetProperty("bypass_cache", out _));
    }

    // ──── CheckCachedOptions ────

    [Fact]
    public void CheckCachedOptions_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        CheckCachedOptions options = new()
        {
            Hashes = ["hash1", "hash2", "hash3"],
            Format = "object",
            ListFiles = true,
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        JsonElement hashes = root.GetProperty("hash");
        Assert.Equal(3, hashes.GetArrayLength());
        Assert.Equal("hash1", hashes[0].GetString());
        Assert.Equal("hash2", hashes[1].GetString());
        Assert.Equal("hash3", hashes[2].GetString());

        Assert.Equal("object", root.GetProperty("format").GetString());
        Assert.True(root.GetProperty("list_files").GetBoolean());
    }

    [Fact]
    public void CheckCachedOptions_Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        CheckCachedOptions options = new()
        {
            Hashes = ["abc123"],
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.True(root.TryGetProperty("hash", out _));
        Assert.False(root.TryGetProperty("format", out _));
        Assert.False(root.TryGetProperty("list_files", out _));
    }
}
