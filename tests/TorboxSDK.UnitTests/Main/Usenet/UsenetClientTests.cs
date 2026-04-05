using TorBoxSDK.Main.Usenet;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Usenet;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Main.Usenet;

public sealed class UsenetClientTests
{
    private const string UsenetListJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "name": "my-nzb-download",
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

    private const string SingleUsenetJson = """
        {
            "success": true,
            "error": null,
            "detail": "Created.",
            "data": {
                "id": 1,
                "name": "my-nzb-download",
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

    // --- CreateUsenetDownloadAsync ---

    [Fact]
    public async Task CreateUsenetDownloadAsync_WithLink_SendsMultipartRequest()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(SingleUsenetJson);
        CreateUsenetDownloadRequest request = new() { Link = "https://example.com/file.nzb" };

        // Act
        TorBoxResponse<UsenetDownload> result = await client.CreateUsenetDownloadAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("usenet/createusenetdownload", handler.LastRequest.RequestUri!.ToString());
        Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task CreateUsenetDownloadAsync_WithFile_SendsMultipartRequest()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(SingleUsenetJson);
        CreateUsenetDownloadRequest request = new() { File = [0x01, 0x02, 0x03] };

        // Act
        await client.CreateUsenetDownloadAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
    }

    [Fact]
    public async Task CreateUsenetDownloadAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(SingleUsenetJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CreateUsenetDownloadAsync(null!));
    }

    [Fact]
    public async Task CreateUsenetDownloadAsync_WithNoLinkOrFile_ThrowsArgumentException()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(SingleUsenetJson);
        CreateUsenetDownloadRequest request = new();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.CreateUsenetDownloadAsync(request));
    }

    // --- ControlUsenetDownloadAsync ---

    [Fact]
    public async Task ControlUsenetDownloadAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(SuccessJson);
        ControlUsenetDownloadRequest request = new() { UsenetId = 1, Operation = ControlOperation.Delete };

        // Act
        TorBoxResponse result = await client.ControlUsenetDownloadAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("usenet/controlusenetdownload", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ControlUsenetDownloadAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.ControlUsenetDownloadAsync(null!));
    }

    // --- RequestDownloadAsync ---

    [Fact]
    public async Task RequestDownloadAsync_WithValidOptions_SendsGetRequest()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(DownloadJson);
        RequestUsenetDownloadOptions options = new() { UsenetId = 1 };

        // Act
        TorBoxResponse<string> result = await client.RequestDownloadAsync(options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("usenet/requestdl", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("usenet_id=1", handler.LastRequest.RequestUri!.ToString());
        Assert.Equal("https://download.torbox.app/file/abc123", result.Data);
    }

    [Fact]
    public async Task RequestDownloadAsync_WithAllOptions_IncludesInQueryString()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(DownloadJson);
        RequestUsenetDownloadOptions options = new()
        {
            UsenetId = 42,
            Token = "custom-token",
            AppendName = true,
            Redirect = false,
        };

        // Act
        await client.RequestDownloadAsync(options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("usenet_id=42", url);
        Assert.Contains("token=custom-token", url);
        Assert.Contains("append_name=true", url);
        Assert.Contains("redirect=false", url);
    }

    [Fact]
    public async Task RequestDownloadAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(DownloadJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.RequestDownloadAsync(null!));
    }

    // --- GetMyUsenetListAsync ---

    [Fact]
    public async Task GetMyUsenetListAsync_WithValidResponse_ReturnsList()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(UsenetListJson);

        // Act
        TorBoxResponse<IReadOnlyList<UsenetDownload>> result = await client.GetMyUsenetListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal("my-nzb-download", result.Data[0].Name);
    }

    [Fact]
    public async Task GetMyUsenetListAsync_WithAllParams_IncludesInQueryString()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(UsenetListJson);

        // Act
        await client.GetMyUsenetListAsync(id: 42, offset: 10, limit: 50, bypassCache: true);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("id=42", url);
        Assert.Contains("offset=10", url);
        Assert.Contains("limit=50", url);
        Assert.Contains("bypass_cache=true", url);
    }

    [Fact]
    public async Task GetMyUsenetListAsync_WithNullParams_OmitsFromQueryString()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(UsenetListJson);

        // Act
        await client.GetMyUsenetListAsync();

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
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(CachedJson);
        IReadOnlyList<string> hashes = ["hash1", "hash2"];

        // Act
        await client.CheckCachedAsync(hashes);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("usenet/checkcached", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("hash=hash1%2Chash2", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task CheckCachedAsync_WithAllParams_IncludesInQueryString()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(CachedJson);

        // Act
        await client.CheckCachedAsync(["hash1"], format: "object", listFiles: true);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("format=object", url);
        Assert.Contains("list_files=true", url);
    }

    [Fact]
    public async Task CheckCachedAsync_WithNullHashes_ThrowsArgumentNullException()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(CachedJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CheckCachedAsync(null!));
    }

    // --- CheckCachedByPostAsync ---

    [Fact]
    public async Task CheckCachedByPostAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(CachedJson);
        CheckUsenetCachedRequest request = new();

        // Act
        await client.CheckCachedByPostAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("usenet/checkcached", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task CheckCachedByPostAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(CachedJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CheckCachedByPostAsync(null!));
    }

    // --- EditUsenetDownloadAsync ---

    [Fact]
    public async Task EditUsenetDownloadAsync_WithValidRequest_SendsPutRequest()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(SuccessJson);
        EditUsenetDownloadRequest request = new() { UsenetDownloadId = 1, Name = "new-name" };

        // Act
        TorBoxResponse result = await client.EditUsenetDownloadAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Put, handler.LastRequest.Method);
        Assert.Contains("usenet/editusenetdownload", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task EditUsenetDownloadAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.EditUsenetDownloadAsync(null!));
    }

    // --- AsyncCreateUsenetDownloadAsync ---

    [Fact]
    public async Task AsyncCreateUsenetDownloadAsync_WithLink_SendsMultipartRequest()
    {
        // Arrange
        (UsenetClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UsenetClient>(SingleUsenetJson);
        CreateUsenetDownloadRequest request = new() { Link = "https://example.com/file.nzb" };

        // Act
        await client.AsyncCreateUsenetDownloadAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("usenet/asynccreateusenetdownload", handler.LastRequest.RequestUri!.ToString());
        Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
    }

    [Fact]
    public async Task AsyncCreateUsenetDownloadAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(SingleUsenetJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.AsyncCreateUsenetDownloadAsync(null!));
    }

    [Fact]
    public async Task AsyncCreateUsenetDownloadAsync_WithNoLinkOrFile_ThrowsArgumentException()
    {
        // Arrange
        (UsenetClient client, _) = ClientTestBase.CreateClient<UsenetClient>(SingleUsenetJson);
        CreateUsenetDownloadRequest request = new();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.AsyncCreateUsenetDownloadAsync(request));
    }
}
