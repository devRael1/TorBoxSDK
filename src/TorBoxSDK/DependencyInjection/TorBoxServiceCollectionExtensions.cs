using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TorBoxSDK.Http;
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

namespace TorBoxSDK.DependencyInjection;

/// <summary>
/// Extension methods for registering the TorBox SDK services
/// with a <see cref="IServiceCollection"/>.
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
    public static IServiceCollection AddTorBox(
        this IServiceCollection services,
        Action<TorBoxClientOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

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
    public static IServiceCollection AddTorBox(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

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

        // --- Main API resource clients (all share the Main API base URL) ---
        AddMainApiClient<IGeneralClient, GeneralClient>(services);
        AddMainApiClient<ITorrentsClient, TorrentsClient>(services);
        AddMainApiClient<IUsenetClient, UsenetClient>(services);
        AddMainApiClient<IWebDownloadsClient, WebDownloadsClient>(services);
        AddMainApiClient<IUserClient, UserClient>(services);
        AddMainApiClient<INotificationsClient, NotificationsClient>(services);
        AddMainApiClient<IRssClient, RssClient>(services);
        AddMainApiClient<IStreamClient, StreamClient>(services);
        AddMainApiClient<IIntegrationsClient, IntegrationsClient>(services);
        AddMainApiClient<IVendorsClient, VendorsClient>(services);
        AddMainApiClient<IQueuedClient, QueuedClient>(services);

        // --- Search API client ---
        services
            .AddHttpClient<ISearchApiClient, SearchApiClient>((sp, client) =>
            {
                TorBoxClientOptions options = sp.GetRequiredService<IOptions<TorBoxClientOptions>>().Value;
                client.BaseAddress = new Uri(options.SearchApiBaseUrl);
                client.Timeout = options.Timeout;
            })
            .AddHttpMessageHandler<AuthHandler>();

        // --- Relay API client ---
        services
            .AddHttpClient<IRelayApiClient, RelayApiClient>((sp, client) =>
            {
                TorBoxClientOptions options = sp.GetRequiredService<IOptions<TorBoxClientOptions>>().Value;
                client.BaseAddress = new Uri(options.RelayApiBaseUrl);
                client.Timeout = options.Timeout;
            })
            .AddHttpMessageHandler<AuthHandler>();

        // --- Aggregate clients ---
        services.AddScoped<IMainApiClient, MainApiClient>();
        services.AddScoped<ITorBoxClient, TorBoxClient>();
    }

    private static void AddMainApiClient<TClient, TImplementation>(IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
    {
        services
            .AddHttpClient<TClient, TImplementation>((sp, client) =>
            {
                TorBoxClientOptions options = sp.GetRequiredService<IOptions<TorBoxClientOptions>>().Value;
                client.BaseAddress = new Uri(options.MainApiBaseUrl);
                client.Timeout = options.Timeout;
            })
            .AddHttpMessageHandler<AuthHandler>();
    }
}
