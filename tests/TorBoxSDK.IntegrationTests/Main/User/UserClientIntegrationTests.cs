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
    public async Task GetMeAsync_WithValidApiKey_ReturnsUserProfile()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<UserProfile> response = await _fixture.Client.Main.User.GetMeAsync(cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.True(response.Data.Id > 0);
        Assert.NotNull(response.Data.Email);
    }

    [SkippableFact]
    public async Task GetMeAsync_WithSettings_ReturnsSettings()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<UserProfile> response = await _fixture.Client.Main.User.GetMeAsync(
            settings: true,
            cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Settings);
    }

    [SkippableFact]
    public async Task GetConfirmationAsync_WithValidApiKey_ReturnsResponse()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<object> response = await _fixture.Client.Main.User.GetConfirmationAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetReferralDataAsync_WithValidApiKey_ReturnsReferralData()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<ReferralData> response = await _fixture.Client.Main.User.GetReferralDataAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetSubscriptionsAsync_WithValidApiKey_ReturnsSubscriptions()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<Subscription>> response = await _fixture.Client.Main.User
            .GetSubscriptionsAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetTransactionsAsync_WithValidApiKey_ReturnsTransactions()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<Transaction>> response = await _fixture.Client.Main.User
            .GetTransactionsAsync(cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [SkippableFact]
    public async Task GetSearchEnginesAsync_WithValidApiKey_ReturnsSearchEngines()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        TorBoxResponse<IReadOnlyList<SearchEngine>> response = await _fixture.Client.Main.User
            .GetSearchEnginesAsync(cancellationToken: cts.Token);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }
}
