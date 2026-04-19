using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK.DependencyInjection;

namespace TorBoxSDK.IntegrationTests.Helpers;

/// <summary>
/// Shared xUnit class fixture that bootstraps the TorBox SDK via dependency injection
/// and exposes the resolved <see cref="ITorBoxClient"/> for integration tests.
/// </summary>
public sealed class TorBoxIntegrationFixture : IAsyncLifetime
{
	private ServiceProvider? _serviceProvider;

	/// <summary>
	/// Gets a value indicating whether the <c>TORBOX_API_KEY</c> environment variable is set.
	/// </summary>
	public bool HasApiKey { get; }

	/// <summary>
	/// Gets the resolved <see cref="ITorBoxClient"/> from the DI container.
	/// </summary>
	// Set in constructor — guaranteed non-null before any test accesses it.
	public ITorBoxClient Client { get; private set; } = null!;

	public TorBoxIntegrationFixture()
	{
		string? apiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY");
		HasApiKey = !string.IsNullOrWhiteSpace(apiKey);

		// Use a placeholder key when the real key is unavailable so the DI
		// container still builds.  Tests that hit real APIs skip via HasApiKey.
		// Non-null because HasApiKey is true only when apiKey is non-empty.
		string effectiveKey = HasApiKey ? apiKey! : "no-api-key-set";

		ServiceCollection services = new();
		services.AddTorBox(options => options.ApiKey = effectiveKey);

		_serviceProvider = services.BuildServiceProvider();
		Client = _serviceProvider.GetRequiredService<ITorBoxClient>();
	}

	/// <inheritdoc />
	public Task InitializeAsync() => Task.CompletedTask;

	/// <inheritdoc />
	public async Task DisposeAsync()
	{
		if (_serviceProvider is not null)
		{
			await _serviceProvider.DisposeAsync();
			_serviceProvider = null;
		}
	}
}
