using TorBoxSDK.Http;

namespace TorBoxSDK.Main.Notifications;

internal sealed class NotificationsClient : INotificationsClient
{
	internal NotificationsClient(HttpClient httpClient, ITorBoxApiTransport transport)
	{
		if (httpClient is null)
		{
			throw new ArgumentNullException(nameof(httpClient));
		}

		if (transport is null)
		{
			throw new ArgumentNullException(nameof(transport));
		}
	}
}
