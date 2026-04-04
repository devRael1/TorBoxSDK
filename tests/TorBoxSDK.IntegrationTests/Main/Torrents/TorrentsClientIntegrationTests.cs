using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.IntegrationTests.Main.Torrents;

/// <summary>
/// Integration tests for the Torrents resource client against the live TorBox API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class TorrentsClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    [Trait("Category", "Integration")]
    public async Task GetMyTorrentListAsync_WithValidApiKey_ReturnsResponse()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<Torrent>> response = await _fixture.Client.Main.Torrents
            .GetMyTorrentListAsync(ct: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }
}
