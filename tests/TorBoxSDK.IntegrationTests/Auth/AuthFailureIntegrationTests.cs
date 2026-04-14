using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.IntegrationTests.Auth;

/// <summary>
/// Integration tests that verify the SDK correctly surfaces authentication errors
/// when an invalid API key is provided.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class AuthFailureIntegrationTests
{
    [SkippableFact]
    public async Task GetMeAsync_WithInvalidApiKey_ThrowsTorBoxException()
    {
        // Skip in environments where outbound HTTP is unavailable.
        Skip.If(
            Environment.GetEnvironmentVariable("TORBOX_INTEGRATION_TESTS_ENABLED") is null
                && Environment.GetEnvironmentVariable("TORBOX_API_KEY") is null,
            "Integration test environment not configured.");

        // Arrange
        ServiceCollection services = new();
        services.AddTorBox(options => options.ApiKey = "invalid-api-key");

        await using ServiceProvider provider = services.BuildServiceProvider();
        ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();

        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act & Assert
        TorBoxException exception = await Assert.ThrowsAsync<TorBoxException>(
            () => client.Main.User.GetMeAsync(cancellationToken: cts.Token));

        Assert.Equal(TorBoxErrorCode.BadToken, exception.ErrorCode);
    }
}
