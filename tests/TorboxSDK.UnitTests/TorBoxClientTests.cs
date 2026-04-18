using TorBoxSDK;

namespace TorboxSDK.UnitTests;

public sealed class TorBoxClientTests
{
    [Fact]
    public void Constructor_WithApiKey_CreatesClientWithAllSubClients()
    {
        // Arrange & Act
        using var client = new TorBoxClient("test-key");

        // Assert
        Assert.NotNull(client.Main);
        Assert.NotNull(client.Search);
        Assert.NotNull(client.Relay);
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClientWithAllSubClients()
    {
        // Arrange
        var options = new TorBoxClientOptions { ApiKey = "test-key" };

        // Act
        using var client = new TorBoxClient(options);

        // Assert
        Assert.NotNull(client.Main);
        Assert.NotNull(client.Search);
        Assert.NotNull(client.Relay);
    }

    [Fact]
    public void Constructor_WithConfigure_CreatesClientWithAllSubClients()
    {
        // Arrange & Act
        using var client = new TorBoxClient(opts => opts.ApiKey = "test-key");

        // Assert
        Assert.NotNull(client.Main);
        Assert.NotNull(client.Search);
        Assert.NotNull(client.Relay);
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TorBoxClient((string)null!));
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new TorBoxClient(string.Empty));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TorBoxClient((TorBoxClientOptions)null!));
    }

    [Fact]
    public void Constructor_WithNullConfigure_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TorBoxClient((Action<TorBoxClientOptions>)null!));
    }

    [Fact]
    public void Constructor_WithCustomTimeout_UsesProvidedTimeout()
    {
        // Arrange
        var options = new TorBoxClientOptions
        {
            ApiKey = "test-key",
            Timeout = TimeSpan.FromSeconds(120)
        };

        // Act
        using var client = new TorBoxClient(options);

        // Assert
        Assert.NotNull(client.Main);
        Assert.NotNull(client.Search);
        Assert.NotNull(client.Relay);
    }

    [Fact]
    public void Dispose_WhenStandalone_DisposesOwnedClients()
    {
        // Arrange
        var client = new TorBoxClient("test-key");

        // Act
        client.Dispose();

        // Assert — sub-client references remain valid (they are not nulled out)
        Assert.NotNull(client.Main);
        Assert.NotNull(client.Search);
        Assert.NotNull(client.Relay);
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        // Arrange
        var client = new TorBoxClient("test-key");

        // Act & Assert — no exception on double dispose
        client.Dispose();
        client.Dispose();
    }
}
