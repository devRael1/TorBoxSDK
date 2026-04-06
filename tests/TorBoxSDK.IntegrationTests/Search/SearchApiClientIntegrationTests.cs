using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;

namespace TorBoxSDK.IntegrationTests.Search;

/// <summary>
/// Integration tests for the Search API client against the live TorBox Search API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class SearchApiClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetTorrentSearchTutorialAsync_WithValidApiKey_ReturnsTutorial()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<string> response = await _fixture.Client.Search
            .GetTorrentSearchTutorialAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task SearchTorrentsAsync_WithValidQuery_ReturnsResults()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<TorrentSearchResponse> response = await _fixture.Client.Search
            .SearchTorrentsAsync("ubuntu", cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data.Torrents);
    }

    [SkippableFact]
    public async Task GetUsenetSearchTutorialAsync_WithValidApiKey_ReturnsTutorial()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<string> response = await _fixture.Client.Search
            .GetUsenetSearchTutorialAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task SearchUsenetAsync_WithValidQuery_ReturnsResults()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<UsenetSearchResponse> response = await _fixture.Client.Search
            .SearchUsenetAsync("ubuntu", cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [SkippableFact]
    public async Task GetMetaSearchTutorialAsync_WithValidApiKey_ReturnsTutorial()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<string> response = await _fixture.Client.Search
            .GetMetaSearchTutorialAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task SearchMetaAsync_WithValidQuery_ReturnsResults()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<MetaSearchResult>> response = await _fixture.Client.Search
            .SearchMetaAsync("inception", cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [SkippableFact]
    public async Task SearchTorznabAsync_WithValidQuery_ReturnsResults()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<string> response = await _fixture.Client.Search
            .SearchTorznabAsync("ubuntu", cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task SearchNewznabAsync_WithValidQuery_ReturnsResults()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<string> response = await _fixture.Client.Search
            .SearchNewznabAsync("ubuntu", cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }
}
