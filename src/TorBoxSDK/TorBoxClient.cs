using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TorBoxSDK.Http;
using TorBoxSDK.Http.Handlers;
using TorBoxSDK.Http.Validation;
using TorBoxSDK.Main;
using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorBoxSDK;

/// <summary>
/// Root entry point for the TorBox SDK.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="TorBoxClient"/> aggregates the three API-family clients
/// (<see cref="IMainApiClient"/>, <see cref="ISearchApiClient"/>,
/// <see cref="IRelayApiClient"/>) into a single injectable service.
/// All sub-clients are instantiated internally and are not available
/// through the DI container. Users must use <see cref="ITorBoxClient"/>
/// to access all SDK functionality.
/// </para>
/// <para>
/// The client can be used standalone (via <c>new TorBoxClient(apiKey)</c>) or
/// through dependency injection using
/// <see cref="DependencyInjection.TorBoxServiceCollectionExtensions.AddTorBox(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action{TorBoxClientOptions})"/>.
/// </para>
/// <para>
/// In standalone mode, the client owns its <see cref="HttpClient"/> instances
/// and must be disposed when no longer needed (preferably with a <c>using</c> statement).
/// In DI mode, the container manages the lifecycle and <see cref="Dispose"/> is a no-op.
/// </para>
/// </remarks>
public sealed class TorBoxClient : ITorBoxClient
{
    private readonly HttpClient? _ownedMainHttpClient;
    private readonly HttpClient? _ownedSearchHttpClient;
    private readonly HttpClient? _ownedRelayHttpClient;
    private bool _disposed;

    /// <inheritdoc />
    public IMainApiClient Main { get; }

    /// <inheritdoc />
    public ISearchApiClient Search { get; }

    /// <inheritdoc />
    public IRelayApiClient Relay { get; }

    /// <summary>
    /// Initializes a new standalone instance of the <see cref="TorBoxClient"/> class
    /// using the specified API key and default options.
    /// </summary>
    /// <param name="apiKey">The TorBox API key used for Bearer authentication.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="apiKey"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="apiKey"/> is empty.
    /// </exception>
    public TorBoxClient(string apiKey)
        : this(new TorBoxClientOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Initializes a new standalone instance of the <see cref="TorBoxClient"/> class
    /// using the specified options.
    /// </summary>
    /// <param name="options">The SDK configuration options.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <see cref="TorBoxClientOptions.ApiKey"/> is <see langword="null"/> or empty,
    /// or when a base URL is not a valid absolute URI.
    /// </exception>
    public TorBoxClient(TorBoxClientOptions options)
    {
        Guard.ThrowIfNull(options);
        Guard.ThrowIfNullOrEmpty(options.ApiKey, nameof(options.ApiKey));
        ValidateBaseUrl(options.MainApiVersionedUrl, nameof(options.MainApiBaseUrl));
        ValidateBaseUrl(options.SearchApiBaseUrl, nameof(options.SearchApiBaseUrl));
        ValidateBaseUrl(options.RelayApiVersionedUrl, nameof(options.RelayApiBaseUrl));

        _ownedMainHttpClient = CreateHttpClient(options.ApiKey, options.MainApiVersionedUrl, options.Timeout);
        _ownedSearchHttpClient = CreateHttpClient(options.ApiKey, options.SearchApiBaseUrl, options.Timeout);
        _ownedRelayHttpClient = CreateHttpClient(options.ApiKey, options.RelayApiVersionedUrl, options.Timeout);

        Main = new MainApiClient(_ownedMainHttpClient, options.ApiKey, options.MainApiBaseUrl);
        Search = new SearchApiClient(_ownedSearchHttpClient);
        Relay = new RelayApiClient(_ownedRelayHttpClient, options.RelayApiBaseUrl);
    }

    /// <summary>
    /// Initializes a new standalone instance of the <see cref="TorBoxClient"/> class
    /// using a configuration delegate.
    /// </summary>
    /// <param name="configure">A delegate to configure <see cref="TorBoxClientOptions"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="configure"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the configured <see cref="TorBoxClientOptions.ApiKey"/> is <see langword="null"/> or empty.
    /// </exception>
    public TorBoxClient(Action<TorBoxClientOptions> configure)
        : this(ApplyConfigure(configure))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TorBoxClient"/> class
    /// using an <see cref="IHttpClientFactory"/> to create the required HTTP clients.
    /// This constructor is intended for DI usage.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory used to create named clients.</param>
    /// <param name="options">The SDK configuration options.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClientFactory"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    [ActivatorUtilitiesConstructor]
    public TorBoxClient(IHttpClientFactory httpClientFactory, IOptions<TorBoxClientOptions> options)
    {
        Guard.ThrowIfNull(httpClientFactory);
        Guard.ThrowIfNull(options);

        HttpClient mainHttpClient = httpClientFactory.CreateClient(HttpClientNames.MainApi);
        HttpClient searchHttpClient = httpClientFactory.CreateClient(HttpClientNames.SearchApi);
        HttpClient relayHttpClient = httpClientFactory.CreateClient(HttpClientNames.RelayApi);

        Main = new MainApiClient(mainHttpClient, options.Value.ApiKey, options.Value.MainApiBaseUrl);
        Search = new SearchApiClient(searchHttpClient);
        Relay = new RelayApiClient(relayHttpClient, options.Value.RelayApiBaseUrl);
    }

    /// <summary>
    /// Releases the HTTP clients owned by this instance (standalone mode only).
    /// In DI mode, this method is a no-op because the container manages the HTTP client lifecycle.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _ownedMainHttpClient?.Dispose();
        _ownedSearchHttpClient?.Dispose();
        _ownedRelayHttpClient?.Dispose();
        _disposed = true;
    }

    private static HttpClient CreateHttpClient(string apiKey, string baseUrl, TimeSpan timeout)
    {
        var authHandler = new AuthHandler(apiKey) { InnerHandler = new HttpClientHandler() };

        return new HttpClient(authHandler)
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = timeout
        };
    }

    private static TorBoxClientOptions ApplyConfigure(Action<TorBoxClientOptions> configure)
    {
        Guard.ThrowIfNull(configure);
        TorBoxClientOptions options = new();
        configure(options);
        return options;
    }

    private static void ValidateBaseUrl(string url, string paramName)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            throw new ArgumentException($"'{url}' is not a valid absolute URI.", paramName);
        }

        if (!url.EndsWith('/'))
        {
            throw new ArgumentException($"Base URL '{url}' must end with a trailing '/' for correct relative URI resolution.", paramName);
        }
    }
}
