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
/// </para>
/// <para>
/// Register the client through dependency injection using
/// <see cref="DependencyInjection.TorBoxServiceCollectionExtensions.AddTorBox(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action{TorBoxClientOptions})"/>
/// or construct it directly for non-DI scenarios.
/// </para>
/// </remarks>
public sealed class TorBoxClient : ITorBoxClient
{
    /// <inheritdoc />
    public IMainApiClient Main { get; }

    /// <inheritdoc />
    public ISearchApiClient Search { get; }

    /// <inheritdoc />
    public IRelayApiClient Relay { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TorBoxClient"/> class.
    /// </summary>
    /// <param name="main">The Main API client.</param>
    /// <param name="search">The Search API client.</param>
    /// <param name="relay">The Relay API client.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="main"/>, <paramref name="search"/>, or <paramref name="relay"/> is <see langword="null"/>.
    /// </exception>
    public TorBoxClient(IMainApiClient main, ISearchApiClient search, IRelayApiClient relay)
    {
        Main = main ?? throw new ArgumentNullException(nameof(main));
        Search = search ?? throw new ArgumentNullException(nameof(search));
        Relay = relay ?? throw new ArgumentNullException(nameof(relay));
    }
}
