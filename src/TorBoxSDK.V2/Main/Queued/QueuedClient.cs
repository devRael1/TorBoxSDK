using TorBoxSDK.Http;

namespace TorBoxSDK.Main.Queued;

internal sealed class QueuedClient : IQueuedClient
{
	internal QueuedClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
