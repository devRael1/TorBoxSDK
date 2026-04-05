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
        var (client, handler) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

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
        var (client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.SearchTorrentsAsync(null!));
    }

    [Fact]
    public async Task GetTorrentByIdAsync_WithId_SendsCorrectUrl()
    {
        // Arrange
        var (client, handler) = ClientTestBase.CreateClient<SearchApiClient>(SingleResultJson);

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
        var (client, handler) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);
        var options = new TorrentSearchOptions
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
        var (client, handler) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

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
        var (client, handler) = ClientTestBase.CreateClient<SearchApiClient>(SingleResultJson);
        var options = new TorrentSearchOptions
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
        var (client, handler) = ClientTestBase.CreateClient<SearchApiClient>(usenetResultsJson);
        var options = new UsenetSearchOptions
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
        var (client, handler) = ClientTestBase.CreateClient<SearchApiClient>(metaResultsJson);
        var options = new MetaSearchOptions
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
        var (client, handler) = ClientTestBase.CreateClient<SearchApiClient>(metaResultsJson);

        // Act
        await client.SearchMetaAsync("inception");

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("meta/search/inception", url);
        Assert.DoesNotContain("type=", url);
    }
}
