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
    public async Task SearchTorrentsAsync_WithNullQuery_ThrowsArgumentException()
    {
        // Arrange
        var (client, _) = ClientTestBase.CreateClient<SearchApiClient>(SearchResultsJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.SearchTorrentsAsync(null!));
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
}
