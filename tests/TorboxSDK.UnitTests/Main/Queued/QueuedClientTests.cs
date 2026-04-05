using TorBoxSDK.Main.Queued;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Queued;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Main.Queued;

public sealed class QueuedClientTests
{
    private const string QueuedListJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "auth_id": "user-123",
                    "name": "queued-download",
                    "download_type": "torrent",
                    "magnet": "magnet:?xt=urn:btih:abc123",
                    "hash": "abc123",
                    "size": 1073741824,
                    "status": "pending"
                }
            ]
        }
        """;

    private const string SuccessJson = """
        {
            "success": true,
            "error": null,
            "detail": "OK."
        }
        """;

    [Fact]
    public async Task GetQueuedAsync_WithNoParams_SendsCorrectUrl()
    {
        // Arrange
        var (client, handler) = ClientTestBase.CreateClient<QueuedClient>(QueuedListJson);

        // Act
        TorBoxResponse<IReadOnlyList<QueuedDownload>> result = await client.GetQueuedAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("queued/getqueued", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task GetQueuedAsync_WithAllParams_IncludesInQueryString()
    {
        // Arrange
        var (client, handler) = ClientTestBase.CreateClient<QueuedClient>(QueuedListJson);

        // Act
        await client.GetQueuedAsync(id: 42, offset: 10, limit: 50, bypassCache: true, type: "torrent");

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("id=42", url);
        Assert.Contains("offset=10", url);
        Assert.Contains("limit=50", url);
        Assert.Contains("bypass_cache=true", url);
        Assert.Contains("type=torrent", url);
    }

    [Fact]
    public async Task GetQueuedAsync_WithNullParams_OmitsFromQueryString()
    {
        // Arrange
        var (client, handler) = ClientTestBase.CreateClient<QueuedClient>(QueuedListJson);

        // Act
        await client.GetQueuedAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.DoesNotContain("id=", url);
        Assert.DoesNotContain("offset=", url);
        Assert.DoesNotContain("limit=", url);
        Assert.DoesNotContain("bypass_cache=", url);
        Assert.DoesNotContain("type=", url);
    }

    [Fact]
    public async Task ControlQueuedAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        var (client, handler) = ClientTestBase.CreateClient<QueuedClient>(SuccessJson);
        var request = new ControlQueuedRequest { QueuedId = 1, Operation = ControlOperation.Delete };

        // Act
        TorBoxResponse result = await client.ControlQueuedAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("queued/controlqueued", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ControlQueuedAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var (client, _) = ClientTestBase.CreateClient<QueuedClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.ControlQueuedAsync(null!));
    }
}
