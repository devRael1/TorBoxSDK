using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.IntegrationTests.Main.Notifications;

/// <summary>
/// Integration tests for the Notifications resource client against the live TorBox API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class NotificationsClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetMyNotificationsAsync_WithValidApiKey_ReturnsResponse()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<Notification>> response = await _fixture.Client.Main.Notifications
            .GetMyNotificationsAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetNotificationRssAsync_WithValidApiKey_ReturnsRssUrl()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<string> response = await _fixture.Client.Main.Notifications
            .GetNotificationRssAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetIntercomHashAsync_WithValidApiKey_ReturnsHash()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        TorBoxResponse<UserProfile> meResponse = await _fixture.Client.Main.User
            .GetMeAsync(cancellationToken: cts.Token);
        string? authId = meResponse.Data?.AuthId;
        string? email = meResponse.Data?.Email;
        Skip.If(string.IsNullOrEmpty(authId) || string.IsNullOrEmpty(email),
            "User auth_id or email not available from GetMeAsync.");

        // Act
        TorBoxResponse<IntercomHash> response = await _fixture.Client.Main.Notifications
            .GetIntercomHashAsync(authId, email, cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }
}
