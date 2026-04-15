using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.WebDownloads;

namespace TorboxSDK.UnitTests.Models.WebDownloads;

public sealed class WebDownloadRequestTests
{
    [Fact]
    public void CreateWebDownloadRequest_Serialize_WithNewProperties_ProducesExpectedJson()
    {
        // Arrange
        CreateWebDownloadRequest request = new()
        {
            Link = "https://example.com/file.zip",
            Name = "my-file",
            Password = "secret",
            AsQueued = true,
            AddOnlyIfCached = false,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("https://example.com/file.zip", root.GetProperty("link").GetString());
        Assert.True(root.GetProperty("as_queued").GetBoolean());
        Assert.False(root.GetProperty("add_only_if_cached").GetBoolean());
    }

    [Fact]
    public void CreateWebDownloadRequest_Serialize_WithNullNewProperties_OmitsThem()
    {
        // Arrange
        CreateWebDownloadRequest request = new()
        {
            Link = "https://example.com/file.zip",
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
    public void EditWebDownloadRequest_Serialize_WithAlternativeHashes_ProducesExpectedJson()
    {
        // Arrange
        EditWebDownloadRequest request = new()
        {
            WebdlId = 77,
            Name = "renamed-download",
            Tags = ["web", "download"],
            AlternativeHashes = ["web-hash-1", "web-hash-2"],
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(77, root.GetProperty("webdl_id").GetInt64());
        Assert.Equal("renamed-download", root.GetProperty("name").GetString());

        JsonElement hashes = root.GetProperty("alternative_hashes");
        Assert.Equal(2, hashes.GetArrayLength());
        Assert.Equal("web-hash-1", hashes[0].GetString());
        Assert.Equal("web-hash-2", hashes[1].GetString());
    }

    [Fact]
    public void EditWebDownloadRequest_Serialize_WithNullAlternativeHashes_OmitsProperty()
    {
        // Arrange
        EditWebDownloadRequest request = new()
        {
            WebdlId = 5,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("alternative_hashes", out _));
    }

    [Fact]
    public void RequestWebDownloadOptions_Serialize_WithAllNewProperties_ProducesExpectedJson()
    {
        // Arrange
        RequestWebDownloadOptions options = new()
        {
            WebId = 42,
            FileId = 2,
            ZipLink = false,
            UserIp = "172.16.0.1",
            Token = "web-token-abc",
            Redirect = true,
            AppendName = true,
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(42, root.GetProperty("web_id").GetInt64());
        Assert.Equal("web-token-abc", root.GetProperty("token").GetString());
        Assert.True(root.GetProperty("redirect").GetBoolean());
        Assert.True(root.GetProperty("append_name").GetBoolean());
    }

    [Fact]
    public void RequestWebDownloadOptions_Serialize_WithNullNewProperties_OmitsThem()
    {
        // Arrange
        RequestWebDownloadOptions options = new()
        {
            WebId = 1,
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
