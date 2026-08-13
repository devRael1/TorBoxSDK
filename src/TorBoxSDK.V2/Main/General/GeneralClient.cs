using TorBoxSDK.Http;

namespace TorBoxSDK.Main.General;

internal sealed class GeneralClient : IGeneralClient
{
	internal GeneralClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
