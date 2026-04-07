using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;

namespace TorBoxSDK.IntegrationTests.Main.Integrations;

/// <summary>
/// Integration tests for the Integrations resource client against the live TorBox API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class IntegrationsClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetOAuthMeAsync_WithValidApiKey_ReturnsConnectedProviders()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyDictionary<string, bool>> response = await _fixture.Client.Main.Integrations
            .GetOAuthMeAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetJobsAsync_WithValidApiKey_ReturnsJobs()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<IntegrationJob>> response = await _fixture.Client.Main.Integrations
            .GetJobsAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetLinkedDiscordRolesAsync_WithValidApiKey_ReturnsResponse()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");
        string? discordToken = Environment.GetEnvironmentVariable("TORBOX_DISCORD_TOKEN");
        Skip.If(string.IsNullOrEmpty(discordToken), "TORBOX_DISCORD_TOKEN not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<object> response = await _fixture.Client.Main.Integrations
            .GetLinkedDiscordRolesAsync(
                new LinkedRolesRequest { DiscordToken = discordToken },
                cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }
}
