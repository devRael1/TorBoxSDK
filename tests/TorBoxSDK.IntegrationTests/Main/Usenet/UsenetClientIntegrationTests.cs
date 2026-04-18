using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Usenet;

namespace TorBoxSDK.IntegrationTests.Main.Usenet;

/// <summary>
/// Integration tests for the Usenet resource client against the live TorBox API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class UsenetClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetMyUsenetListAsync_WithValidApiKey_ReturnsResponse()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

        // Act
        TorBoxResponse<IReadOnlyList<UsenetDownload>> response = await _fixture.Client.Main.Usenet
            .GetMyUsenetListAsync(cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [SkippableFact]
    public async Task CheckCachedAsync_WithDummyHashes_ReturnsResponse()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));
        IReadOnlyList<string> hashes = ["abc123def456"];

        // Act
        TorBoxResponse<CheckUsenetCached> response = await _fixture.Client.Main.Usenet
            .CheckCachedAsync(hashes, cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }
}
