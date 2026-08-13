using TorBoxSDK.Http;

namespace TorBoxSDK.Main.Torrents;

internal sealed class TorrentsClient : ITorrentsClient
{
	internal TorrentsClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
