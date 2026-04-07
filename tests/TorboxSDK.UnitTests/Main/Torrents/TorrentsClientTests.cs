using TorBoxSDK.Main.Torrents;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Main.Torrents;

public sealed class TorrentsClientTests
{
    private const string TorrentListJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "name": "ubuntu-24.04.torrent",
                    "hash": "abc123def456",
                    "size": 2147483648,
                    "progress": 100.0,
                    "download_speed": 0,
                    "upload_speed": 1024,
                    "seeds": 50,
                    "peers": 10,
                    "active": true,
                    "download_state": "completed",
                    "download_finished": true,
                    "download_present": true,
                    "files": []
                }
            ]
        }
        """;

    private const string SingleTorrentJson = """
        {
            "success": true,
            "error": null,
            "detail": "Created.",
            "data": {
                "id": 1,
                "name": "ubuntu-24.04.torrent",
                "hash": "abc123def456",
                "size": 2147483648,
                "progress": 0.0,
                "download_speed": 0,
                "upload_speed": 0,
                "seeds": 0,
                "peers": 0,
                "active": true,
                "download_state": "downloading",
                "download_finished": false,
                "download_present": false,
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

    private const string TorrentInfoJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {
                "hash": "abc123def456",
                "name": "ubuntu-24.04",
                "size": 2147483648,
                "peers": 10,
                "seeds": 50,
                "files": [],
                "trackers": []
            }
        }
        """;

    private const string ExportDataJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": "magnet:?xt=urn:btih:abc123def456"
        }
        """;

    [Fact]
    public async Task GetMyTorrentListAsync_WithValidResponse_ReturnsTorrentList()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(TorrentListJson);

        // Act
        TorBoxResponse<IReadOnlyList<Torrent>> result = await client.GetMyTorrentListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal("ubuntu-24.04.torrent", result.Data[0].Name);
        Assert.Equal("abc123def456", result.Data[0].Hash);
    }

    [Fact]
    public async Task GetMyTorrentListAsync_WithId_SendsIdParameter()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(TorrentListJson);

        // Act
        await client.GetMyTorrentListAsync(new GetMyListOptions { Id = 42 });

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Contains("id=42", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task CreateTorrentAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(SingleTorrentJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CreateTorrentAsync(null!));
    }

    [Fact]
    public async Task CreateTorrentAsync_WithMagnet_SendsMultipartRequest()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(SingleTorrentJson);
        CreateTorrentRequest request = new() { Magnet = "magnet:?xt=urn:btih:abc123" };

        // Act
        TorBoxResponse<Torrent> result = await client.CreateTorrentAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("torrents/createtorrent", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ControlTorrentAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(SuccessJson);
        ControlTorrentRequest request = new() { TorrentId = 1, Operation = ControlOperation.Delete };

        // Act
        TorBoxResponse result = await client.ControlTorrentAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("torrents/controltorrent", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ControlTorrentAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.ControlTorrentAsync(null!));
    }

    [Fact]
    public async Task RequestDownloadAsync_WithValidOptions_SendsGetRequest()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(DownloadJson);
        RequestDownloadOptions options = new() { TorrentId = 1 };

        // Act
        TorBoxResponse<string> result = await client.RequestDownloadAsync(options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("torrents/requestdl", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("torrent_id=1", handler.LastRequest.RequestUri!.ToString());
        Assert.Equal("https://download.torbox.app/file/abc123", result.Data);
    }

    [Fact]
    public async Task RequestDownloadAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(DownloadJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.RequestDownloadAsync(null!));
    }

    [Fact]
    public async Task EditTorrentAsync_WithValidRequest_SendsPutRequest()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(SuccessJson);
        EditTorrentRequest request = new() { TorrentId = 1, Name = "new-name" };

        // Act
        TorBoxResponse result = await client.EditTorrentAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Put, handler.LastRequest.Method);
        Assert.Contains("torrents/edittorrent", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task EditTorrentAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.EditTorrentAsync(null!));
    }

    [Fact]
    public async Task GetTorrentInfoAsync_WithHash_SendsGetRequest()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(TorrentInfoJson);

        // Act
        TorBoxResponse<TorrentInfo> result = await client.GetTorrentInfoAsync(new GetTorrentInfoOptions { Hash = "abc123def456" });

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("torrents/torrentinfo", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("hash=abc123def456", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result.Data);
        Assert.Equal("abc123def456", result.Data.Hash);
    }

    [Fact]
    public async Task GetTorrentInfoAsync_WithUseCacheLookup_IncludesInQueryString()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(TorrentInfoJson);

        // Act
        await client.GetTorrentInfoAsync(new GetTorrentInfoOptions { Hash = "abc123def456", Timeout = 60, UseCacheLookup = true });

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("hash=abc123def456", url);
        Assert.Contains("timeout=60", url);
        Assert.Contains("use_cache_lookup=true", url);
    }

    [Fact]
    public async Task GetTorrentInfoAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(TorrentInfoJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetTorrentInfoAsync(null!));
    }

    [Fact]
    public async Task GetTorrentInfoAsync_WithEmptyHash_ThrowsArgumentException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(TorrentInfoJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetTorrentInfoAsync(new GetTorrentInfoOptions { Hash = string.Empty }));
    }

    [Fact]
    public async Task GetTorrentInfoAsync_WithNullUseCacheLookup_OmitsFromQueryString()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(TorrentInfoJson);

        // Act
        await client.GetTorrentInfoAsync(new GetTorrentInfoOptions { Hash = "abc123def456" });

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.DoesNotContain("use_cache_lookup", url);
        Assert.DoesNotContain("timeout", url);
    }

    [Fact]
    public async Task GetTorrentInfoByFileAsync_WithTorrentInfoRequest_SendsPostRequest()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(TorrentInfoJson);
        TorrentInfoRequest request = new()
        {
            Hash = "abc123def456",
            Timeout = 30,
            UseCacheLookup = true,
        };

        // Act
        TorBoxResponse<TorrentInfo> result = await client.GetTorrentInfoByFileAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("torrents/torrentinfo", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result.Data);
        Assert.Equal("abc123def456", result.Data.Hash);
    }

    [Fact]
    public async Task GetTorrentInfoByFileAsync_WithFile_SendsMultipartRequest()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(TorrentInfoJson);
        TorrentInfoRequest request = new()
        {
            File = [0x01, 0x02, 0x03],
            FileName = "my-torrent.torrent",
            PeersOnly = true,
        };

        // Act
        await client.GetTorrentInfoByFileAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
    }

    [Fact]
    public async Task GetTorrentInfoByFileAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(TorrentInfoJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetTorrentInfoByFileAsync(null!));
    }

    [Fact]
    public async Task RequestDownloadAsync_WithNewOptions_IncludesInQueryString()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(DownloadJson);
        RequestDownloadOptions options = new()
        {
            TorrentId = 42,
            Token = "custom-token",
            AppendName = true,
            Redirect = false,
        };

        // Act
        await client.RequestDownloadAsync(options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("torrent_id=42", url);
        Assert.Contains("redirect=false", url);
        Assert.Contains("token=custom-token", url);
        Assert.Contains("append_name=true", url);
    }

    [Fact]
    public async Task ExportDataAsync_WithTorrentId_SendsGetRequest()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(ExportDataJson);

        // Act
        TorBoxResponse<string> result = await client.ExportDataAsync(new ExportDataOptions { TorrentId = 42 });

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("torrents/exportdata", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("torrent_id=42", handler.LastRequest.RequestUri!.ToString());
        Assert.Equal("magnet:?xt=urn:btih:abc123def456", result.Data);
    }

    [Fact]
    public async Task ExportDataAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(ExportDataJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.ExportDataAsync(null!));
    }

    // --- CheckCachedAsync ---

    [Fact]
    public async Task CheckCachedAsync_WithHashes_SendsGetRequest()
    {
        // Arrange
        string cachedJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": {}
            }
            """;
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(cachedJson);

        // Act
        await client.CheckCachedAsync(new CheckCachedOptions { Hashes = ["hash1", "hash2"] });

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("torrents/checkcached", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("hash=hash1%2Chash2", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task CheckCachedAsync_WithAllParams_IncludesInQueryString()
    {
        // Arrange
        string cachedJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": {}
            }
            """;
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(cachedJson);

        // Act
        await client.CheckCachedAsync(new CheckCachedOptions { Hashes = ["hash1"], Format = "object", ListFiles = true });

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
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CheckCachedAsync(null!));
    }

    // --- CheckCachedByPostAsync ---

    [Fact]
    public async Task CheckCachedByPostAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        string cachedJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": {}
            }
            """;
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(cachedJson);
        CheckCachedRequest request = new();

        // Act
        await client.CheckCachedByPostAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("torrents/checkcached", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task CheckCachedByPostAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CheckCachedByPostAsync(null!));
    }

    // --- AsyncCreateTorrentAsync ---

    [Fact]
    public async Task AsyncCreateTorrentAsync_WithMagnet_SendsMultipartPost()
    {
        // Arrange
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(SingleTorrentJson);
        CreateTorrentRequest request = new() { Magnet = "magnet:?xt=urn:btih:abc123" };

        // Act
        TorBoxResponse<Torrent> result = await client.AsyncCreateTorrentAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("torrents/asynccreatetorrent", handler.LastRequest.RequestUri!.ToString());
        Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task AsyncCreateTorrentAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(SingleTorrentJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.AsyncCreateTorrentAsync(null!));
    }

    [Fact]
    public async Task AsyncCreateTorrentAsync_WithNoMagnetOrFile_ThrowsArgumentException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(SingleTorrentJson);
        CreateTorrentRequest request = new();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.AsyncCreateTorrentAsync(request));
    }

    // --- MagnetToFileAsync ---

    [Fact]
    public async Task MagnetToFileAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        string magnetFileJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "base64-torrent-file-content"
            }
            """;
        (TorrentsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<TorrentsClient>(magnetFileJson);
        MagnetToFileRequest request = new() { Magnet = "magnet:?xt=urn:btih:abc123" };

        // Act
        TorBoxResponse<string> result = await client.MagnetToFileAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("torrents/magnettofile", handler.LastRequest.RequestUri!.ToString());
        Assert.Equal("base64-torrent-file-content", result.Data);
    }

    [Fact]
    public async Task MagnetToFileAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (TorrentsClient client, _) = ClientTestBase.CreateClient<TorrentsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.MagnetToFileAsync(null!));
    }
}
