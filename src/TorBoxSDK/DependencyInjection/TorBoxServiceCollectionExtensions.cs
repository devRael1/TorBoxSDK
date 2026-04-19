using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TorBoxSDK.Http;
using TorBoxSDK.Http.Handlers;
using TorBoxSDK.Http.Validation;

namespace TorBoxSDK.DependencyInjection;

/// <summary>
/// Extension methods for registering the TorBox SDK services with a <see cref="IServiceCollection"/>.
/// </summary>
public static class TorBoxServiceCollectionExtensions
{
	/// <summary>
	/// Adds TorBox SDK services to the specified <see cref="IServiceCollection"/>
	/// using the provided configuration delegate.
	/// </summary>
	/// <param name="services">The service collection to add services to.</param>
	/// <param name="configure">A delegate to configure <see cref="TorBoxClientOptions"/>.</param>
	/// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="services"/> or <paramref name="configure"/> is <see langword="null"/>.
	/// </exception>
	public static IServiceCollection AddTorBox(this IServiceCollection services, Action<TorBoxClientOptions> configure)
	{
		Guard.ThrowIfNull(services);
		Guard.ThrowIfNull(configure);

		services
			.AddOptions<TorBoxClientOptions>()
			.Configure(configure)
			.ValidateDataAnnotations();

		RegisterCore(services);

		return services;
	}

	/// <summary>
	/// Adds TorBox SDK services to the specified <see cref="IServiceCollection"/>
	/// using the provided <see cref="IConfiguration"/> section.
	/// </summary>
	/// <param name="services">The service collection to add services to.</param>
	/// <param name="configuration">
	/// The configuration instance. The SDK binds from the <c>TorBox</c> section.
	/// </param>
	/// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is <see langword="null"/>.
	/// </exception>
	public static IServiceCollection AddTorBox(this IServiceCollection services, IConfiguration configuration)
	{
		Guard.ThrowIfNull(services);
		Guard.ThrowIfNull(configuration);

		services
			.AddOptions<TorBoxClientOptions>()
			.Bind(configuration.GetSection("TorBox"))
			.ValidateDataAnnotations();

		RegisterCore(services);

		return services;
	}

	private static void RegisterCore(IServiceCollection services)
	{
		// Register the auth handler as transient so each HttpClient pipeline gets its own instance.
		services.AddTransient<AuthHandler>();

		// --- Named HTTP clients (sub-clients are NOT registered in the DI container) ---
		services
			.AddHttpClient(HttpClientNames.MainApi, (sp, client) =>
			{
				TorBoxClientOptions options = sp.GetRequiredService<IOptions<TorBoxClientOptions>>().Value;
				client.BaseAddress = new Uri(options.MainApiVersionedUrl);
				client.Timeout = options.Timeout;
			})
			.AddHttpMessageHandler<AuthHandler>();

		services
			.AddHttpClient(HttpClientNames.SearchApi, (sp, client) =>
			{
				TorBoxClientOptions options = sp.GetRequiredService<IOptions<TorBoxClientOptions>>().Value;
				client.BaseAddress = new Uri(options.SearchApiBaseUrl);
				client.Timeout = options.Timeout;
			})
			.AddHttpMessageHandler<AuthHandler>();

		services
			.AddHttpClient(HttpClientNames.RelayApi, (sp, client) =>
			{
				TorBoxClientOptions options = sp.GetRequiredService<IOptions<TorBoxClientOptions>>().Value;
				client.BaseAddress = new Uri(options.RelayApiVersionedUrl);
				client.Timeout = options.Timeout;
			})
			.AddHttpMessageHandler<AuthHandler>();

		// --- Only ITorBoxClient / TorBoxClient is registered in the DI container ---
		// The container resolves the (IHttpClientFactory, IOptions<TorBoxClientOptions>) constructor
		// automatically via [ActivatorUtilitiesConstructor].
		services.AddScoped<ITorBoxClient, TorBoxClient>();
	}
}
