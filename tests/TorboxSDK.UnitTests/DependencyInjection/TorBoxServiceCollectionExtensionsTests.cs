using Microsoft.Extensions.DependencyInjection;

using TorBoxSDK;
using TorBoxSDK.DependencyInjection;

namespace TorboxSDK.UnitTests.DependencyInjection;

public sealed class TorBoxServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTorBox_WithValidConfig_RegistersAllServices()
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
    public void AddTorBox_WithNullConfigure_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddTorBox((Action<TorBoxClientOptions>)null!));
    }
}
