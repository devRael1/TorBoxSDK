using TorBoxSDK.Http;
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

internal sealed class MainApiClient : IMainApiClient
{
	internal MainApiClient(HttpClient httpClient, ITorBoxApiTransport transport)
	{
		if (httpClient is null)
		{
			throw new ArgumentNullException(nameof(httpClient));
		}

		if (transport is null)
		{
			throw new ArgumentNullException(nameof(transport));
		}

		General = new GeneralClient(httpClient, transport);
		Torrents = new TorrentsClient(httpClient, transport);
		Usenet = new UsenetClient(httpClient, transport);
		WebDownloads = new WebDownloadsClient(httpClient, transport);
		User = new UserClient(httpClient, transport);
		Notifications = new NotificationsClient(httpClient, transport);
		Rss = new RssClient(httpClient, transport);
		Stream = new StreamClient(httpClient, transport);
		Integrations = new IntegrationsClient(httpClient, transport);
		Vendors = new VendorsClient(httpClient, transport);
		Queued = new QueuedClient(httpClient, transport);
	}

	public IGeneralClient General { get; }

	public ITorrentsClient Torrents { get; }

	public IUsenetClient Usenet { get; }

	public IWebDownloadsClient WebDownloads { get; }

	public IUserClient User { get; }

	public INotificationsClient Notifications { get; }

	public IRssClient Rss { get; }

	public IStreamClient Stream { get; }

	public IIntegrationsClient Integrations { get; }

	public IVendorsClient Vendors { get; }

	public IQueuedClient Queued { get; }
}
