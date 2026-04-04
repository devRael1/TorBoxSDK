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
/// This class is registered as a scoped service by
/// <see cref="DependencyInjection.TorBoxServiceCollectionExtensions"/>.
/// </remarks>
public sealed class MainApiClient : IMainApiClient
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
    /// <param name="general">The general client.</param>
    /// <param name="torrents">The torrents client.</param>
    /// <param name="usenet">The usenet client.</param>
    /// <param name="webDownloads">The web downloads client.</param>
    /// <param name="user">The user client.</param>
    /// <param name="notifications">The notifications client.</param>
    /// <param name="rss">The RSS client.</param>
    /// <param name="stream">The stream client.</param>
    /// <param name="integrations">The integrations client.</param>
    /// <param name="vendors">The vendors client.</param>
    /// <param name="queued">The queued client.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any parameter is <see langword="null"/>.
    /// </exception>
    public MainApiClient(
        IGeneralClient general,
        ITorrentsClient torrents,
        IUsenetClient usenet,
        IWebDownloadsClient webDownloads,
        IUserClient user,
        INotificationsClient notifications,
        IRssClient rss,
        IStreamClient stream,
        IIntegrationsClient integrations,
        IVendorsClient vendors,
        IQueuedClient queued)
    {
        General = general ?? throw new ArgumentNullException(nameof(general));
        Torrents = torrents ?? throw new ArgumentNullException(nameof(torrents));
        Usenet = usenet ?? throw new ArgumentNullException(nameof(usenet));
        WebDownloads = webDownloads ?? throw new ArgumentNullException(nameof(webDownloads));
        User = user ?? throw new ArgumentNullException(nameof(user));
        Notifications = notifications ?? throw new ArgumentNullException(nameof(notifications));
        Rss = rss ?? throw new ArgumentNullException(nameof(rss));
        Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        Integrations = integrations ?? throw new ArgumentNullException(nameof(integrations));
        Vendors = vendors ?? throw new ArgumentNullException(nameof(vendors));
        Queued = queued ?? throw new ArgumentNullException(nameof(queued));
    }
}
