using TorBoxSDK.Http;

namespace TorBoxSDK.Main.WebDownloads;

internal sealed class WebDownloadsClient : IWebDownloadsClient
{
	internal WebDownloadsClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
