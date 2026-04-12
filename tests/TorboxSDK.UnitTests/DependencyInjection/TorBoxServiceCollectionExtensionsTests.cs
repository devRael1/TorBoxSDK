using Microsoft.Extensions.DependencyInjection;

using TorBoxSDK;
using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Main;
using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorboxSDK.UnitTests.DependencyInjection;

public sealed class TorBoxServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTorBox_WithValidConfig_RegistersITorBoxClient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTorBox(options =>
        {
            options.ApiKey = "test-key";
        });

        // Act
        using ServiceProvider provider = services.BuildServiceProvider();
        ITorBoxClient? client = provider.GetService<ITorBoxClient>();

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(client.Main);
        Assert.NotNull(client.Search);
        Assert.NotNull(client.Relay);
    }

    [Fact]
    public void AddTorBox_WithValidConfig_SubClientsNotDirectlyRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTorBox(options =>
        {
            options.ApiKey = "test-key";
        });

        // Act
        using ServiceProvider provider = services.BuildServiceProvider();

        // Assert — sub-clients should not be resolvable directly from the DI container
        Assert.Null(provider.GetService<IMainApiClient>());
        Assert.Null(provider.GetService<ISearchApiClient>());
        Assert.Null(provider.GetService<IRelayApiClient>());
    }

    [Fact]
    public void AddTorBox_WithValidConfig_AllMainResourceClientsAccessibleThroughITorBoxClient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTorBox(options =>
        {
            options.ApiKey = "test-key";
        });

        // Act
        using ServiceProvider provider = services.BuildServiceProvider();
        ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();

        // Assert
        Assert.NotNull(client.Main.General);
        Assert.NotNull(client.Main.Torrents);
        Assert.NotNull(client.Main.Usenet);
        Assert.NotNull(client.Main.WebDownloads);
        Assert.NotNull(client.Main.User);
        Assert.NotNull(client.Main.Notifications);
        Assert.NotNull(client.Main.Rss);
        Assert.NotNull(client.Main.Stream);
        Assert.NotNull(client.Main.Integrations);
        Assert.NotNull(client.Main.Vendors);
        Assert.NotNull(client.Main.Queued);
    }

    [Fact]
    public void AddTorBox_WithNullConfigure_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddTorBox((Action<TorBoxClientOptions>)null!));
    }
}
