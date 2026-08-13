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
/// Defines the Main API family and its resource clients.
/// </summary>
public interface IMainApiClient
{
	/// <summary>
	/// Gets the general Main API resource.
	/// </summary>
	IGeneralClient General { get; }

	/// <summary>
	/// Gets the torrent resource.
	/// </summary>
	ITorrentsClient Torrents { get; }

	/// <summary>
	/// Gets the Usenet resource.
	/// </summary>
	IUsenetClient Usenet { get; }

	/// <summary>
	/// Gets the web-download resource.
	/// </summary>
	IWebDownloadsClient WebDownloads { get; }

	/// <summary>
	/// Gets the user resource.
	/// </summary>
	IUserClient User { get; }

	/// <summary>
	/// Gets the notification resource.
	/// </summary>
	INotificationsClient Notifications { get; }

	/// <summary>
	/// Gets the RSS resource.
	/// </summary>
	IRssClient Rss { get; }

	/// <summary>
	/// Gets the streaming resource.
	/// </summary>
	IStreamClient Stream { get; }

	/// <summary>
	/// Gets the integrations resource.
	/// </summary>
	IIntegrationsClient Integrations { get; }

	/// <summary>
	/// Gets the vendors resource.
	/// </summary>
	IVendorsClient Vendors { get; }

	/// <summary>
	/// Gets the queued-download resource.
	/// </summary>
	IQueuedClient Queued { get; }
}
