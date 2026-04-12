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

namespace TorBoxSDK.Main;

/// <summary>
/// Default implementation of <see cref="IMainApiClient"/> that aggregates
/// all Main API resource clients.
/// </summary>
/// <remarks>
/// <para>
/// All resource clients are instantiated internally using the shared
/// <see cref="HttpClient"/> configured for the Main API. They are not
/// registered in the DI container and are only accessible through
/// <see cref="ITorBoxClient.Main"/>.
/// </para>
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="MainApiClient"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client configured for the Main API.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class MainApiClient(HttpClient httpClient) : IMainApiClient
{
    /// <inheritdoc />
    public IGeneralClient General { get; } = new GeneralClient(httpClient ?? throw new ArgumentNullException(nameof(httpClient)));

    /// <inheritdoc />
    public ITorrentsClient Torrents { get; } = new TorrentsClient(httpClient);

    /// <inheritdoc />
    public IUsenetClient Usenet { get; } = new UsenetClient(httpClient);

    /// <inheritdoc />
    public IWebDownloadsClient WebDownloads { get; } = new WebDownloadsClient(httpClient);

    /// <inheritdoc />
    public IUserClient User { get; } = new UserClient(httpClient);

    /// <inheritdoc />
    public INotificationsClient Notifications { get; } = new NotificationsClient(httpClient);

    /// <inheritdoc />
    public IRssClient Rss { get; } = new RssClient(httpClient);

    /// <inheritdoc />
    public IStreamClient Stream { get; } = new StreamClient(httpClient);

    /// <inheritdoc />
    public IIntegrationsClient Integrations { get; } = new IntegrationsClient(httpClient);

    /// <inheritdoc />
    public IVendorsClient Vendors { get; } = new VendorsClient(httpClient);

    /// <inheritdoc />
    public IQueuedClient Queued { get; } = new QueuedClient(httpClient);
}
