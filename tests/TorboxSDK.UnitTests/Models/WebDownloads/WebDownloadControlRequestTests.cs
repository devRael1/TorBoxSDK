using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.WebDownloads;

namespace TorboxSDK.UnitTests.Models.WebDownloads;

public sealed class WebDownloadControlRequestTests
{
    // ──── ControlWebDownloadRequest ────

    [Fact]
    public void ControlWebDownloadRequest_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        ControlWebDownloadRequest request = new()
        {
            WebdlId = 33,
            Operation = ControlOperation.Pause,
            All = true,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(33, root.GetProperty("webdl_id").GetInt64());
        Assert.True(root.TryGetProperty("operation", out _));
        Assert.True(root.GetProperty("all").GetBoolean());
    }

    [Fact]
    public void ControlWebDownloadRequest_Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        ControlWebDownloadRequest request = new()
        {
            Operation = ControlOperation.Resume,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("webdl_id", out _));
        Assert.False(root.TryGetProperty("all", out _));
        Assert.True(root.TryGetProperty("operation", out _));
    }

    // ──── CheckWebCachedRequest ────

    [Fact]
    public void CheckWebCachedRequest_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        CheckWebCachedRequest request = new()
        {
            Hashes = ["web-hash-a", "web-hash-b", "web-hash-c"],
            Format = "object",
            ListFiles = false,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        JsonElement hashes = root.GetProperty("hashes");
        Assert.Equal(3, hashes.GetArrayLength());
        Assert.Equal("web-hash-a", hashes[0].GetString());
        Assert.Equal("web-hash-b", hashes[1].GetString());
        Assert.Equal("web-hash-c", hashes[2].GetString());

        Assert.Equal("object", root.GetProperty("format").GetString());
        Assert.False(root.GetProperty("list_files").GetBoolean());
    }

    [Fact]
    public void CheckWebCachedRequest_Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        CheckWebCachedRequest request = new()
        {
            Hashes = ["web-hash-1"],
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
}
