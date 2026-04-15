using Microsoft.Extensions.Options;
using TorBoxSDK.Http;
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
/// Register the client through dependency injection using
/// <see cref="DependencyInjection.TorBoxServiceCollectionExtensions.AddTorBox(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action{TorBoxClientOptions})"/>.
/// This class cannot be instantiated directly — it is created internally
/// by the DI container.
/// </para>
/// </remarks>
public sealed class TorBoxClient : ITorBoxClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TorBoxClient"/> class
    /// using an <see cref="IHttpClientFactory"/> to create the required HTTP clients.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory used to create named clients.</param>
    /// <param name="options">The SDK configuration options.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClientFactory"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    internal TorBoxClient(IHttpClientFactory httpClientFactory, IOptions<TorBoxClientOptions> options)
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

    /// <inheritdoc />
    public IMainApiClient Main { get; }

    /// <inheritdoc />
    public ISearchApiClient Search { get; }

    /// <inheritdoc />
    public IRelayApiClient Relay { get; }
}
