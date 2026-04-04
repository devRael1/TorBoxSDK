using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.IntegrationTests.Main.User;

/// <summary>
/// Integration tests for the User resource client against the live TorBox API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class UserClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    [Trait("Category", "Integration")]
    public async Task GetMeAsync_WithValidApiKey_ReturnsUserProfile()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<UserProfile> response = await _fixture.Client.Main.User.GetMeAsync(ct: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.True(response.Data.Id > 0);
        Assert.NotNull(response.Data.Email);
    }

    [SkippableFact]
    [Trait("Category", "Integration")]
    public async Task GetMeAsync_WithSettings_ReturnsSettings()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<UserProfile> response = await _fixture.Client.Main.User.GetMeAsync(
            settings: true,
            ct: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Settings);
    }
}
