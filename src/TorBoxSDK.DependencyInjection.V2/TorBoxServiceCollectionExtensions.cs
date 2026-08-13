using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TorBoxSDK.Http;
using TorBoxSDK.Http.Handlers;
using TorBoxSDK.Internal;

namespace TorBoxSDK.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the TorBox V2 root client with an <see cref="IServiceCollection"/>.
/// </summary>
public static class TorBoxServiceCollectionExtensions
{
	private const string MainHttpClientName = "TorBoxSDK.Main";
	private const string SearchHttpClientName = "TorBoxSDK.Search";
	private const string RelayHttpClientName = "TorBoxSDK.Relay";

	private sealed class TorBoxRegistrationMarker
	{
	}

	/// <summary>
	/// Adds the TorBox V2 root client using the supplied configuration delegate.
	/// </summary>
	/// <remarks>
	/// Repeated calls compose configuration delegates in registration order. The named HTTP pipelines and
	/// <see cref="ITorBoxClient"/> registration are added once, so a later delegate can override an earlier
	/// option value without duplicating the root client registration.
	/// </remarks>
	/// <param name="services">The service collection receiving the TorBox registrations.</param>
	/// <param name="configure">The delegate that configures the TorBox API key, family URLs, and timeout.</param>
	/// <returns>The same <paramref name="services"/> instance for chaining.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configure"/> is <see langword="null"/>.</exception>
	public static IServiceCollection AddTorBox(
		this IServiceCollection services,
		Action<TorBoxClientOptions> configure)
	{
		if (services is null)
		{
			throw new ArgumentNullException(nameof(services));
		}

		if (configure is null)
		{
			throw new ArgumentNullException(nameof(configure));
		}

		services.AddOptions<TorBoxClientOptions>().Configure(configure);
		RegisterCore(services);

		return services;
	}

	/// <summary>
	/// Adds the TorBox V2 root client by binding configuration from the <c>TorBox</c> section.
	/// </summary>
	/// <remarks>
	/// Repeated calls compose configuration registrations in registration order. The named HTTP pipelines and
	/// <see cref="ITorBoxClient"/> registration are added once, so later configuration can override earlier
	/// option values without duplicating the root client registration.
	/// </remarks>
	/// <param name="services">The service collection receiving the TorBox registrations.</param>
	/// <param name="configuration">The configuration root containing the <c>TorBox</c> section.</param>
	/// <returns>The same <paramref name="services"/> instance for chaining.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configuration"/> is <see langword="null"/>.</exception>
	public static IServiceCollection AddTorBox(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		if (services is null)
		{
			throw new ArgumentNullException(nameof(services));
		}

		if (configuration is null)
		{
			throw new ArgumentNullException(nameof(configuration));
		}

		services.AddOptions<TorBoxClientOptions>().Bind(configuration.GetSection("TorBox"));
		RegisterCore(services);

		return services;
	}

	private static void RegisterCore(IServiceCollection services)
	{
		if (services.Any(static descriptor => descriptor.ServiceType == typeof(TorBoxRegistrationMarker)))
		{
			return;
		}

		services.AddSingleton<TorBoxRegistrationMarker>();
		services
			.AddHttpClient(MainHttpClientName, ConfigureMainHttpClient)
			.ConfigurePrimaryHttpMessageHandler(TorBoxHttpClientHandlerFactory.Create)
			.AddHttpMessageHandler(CreateAuthenticationHandler);
		services
			.AddHttpClient(SearchHttpClientName, ConfigureSearchHttpClient)
			.ConfigurePrimaryHttpMessageHandler(TorBoxHttpClientHandlerFactory.Create)
			.AddHttpMessageHandler(CreateAuthenticationHandler);
		services
			.AddHttpClient(RelayHttpClientName, ConfigureRelayHttpClient)
			.ConfigurePrimaryHttpMessageHandler(TorBoxHttpClientHandlerFactory.Create)
			.AddHttpMessageHandler(CreateAuthenticationHandler);
		services.TryAddTransient<ITorBoxClient>(CreateRootClient);
	}

	private static ITorBoxClient CreateRootClient(IServiceProvider serviceProvider)
	{
		ValidatedTorBoxClientOptions options = GetValidatedOptions(serviceProvider);
		IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
		HttpClient mainHttpClient = httpClientFactory.CreateClient(MainHttpClientName);
		HttpClient searchHttpClient = httpClientFactory.CreateClient(SearchHttpClientName);
		HttpClient relayHttpClient = httpClientFactory.CreateClient(RelayHttpClientName);

		return TorBoxClientFactory.Create(
			mainHttpClient,
			searchHttpClient,
			relayHttpClient,
			new TorBoxApiTransport(),
			ownsHttpClients: false);
	}

	private static void ConfigureMainHttpClient(IServiceProvider serviceProvider, HttpClient httpClient)
	{
		ValidatedTorBoxClientOptions options = GetValidatedOptions(serviceProvider);
		ConfigureHttpClient(httpClient, options.MainApiBaseUri, options.Timeout);
	}

	private static void ConfigureSearchHttpClient(IServiceProvider serviceProvider, HttpClient httpClient)
	{
		ValidatedTorBoxClientOptions options = GetValidatedOptions(serviceProvider);
		ConfigureHttpClient(httpClient, options.SearchApiBaseUri, options.Timeout);
	}

	private static void ConfigureRelayHttpClient(IServiceProvider serviceProvider, HttpClient httpClient)
	{
		ValidatedTorBoxClientOptions options = GetValidatedOptions(serviceProvider);
		ConfigureHttpClient(httpClient, options.RelayApiBaseUri, options.Timeout);
	}

	private static void ConfigureHttpClient(HttpClient httpClient, Uri baseAddress, TimeSpan timeout)
	{
		httpClient.BaseAddress = baseAddress;
		httpClient.Timeout = timeout;
	}

	private static AuthHandler CreateAuthenticationHandler(IServiceProvider serviceProvider)
	{
		ValidatedTorBoxClientOptions options = GetValidatedOptions(serviceProvider);
		return new AuthHandler(options.ApiKey);
	}

	private static ValidatedTorBoxClientOptions GetValidatedOptions(IServiceProvider serviceProvider)
	{
		TorBoxClientOptions options = serviceProvider.GetRequiredService<IOptions<TorBoxClientOptions>>().Value;
		return options.ValidateAndNormalize();
	}
}
