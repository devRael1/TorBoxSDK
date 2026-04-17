using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Relay;

namespace TorBoxSDK.IntegrationTests.Relay;

/// <summary>
/// Integration tests for the Relay API client against the live TorBox Relay API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class RelayApiClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetStatusAsync_WithValidApiKey_ReturnsRelayStatus()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

        // Act
        TorBoxResponse<RelayStatus> response = await _fixture.Client.Relay.GetStatusAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }
}
