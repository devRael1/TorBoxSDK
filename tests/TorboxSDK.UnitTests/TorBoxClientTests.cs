using System.Reflection;
using TorBoxSDK;

namespace TorboxSDK.UnitTests;

public sealed class TorBoxClientTests
{
	[Fact]
	public void Constructor_WithApiKey_CreatesClientWithAllSubClients()
	{
		// Arrange
		string apiKey = "test-key";

		// Act
		using TorBoxClient client = new(apiKey);

		// Assert
		Assert.NotNull(client.Main);
		Assert.NotNull(client.Search);
		Assert.NotNull(client.Relay);
	}

	[Fact]
	public void Constructor_WithOptions_CreatesClientWithAllSubClients()
	{
		// Arrange
		TorBoxClientOptions options = new() { ApiKey = "test-key" };

		// Act
		using TorBoxClient client = new(options);

		// Assert
		Assert.NotNull(client.Main);
		Assert.NotNull(client.Search);
		Assert.NotNull(client.Relay);
	}

	[Fact]
	public void Constructor_WithConfigure_CreatesClientWithAllSubClients()
	{
		// Arrange
		Action<TorBoxClientOptions> configure = opts => opts.ApiKey = "test-key";

		// Act
		using TorBoxClient client = new(configure);

		// Assert
		Assert.NotNull(client.Main);
		Assert.NotNull(client.Search);
		Assert.NotNull(client.Relay);
	}

	[Fact]
	public void Constructor_WithNullApiKey_ThrowsArgumentNullException()
	{
		// Arrange
		string apiKey = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new TorBoxClient(apiKey));
	}

	[Fact]
	public void Constructor_WithEmptyApiKey_ThrowsArgumentException()
	{
		// Arrange
		string apiKey = string.Empty;

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new TorBoxClient(apiKey));
	}

	[Fact]
	public void Constructor_WithNullOptions_ThrowsArgumentNullException()
	{
		// Arrange
		TorBoxClientOptions options = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new TorBoxClient(options));
	}

	[Fact]
	public void Constructor_WithNullConfigure_ThrowsArgumentNullException()
	{
		// Arrange
		Action<TorBoxClientOptions> configure = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new TorBoxClient(configure));
	}

	[Fact]
	public void Constructor_WithCustomTimeout_UsesProvidedTimeout()
	{
		// Arrange
		TimeSpan expectedTimeout = TimeSpan.FromSeconds(120);
		TorBoxClientOptions options = new()
		{
			ApiKey = "test-key",
			Timeout = expectedTimeout
		};

		// Act
		using TorBoxClient client = new(options);

		// Assert — verify owned HttpClient has the configured timeout via reflection
		HttpClient? ownedHttpClient = typeof(TorBoxClient)
			.GetField("_ownedMainHttpClient", BindingFlags.Instance | BindingFlags.NonPublic)
			?.GetValue(client) as HttpClient;

		Assert.NotNull(ownedHttpClient);
		Assert.Equal(expectedTimeout, ownedHttpClient.Timeout);
	}

	[Fact]
	public void Dispose_WhenStandalone_DisposesOwnedHttpClients()
	{
		// Arrange
		TorBoxClient client = new("test-key");

		HttpClient? ownedHttpClient = typeof(TorBoxClient)
			.GetField("_ownedMainHttpClient", BindingFlags.Instance | BindingFlags.NonPublic)
			?.GetValue(client) as HttpClient;

		Assert.NotNull(ownedHttpClient);

		// Act
		client.Dispose();

		// Assert — a disposed HttpClient throws ObjectDisposedException on use
		Assert.Throws<ObjectDisposedException>(() =>
		{
			using HttpRequestMessage request = new(HttpMethod.Get, "https://example.com");
			ownedHttpClient.Send(request);
		});
	}

	[Fact]
	public void Dispose_CalledTwice_DoesNotThrow()
	{
		// Arrange
		TorBoxClient client = new("test-key");

		// Act
		client.Dispose();

		// Assert — second dispose does not throw
		Exception? exception = Record.Exception(() => client.Dispose());
		Assert.Null(exception);
	}
}
