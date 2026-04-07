using TorBoxSDK.Main.WebDownloads;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.WebDownloads;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Main.WebDownloads;

public sealed class WebDownloadsClientTests
{
    private const string WebDownloadListJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "name": "my-web-download",
                    "hash": "abc123",
                    "size": 1073741824,
                    "active": true,
                    "download_state": "completed",
                    "download_finished": true,
                    "download_present": true,
                    "progress": 100.0,
                    "files": []
                }
            ]
        }
        """;

    private const string SingleWebDownloadJson = """
        {
            "success": true,
            "error": null,
            "detail": "Created.",
            "data": {
                "id": 1,
                "name": "my-web-download",
                "hash": "abc123",
                "size": 1073741824,
                "active": true,
                "download_state": "downloading",
                "download_finished": false,
                "download_present": false,
                "progress": 0.0,
                "files": []
            }
        }
        """;

    private const string SuccessJson = """
        {
            "success": true,
            "error": null,
            "detail": "OK."
        }
        """;

    private const string DownloadJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": "https://download.torbox.app/file/abc123"
        }
        """;

    private const string CachedJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {}
        }
        """;

    private const string HostersJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "name": "mega.nz",
                    "daily_bandwidth_limit": 5368709120,
                    "daily_bandwidth_used": 0,
                    "status": true
                }
            ]
        }
        """;

    // --- CreateWebDownloadAsync ---

    [Fact]
    public async Task CreateWebDownloadAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(SingleWebDownloadJson);
        CreateWebDownloadRequest request = new() { Link = "https://example.com/file.zip" };

        // Act
        TorBoxResponse<WebDownload> result = await client.CreateWebDownloadAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("webdl/createwebdownload", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task CreateWebDownloadAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (WebDownloadsClient client, _) = ClientTestBase.CreateClient<WebDownloadsClient>(SingleWebDownloadJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CreateWebDownloadAsync(null!));
    }

    // --- ControlWebDownloadAsync ---

    [Fact]
    public async Task ControlWebDownloadAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(SuccessJson);
        ControlWebDownloadRequest request = new() { WebdlId = 1, Operation = ControlOperation.Delete };

        // Act
        TorBoxResponse result = await client.ControlWebDownloadAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("webdl/controlwebdownload", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ControlWebDownloadAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (WebDownloadsClient client, _) = ClientTestBase.CreateClient<WebDownloadsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.ControlWebDownloadAsync(null!));
    }

    // --- RequestDownloadAsync ---

    [Fact]
    public async Task RequestDownloadAsync_WithValidOptions_SendsGetRequest()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(DownloadJson);
        RequestWebDownloadOptions options = new() { WebId = 1 };

        // Act
        TorBoxResponse<string> result = await client.RequestDownloadAsync(options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("webdl/requestdl", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("web_id=1", handler.LastRequest.RequestUri!.ToString());
        Assert.Equal("https://download.torbox.app/file/abc123", result.Data);
    }

    [Fact]
    public async Task RequestDownloadAsync_WithAllOptions_IncludesInQueryString()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(DownloadJson);
        RequestWebDownloadOptions options = new()
        {
            WebId = 42,
            Token = "custom-token",
            AppendName = true,
            Redirect = false,
        };

        // Act
        await client.RequestDownloadAsync(options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("web_id=42", url);
        Assert.Contains("token=custom-token", url);
        Assert.Contains("append_name=true", url);
        Assert.Contains("redirect=false", url);
    }

    [Fact]
    public async Task RequestDownloadAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        (WebDownloadsClient client, _) = ClientTestBase.CreateClient<WebDownloadsClient>(DownloadJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.RequestDownloadAsync(null!));
    }

    // --- GetMyWebDownloadListAsync ---

    [Fact]
    public async Task GetMyWebDownloadListAsync_WithValidResponse_ReturnsList()
    {
        // Arrange
        (WebDownloadsClient client, _) = ClientTestBase.CreateClient<WebDownloadsClient>(WebDownloadListJson);

        // Act
        TorBoxResponse<IReadOnlyList<WebDownload>> result = await client.GetMyWebDownloadListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal("my-web-download", result.Data[0].Name);
    }

    [Fact]
    public async Task GetMyWebDownloadListAsync_WithAllParams_IncludesInQueryString()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(WebDownloadListJson);

        // Act
        await client.GetMyWebDownloadListAsync(new GetMyListOptions { Id = 42, Offset = 10, Limit = 50, BypassCache = true });

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("id=42", url);
        Assert.Contains("offset=10", url);
        Assert.Contains("limit=50", url);
        Assert.Contains("bypass_cache=true", url);
    }

    [Fact]
    public async Task GetMyWebDownloadListAsync_WithNullParams_OmitsFromQueryString()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(WebDownloadListJson);

        // Act
        await client.GetMyWebDownloadListAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.DoesNotContain("id=", url);
        Assert.DoesNotContain("offset=", url);
        Assert.DoesNotContain("limit=", url);
        Assert.DoesNotContain("bypass_cache", url);
    }

    // --- CheckCachedAsync ---

    [Fact]
    public async Task CheckCachedAsync_WithHashes_SendsGetRequest()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(CachedJson);

        // Act
        await client.CheckCachedAsync(new CheckCachedOptions { Hashes = ["hash1", "hash2"] });

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("webdl/checkcached", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("hash=hash1%2Chash2", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task CheckCachedAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        (WebDownloadsClient client, _) = ClientTestBase.CreateClient<WebDownloadsClient>(CachedJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CheckCachedAsync(null!));
    }

    [Fact]
    public async Task CheckCachedAsync_WithNullHashes_ThrowsArgumentNullException()
    {
        // Arrange
        (WebDownloadsClient client, _) = ClientTestBase.CreateClient<WebDownloadsClient>(CachedJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CheckCachedAsync(new CheckCachedOptions { Hashes = null! }));
    }

    // --- CheckCachedByPostAsync ---

    [Fact]
    public async Task CheckCachedByPostAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(CachedJson);
        CheckWebCachedRequest request = new();

        // Act
        await client.CheckCachedByPostAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("webdl/checkcached", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task CheckCachedByPostAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (WebDownloadsClient client, _) = ClientTestBase.CreateClient<WebDownloadsClient>(CachedJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CheckCachedByPostAsync(null!));
    }

    // --- GetHostersAsync ---

    [Fact]
    public async Task GetHostersAsync_WithValidResponse_ReturnsHosters()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(HostersJson);

        // Act
        TorBoxResponse<IReadOnlyList<Hoster>> result = await client.GetHostersAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("webdl/hosters", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    // --- EditWebDownloadAsync ---

    [Fact]
    public async Task EditWebDownloadAsync_WithValidRequest_SendsPutRequest()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(SuccessJson);
        EditWebDownloadRequest request = new() { WebdlId = 1, Name = "new-name" };

        // Act
        TorBoxResponse result = await client.EditWebDownloadAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Put, handler.LastRequest.Method);
        Assert.Contains("webdl/editwebdownload", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task EditWebDownloadAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (WebDownloadsClient client, _) = ClientTestBase.CreateClient<WebDownloadsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.EditWebDownloadAsync(null!));
    }

    // --- AsyncCreateWebDownloadAsync ---

    [Fact]
    public async Task AsyncCreateWebDownloadAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (WebDownloadsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<WebDownloadsClient>(SingleWebDownloadJson);
        CreateWebDownloadRequest request = new() { Link = "https://example.com/file.zip" };

        // Act
        await client.AsyncCreateWebDownloadAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("webdl/asynccreatewebdownload", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task AsyncCreateWebDownloadAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (WebDownloadsClient client, _) = ClientTestBase.CreateClient<WebDownloadsClient>(SingleWebDownloadJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.AsyncCreateWebDownloadAsync(null!));
    }
}
