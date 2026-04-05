using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;
using TorBoxSDK.Search;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Search;

public sealed class SearchApiClientTests
{
    private const string SearchResultsJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "hash": "abc123def456",
                    "name": "ubuntu-24.04-desktop-amd64.iso",
                    "size": 4700000000,
                    "seeders": 1500,
                    "leechers": 50,
                    "source": "torbox",
                    "category": "software",
                    "magnet": "magnet:?xt=urn:btih:abc123def456",
                    "last_known_seeders": 1500,
                    "last_known_leechers": 50,
                    "updated_at": "2024-01-15T12:00:00Z"
                }
            ]
        }
        """;

    private const string SingleResultJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {
                "hash": "abc123def456",
                "name": "ubuntu-24.04-desktop-amd64.iso",
                "size": 4700000000,
                "seeders": 1500,
                "leechers": 50,
                "source": "torbox",
                "category": "software",
                "magnet": "magnet:?xt=urn:btih:abc123def456",
                "last_known_seeders": 1500,
                "last_known_leechers": 50,
                "updated_at": "2024-01-15T12:00:00Z"
            }
        }
        """;

    [Fact]
    public async Task SearchTorrentsAsync_WithQuery_SendsCorrectUrl()
    {
        // Arrange
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act
        TorBoxResponse<IReadOnlyList<TorrentSearchResult>> result = await client.SearchTorrentsAsync("ubuntu");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("torrents/search/ubuntu", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task SearchTorrentsAsync_WithNullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.SearchTorrentsAsync(null!));
    }

    [Fact]
    public async Task GetTorrentByIdAsync_WithId_SendsCorrectUrl()
    {
        // Arrange
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(SingleResultJson);

        // Act
        TorBoxResponse<TorrentSearchResult> result = await client.GetTorrentByIdAsync("abc123");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("torrents/abc123", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result.Data);
        Assert.Equal("abc123def456", result.Data.Hash);
    }

    [Fact]
    public async Task SearchTorrentsAsync_WithOptions_IncludesQueryParams()
    {
        // Arrange
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);
        TorrentSearchOptions options = new()
        {
            Metadata = true,
            Season = 3,
            Episode = 12,
            CheckCache = true,
            CheckOwned = false,
            SearchUserEngines = true,
            CachedOnly = false,
        };

        // Act
        await client.SearchTorrentsAsync("ubuntu", options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("torrents/search/ubuntu", url);
        Assert.Contains("metadata=true", url);
        Assert.Contains("season=3", url);
        Assert.Contains("episode=12", url);
        Assert.Contains("check_cache=true", url);
        Assert.Contains("check_owned=false", url);
        Assert.Contains("search_user_engines=true", url);
        Assert.Contains("cached_only=false", url);
    }

    [Fact]
    public async Task SearchTorrentsAsync_WithNullOptions_OmitsQueryParams()
    {
        // Arrange
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act
        await client.SearchTorrentsAsync("ubuntu");

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("torrents/search/ubuntu", url);
        Assert.DoesNotContain("metadata=", url);
        Assert.DoesNotContain("check_cache=", url);
    }

    [Fact]
    public async Task GetTorrentByIdAsync_WithOptions_IncludesQueryParams()
    {
        // Arrange
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(SingleResultJson);
        TorrentSearchOptions options = new()
        {
            CheckCache = true,
            CachedOnly = true,
        };

        // Act
        await client.GetTorrentByIdAsync("abc123", options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("torrents/abc123", url);
        Assert.Contains("check_cache=true", url);
        Assert.Contains("cached_only=true", url);
    }

    [Fact]
    public async Task SearchUsenetAsync_WithOptions_IncludesQueryParams()
    {
        // Arrange
        string usenetResultsJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": []
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(usenetResultsJson);
        UsenetSearchOptions options = new()
        {
            Metadata = false,
            Season = 1,
            Episode = 5,
            CheckCache = true,
        };

        // Act
        await client.SearchUsenetAsync("test-query", options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("usenet/search/test-query", url);
        Assert.Contains("metadata=false", url);
        Assert.Contains("season=1", url);
        Assert.Contains("episode=5", url);
        Assert.Contains("check_cache=true", url);
    }

    [Fact]
    public async Task SearchMetaAsync_WithOptions_IncludesTypeQueryParam()
    {
        // Arrange
        string metaResultsJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": []
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(metaResultsJson);
        MetaSearchOptions options = new()
        {
            Type = "movie",
        };

        // Act
        await client.SearchMetaAsync("inception", options);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("meta/search/inception", url);
        Assert.Contains("type=movie", url);
    }

    [Fact]
    public async Task SearchMetaAsync_WithNullOptions_OmitsTypeQueryParam()
    {
        // Arrange
        string metaResultsJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": []
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(metaResultsJson);

        // Act
        await client.SearchMetaAsync("inception");

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("meta/search/inception", url);
        Assert.DoesNotContain("type=", url);
    }

    // --- GetTorrentSearchTutorialAsync ---

    [Fact]
    public async Task GetTorrentSearchTutorialAsync_SendsGetRequest()
    {
        // Arrange
        string tutorialJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "Welcome to the torrent search API."
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(tutorialJson);

        // Act
        TorBoxResponse<string> result = await client.GetTorrentSearchTutorialAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.EndsWith("torrents", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetUsenetSearchTutorialAsync ---

    [Fact]
    public async Task GetUsenetSearchTutorialAsync_SendsGetRequest()
    {
        // Arrange
        string tutorialJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "Welcome to the usenet search API."
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(tutorialJson);

        // Act
        TorBoxResponse<string> result = await client.GetUsenetSearchTutorialAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.EndsWith("usenet", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetMetaSearchTutorialAsync ---

    [Fact]
    public async Task GetMetaSearchTutorialAsync_SendsGetRequest()
    {
        // Arrange
        string tutorialJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "Welcome to the meta search API."
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(tutorialJson);

        // Act
        TorBoxResponse<string> result = await client.GetMetaSearchTutorialAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.EndsWith("meta", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetUsenetByIdAsync ---

    [Fact]
    public async Task GetUsenetByIdAsync_WithId_SendsGetRequest()
    {
        // Arrange
        string usenetSingleJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": {
                    "id": "usenet-abc123",
                    "name": "test.nzb"
                }
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(usenetSingleJson);

        // Act
        await client.GetUsenetByIdAsync("usenet-abc123");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("usenet/usenet-abc123", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetUsenetByIdAsync_WithNullId_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetUsenetByIdAsync(null!));
    }

    // --- DownloadUsenetAsync ---

    [Fact]
    public async Task DownloadUsenetAsync_WithIdAndGuid_SendsGetRequest()
    {
        // Arrange
        string downloadJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "https://download.example.com/nzb"
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(downloadJson);

        // Act
        TorBoxResponse<string> result = await client.DownloadUsenetAsync("nzb-id-123", "guid-456");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("usenet/download/nzb-id-123/guid-456", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task DownloadUsenetAsync_WithNullId_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.DownloadUsenetAsync(null!, "guid"));
    }

    [Fact]
    public async Task DownloadUsenetAsync_WithNullGuid_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.DownloadUsenetAsync("id", null!));
    }

    // --- GetMetaByIdAsync ---

    [Fact]
    public async Task GetMetaByIdAsync_WithId_SendsGetRequest()
    {
        // Arrange
        string metaSingleJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": {
                    "id": "meta-abc123",
                    "name": "Inception"
                }
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(metaSingleJson);

        // Act
        await client.GetMetaByIdAsync("meta-abc123");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("meta/meta-abc123", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetMetaByIdAsync_WithNullId_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetMetaByIdAsync(null!));
    }

    // --- SearchTorznabAsync ---

    [Fact]
    public async Task SearchTorznabAsync_WithQuery_SendsGetRequest()
    {
        // Arrange
        string torznabJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "<xml>torznab results</xml>"
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(torznabJson);

        // Act
        TorBoxResponse<string> result = await client.SearchTorznabAsync("ubuntu");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("torznab/api", url);
        Assert.Contains("t=search", url);
        Assert.Contains("q=ubuntu", url);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task SearchTorznabAsync_WithApiKey_IncludesInQueryString()
    {
        // Arrange
        string torznabJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "<xml/>"
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(torznabJson);

        // Act
        await client.SearchTorznabAsync("ubuntu", apiKey: "my-key");

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("apikey=my-key", url);
    }

    [Fact]
    public async Task SearchTorznabAsync_WithNullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.SearchTorznabAsync(null!));
    }

    // --- SearchNewznabAsync ---

    [Fact]
    public async Task SearchNewznabAsync_WithQuery_SendsGetRequest()
    {
        // Arrange
        string newznabJson = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "<xml>newznab results</xml>"
            }
            """;
        (SearchApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<SearchApiClient>(newznabJson);

        // Act
        TorBoxResponse<string> result = await client.SearchNewznabAsync("ubuntu");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("newznab/api", url);
        Assert.Contains("t=search", url);
        Assert.Contains("q=ubuntu", url);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task SearchNewznabAsync_WithNullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.SearchNewznabAsync(null!));
    }

    // --- GetTorrentByIdAsync null validation ---

    [Fact]
    public async Task GetTorrentByIdAsync_WithNullId_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SingleResultJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetTorrentByIdAsync(null!));
    }

    // --- SearchUsenetAsync null validation ---

    [Fact]
    public async Task SearchUsenetAsync_WithNullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.SearchUsenetAsync(null!));
    }

    // --- SearchMetaAsync null validation ---

    [Fact]
    public async Task SearchMetaAsync_WithNullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        (SearchApiClient client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.SearchMetaAsync(null!));
    }
}
