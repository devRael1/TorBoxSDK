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
/// Defines the Main API client, which provides access to all resource clients
/// for the TorBox Main API surface.
/// </summary>
public interface IMainApiClient
{
    /// <summary>
    /// Gets the client for torrent-related operations.
    /// </summary>
    ITorrentsClient Torrents { get; }

    /// <summary>
    /// Gets the client for usenet-related operations.
    /// </summary>
    IUsenetClient Usenet { get; }

    /// <summary>
    /// Gets the client for web download operations.
    /// </summary>
    IWebDownloadsClient WebDownloads { get; }

    /// <summary>
    /// Gets the client for user account operations.
    /// </summary>
    IUserClient User { get; }

    /// <summary>
    /// Gets the client for notification operations.
    /// </summary>
    INotificationsClient Notifications { get; }

    /// <summary>
    /// Gets the client for RSS feed operations.
    /// </summary>
    IRssClient Rss { get; }

    /// <summary>
    /// Gets the client for streaming operations.
    /// </summary>
    IStreamClient Stream { get; }

    /// <summary>
    /// Gets the client for third-party integration operations.
    /// </summary>
    IIntegrationsClient Integrations { get; }

    /// <summary>
    /// Gets the client for vendor-related operations.
    /// </summary>
    IVendorsClient Vendors { get; }

    /// <summary>
    /// Gets the client for queued item operations.
    /// </summary>
    IQueuedClient Queued { get; }
}
