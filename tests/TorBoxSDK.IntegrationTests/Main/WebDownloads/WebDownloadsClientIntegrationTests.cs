using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.WebDownloads;

namespace TorBoxSDK.IntegrationTests.Main.WebDownloads;

/// <summary>
/// Integration tests for the WebDownloads resource client against the live TorBox API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class WebDownloadsClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetMyWebDownloadListAsync_WithValidApiKey_ReturnsResponse()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<WebDownload>> response = await _fixture.Client.Main.WebDownloads
            .GetMyWebDownloadListAsync(cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [SkippableFact]
    public async Task GetHostersAsync_WithValidApiKey_ReturnsHosters()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<Hoster>> response = await _fixture.Client.Main.WebDownloads
            .GetHostersAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data);
    }

    [SkippableFact]
    public async Task CheckCachedAsync_WithDummyHashes_ReturnsResponse()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        IReadOnlyList<string> hashes = ["abc123def456"];

        // Act
        TorBoxResponse<object> response = await _fixture.Client.Main.WebDownloads
            .CheckCachedAsync(new CheckCachedOptions { Hashes = hashes }, cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }
}
