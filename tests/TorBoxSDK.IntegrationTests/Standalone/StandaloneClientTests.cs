using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.IntegrationTests.Standalone;

/// <summary>
/// Integration tests that verify the standalone <see cref="TorBoxClient"/>
/// constructors work end-to-end against the live TorBox API without
/// requiring a DI container.
/// </summary>
[Trait("Category", "Integration")]
public sealed class StandaloneClientTests
{
    [SkippableFact]
    public async Task StandaloneClient_WithValidApiKey_CanCallApi()
    {
        // Skip when no API key is available.
        string? apiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY");
        Skip.If(string.IsNullOrEmpty(apiKey), "TORBOX_API_KEY not set.");

        // Arrange
        using var client = new TorBoxClient(apiKey!);
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

        // Act
        TorBoxResponse<UserProfile> response = await client.Main.User.GetMeAsync(
            cancellationToken: cts.Token);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [SkippableFact]
    public async Task StandaloneClient_WithOptions_CanCallApi()
    {
        // Skip when no API key is available.
        string? apiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY");
        Skip.If(string.IsNullOrEmpty(apiKey), "TORBOX_API_KEY not set.");

        // Arrange
        using var client = new TorBoxClient(new TorBoxClientOptions { ApiKey = apiKey! });
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

        // Act
        TorBoxResponse<UserProfile> response = await client.Main.User.GetMeAsync(
            cancellationToken: cts.Token);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }
}
