using TorBoxSDK.Http;

namespace TorBoxSDK.Main.Rss;

internal sealed class RssClient : IRssClient
{
	internal RssClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
