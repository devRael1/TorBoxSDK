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
/// All resource clients are instantiated internally using the shared
/// <see cref="HttpClient"/> configured for the Main API. They are not
/// registered in the DI container and are only accessible through
/// <see cref="ITorBoxClient.Main"/>.
/// </remarks>
internal sealed class MainApiClient : IMainApiClient
{
    /// <inheritdoc />
    public IGeneralClient General { get; }

    /// <inheritdoc />
    public ITorrentsClient Torrents { get; }

    /// <inheritdoc />
    public IUsenetClient Usenet { get; }

    /// <inheritdoc />
    public IWebDownloadsClient WebDownloads { get; }

    /// <inheritdoc />
    public IUserClient User { get; }

    /// <inheritdoc />
    public INotificationsClient Notifications { get; }

    /// <inheritdoc />
    public IRssClient Rss { get; }

    /// <inheritdoc />
    public IStreamClient Stream { get; }

    /// <inheritdoc />
    public IIntegrationsClient Integrations { get; }

    /// <inheritdoc />
    public IVendorsClient Vendors { get; }

    /// <inheritdoc />
    public IQueuedClient Queued { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <param name="apiKey">The TorBox API key, used by sub-clients that require it as a query parameter.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    internal MainApiClient(HttpClient httpClient, string apiKey)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        General = new GeneralClient(httpClient);
        Torrents = new TorrentsClient(httpClient);
        Usenet = new UsenetClient(httpClient);
        WebDownloads = new WebDownloadsClient(httpClient);
        User = new UserClient(httpClient);
        Notifications = new NotificationsClient(httpClient, apiKey);
        Rss = new RssClient(httpClient);
        Stream = new StreamClient(httpClient);
        Integrations = new IntegrationsClient(httpClient);
        Vendors = new VendorsClient(httpClient);
        Queued = new QueuedClient(httpClient);
    }
}
