using TorBoxSDK.Http;

namespace TorBoxSDK.Relay;

internal sealed class RelayApiClient : IRelayApiClient
{
	internal RelayApiClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
