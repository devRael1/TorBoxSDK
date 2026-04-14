using System.Text.Json;
using TorBoxSDK.Http;
using TorBoxSDK.Models.Usenet;

namespace TorboxSDK.UnitTests.Models.Usenet;

public sealed class UsenetRequestTests
{
    [Fact]
    public void CreateUsenetDownloadRequest_Serialize_WithNewProperties_ProducesExpectedJson()
    {
        // Arrange
        CreateUsenetDownloadRequest request = new()
        {
            Link = "https://example.com/file.nzb",
            Name = "my-download",
            Password = "secret",
            PostProcessing = -1,
            AsQueued = true,
            AddOnlyIfCached = false,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("https://example.com/file.nzb", root.GetProperty("link").GetString());
        Assert.True(root.GetProperty("as_queued").GetBoolean());
        Assert.False(root.GetProperty("add_only_if_cached").GetBoolean());
    }

    [Fact]
    public void CreateUsenetDownloadRequest_Serialize_WithNullNewProperties_OmitsThem()
    {
        // Arrange
        CreateUsenetDownloadRequest request = new()
        {
            Link = "https://example.com/file.nzb",
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
    public void EditUsenetDownloadRequest_Serialize_WithAlternativeHashes_ProducesExpectedJson()
    {
        // Arrange
        EditUsenetDownloadRequest request = new()
        {
            UsenetDownloadId = 10,
            Name = "updated-name",
            Tags = ["usenet", "download"],
            AlternativeHashes = ["hash-a", "hash-b"],
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(10, root.GetProperty("usenet_download_id").GetInt64());
        Assert.Equal("updated-name", root.GetProperty("name").GetString());

        JsonElement hashes = root.GetProperty("alternative_hashes");
        Assert.Equal(2, hashes.GetArrayLength());
        Assert.Equal("hash-a", hashes[0].GetString());
        Assert.Equal("hash-b", hashes[1].GetString());
    }

    [Fact]
    public void EditUsenetDownloadRequest_Serialize_WithNullAlternativeHashes_OmitsProperty()
    {
        // Arrange
        EditUsenetDownloadRequest request = new()
        {
            UsenetDownloadId = 5,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("alternative_hashes", out _));
    }

    [Fact]
    public void RequestUsenetDownloadOptions_Serialize_WithAllNewProperties_ProducesExpectedJson()
    {
        // Arrange
        RequestUsenetDownloadOptions options = new()
        {
            UsenetId = 42,
            FileId = 3,
            ZipLink = true,
            UserIp = "10.0.0.1",
            Token = "usenet-token",
            Redirect = true,
            AppendName = false,
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(42, root.GetProperty("usenet_id").GetInt64());
        Assert.Equal("usenet-token", root.GetProperty("token").GetString());
        Assert.True(root.GetProperty("redirect").GetBoolean());
        Assert.False(root.GetProperty("append_name").GetBoolean());
    }

    [Fact]
    public void RequestUsenetDownloadOptions_Serialize_WithNullNewProperties_OmitsThem()
    {
        // Arrange
        RequestUsenetDownloadOptions options = new()
        {
            UsenetId = 1,
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("token", out _));
        Assert.False(root.TryGetProperty("redirect", out _));
        Assert.False(root.TryGetProperty("append_name", out _));
    }
}
