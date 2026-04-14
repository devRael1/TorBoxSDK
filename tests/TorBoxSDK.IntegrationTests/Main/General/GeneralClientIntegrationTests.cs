using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;

namespace TorBoxSDK.IntegrationTests.Main.General;

/// <summary>
/// Integration tests for the General resource client against the live TorBox API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class GeneralClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetUpStatusAsync_WithValidApiKey_ReturnsSuccess()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<object> response = await _fixture.Client.Main.General.GetUpStatusAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetStatsAsync_WithValidApiKey_ReturnsStats()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<Stats> response = await _fixture.Client.Main.General.GetStatsAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [SkippableFact]
    public async Task Get30DayStatsAsync_WithValidApiKey_ReturnsStats()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<DailyStats>> response = await _fixture.Client.Main.General.Get30DayStatsAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [SkippableFact]
    public async Task GetSpeedtestFilesAsync_WithValidApiKey_ReturnsResponse()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<SpeedtestServer>> response = await _fixture.Client.Main.General
            .GetSpeedtestFilesAsync(cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetChangelogsRssAsync_WithValidApiKey_ReturnsFeed()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<RssFeed> response = await _fixture.Client.Main.General.GetChangelogsRssAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [SkippableFact]
    public async Task GetChangelogsJsonAsync_WithValidApiKey_ReturnsChangelogs()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<Changelog>> response = await _fixture.Client.Main.General
            .GetChangelogsJsonAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }
}
