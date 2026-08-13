using TorBoxSDK.Http;

namespace TorBoxSDK.Main.Usenet;

internal sealed class UsenetClient : IUsenetClient
{
	internal UsenetClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
