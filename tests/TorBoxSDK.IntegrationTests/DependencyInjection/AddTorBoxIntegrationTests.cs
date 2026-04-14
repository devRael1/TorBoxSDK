using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Main;
using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorBoxSDK.IntegrationTests.DependencyInjection;

/// <summary>
/// Integration tests that verify the <see cref="TorBoxServiceCollectionExtensions.AddTorBox"/>
/// dependency-injection wiring resolves the expected SDK services.
/// </summary>
/// <remarks>
/// Sub-clients are <see langword="internal"/> and are not registered in the DI
/// container. They are only accessible through <see cref="ITorBoxClient"/>.
/// </remarks>
[Trait("Category", "Integration")]
public sealed class AddTorBoxIntegrationTests : IDisposable
{
    private readonly ServiceProvider _provider;

    public AddTorBoxIntegrationTests()
    {
        ServiceCollection services = new();
        services.AddTorBox(options => options.ApiKey = "test-key");
        _provider = services.BuildServiceProvider();
    }

    public void Dispose() => _provider.Dispose();

    [Fact]
    public void AddTorBox_WithValidOptions_ResolvesITorBoxClient()
    {
        // Arrange — provider built in constructor

        // Act
        ITorBoxClient client = _provider.GetRequiredService<ITorBoxClient>();

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void AddTorBox_WithValidOptions_ResolvesAllMainApiResourceClients()
    {
        // Arrange — provider built in constructor

        // Act
        ITorBoxClient torBoxClient = _provider.GetRequiredService<ITorBoxClient>();
        IMainApiClient mainClient = torBoxClient.Main;

        // Assert
        Assert.NotNull(mainClient);
        Assert.NotNull(mainClient.Torrents);
        Assert.NotNull(mainClient.Usenet);
        Assert.NotNull(mainClient.WebDownloads);
        Assert.NotNull(mainClient.User);
        Assert.NotNull(mainClient.Notifications);
        Assert.NotNull(mainClient.Rss);
        Assert.NotNull(mainClient.Stream);
        Assert.NotNull(mainClient.Integrations);
        Assert.NotNull(mainClient.Vendors);
        Assert.NotNull(mainClient.Queued);
        Assert.NotNull(mainClient.General);
    }

    [Fact]
    public void AddTorBox_WithValidOptions_ResolvesSearchAndRelayClients()
    {
        // Arrange — provider built in constructor

        // Act
        ITorBoxClient torBoxClient = _provider.GetRequiredService<ITorBoxClient>();
        ISearchApiClient searchClient = torBoxClient.Search;
        IRelayApiClient relayClient = torBoxClient.Relay;

        // Assert
        Assert.NotNull(searchClient);
        Assert.NotNull(relayClient);
    }

    [Fact]
    public void AddTorBox_WithValidOptions_SubClientsNotDirectlyRegistered()
    {
        // Arrange — provider built in constructor

        // Act & Assert — sub-clients should not be resolvable directly
        Assert.Null(_provider.GetService<IMainApiClient>());
        Assert.Null(_provider.GetService<ISearchApiClient>());
        Assert.Null(_provider.GetService<IRelayApiClient>());
    }
}
