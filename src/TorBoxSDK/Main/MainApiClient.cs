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
/// <remarks>
/// Initializes a new instance of the <see cref="MainApiClient"/> class.
/// </remarks>
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
public sealed class MainApiClient(
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
    IQueuedClient queued) : IMainApiClient
{
    /// <inheritdoc />
    public IGeneralClient General { get; } = general ?? throw new ArgumentNullException(nameof(general));

    /// <inheritdoc />
    public ITorrentsClient Torrents { get; } = torrents ?? throw new ArgumentNullException(nameof(torrents));

    /// <inheritdoc />
    public IUsenetClient Usenet { get; } = usenet ?? throw new ArgumentNullException(nameof(usenet));

    /// <inheritdoc />
    public IWebDownloadsClient WebDownloads { get; } = webDownloads ?? throw new ArgumentNullException(nameof(webDownloads));

    /// <inheritdoc />
    public IUserClient User { get; } = user ?? throw new ArgumentNullException(nameof(user));

    /// <inheritdoc />
    public INotificationsClient Notifications { get; } = notifications ?? throw new ArgumentNullException(nameof(notifications));

    /// <inheritdoc />
    public IRssClient Rss { get; } = rss ?? throw new ArgumentNullException(nameof(rss));

    /// <inheritdoc />
    public IStreamClient Stream { get; } = stream ?? throw new ArgumentNullException(nameof(stream));

    /// <inheritdoc />
    public IIntegrationsClient Integrations { get; } = integrations ?? throw new ArgumentNullException(nameof(integrations));

    /// <inheritdoc />
    public IVendorsClient Vendors { get; } = vendors ?? throw new ArgumentNullException(nameof(vendors));

    /// <inheritdoc />
    public IQueuedClient Queued { get; } = queued ?? throw new ArgumentNullException(nameof(queued));
}
