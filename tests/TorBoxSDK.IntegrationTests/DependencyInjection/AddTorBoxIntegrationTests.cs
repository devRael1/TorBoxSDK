using Microsoft.Extensions.DependencyInjection;

using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Main;
using TorBoxSDK.Main.General;
using TorBoxSDK.Main.Integrations;
using TorBoxSDK.Main.Notifications;
using TorBoxSDK.Main.Queued;
using TorBoxSDK.Main.Rss;
using TorBoxSDK.Main.Stream;
using TorBoxSDK.Main.Torrents;
using TorBoxSDK.Main.Usenet;
using TorBoxSDK.Main.User;
using TorBoxSDK.Main.Vendors;
using TorBoxSDK.Main.WebDownloads;
using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorBoxSDK.IntegrationTests.DependencyInjection;

/// <summary>
/// Integration tests that verify the <see cref="TorBoxServiceCollectionExtensions.AddTorBox"/>
/// dependency-injection wiring resolves all expected SDK services.
/// </summary>
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
    [Trait("Category", "Integration")]
    public void AddTorBox_WithValidOptions_ResolvesITorBoxClient()
    {
        // Arrange — provider built in constructor

        // Act
        ITorBoxClient client = _provider.GetRequiredService<ITorBoxClient>();

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddTorBox_WithValidOptions_ResolvesAllMainApiResourceClients()
    {
        // Arrange
        IMainApiClient mainClient = _provider.GetRequiredService<IMainApiClient>();

        // Act & Assert
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
    [Trait("Category", "Integration")]
    public void AddTorBox_WithValidOptions_ResolvesSearchAndRelayClients()
    {
        // Arrange — provider built in constructor

        // Act
        ISearchApiClient searchClient = _provider.GetRequiredService<ISearchApiClient>();
        IRelayApiClient relayClient = _provider.GetRequiredService<IRelayApiClient>();

        // Assert
        Assert.NotNull(searchClient);
        Assert.NotNull(relayClient);
    }
}
