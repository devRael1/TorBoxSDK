using TorBoxSDK.Http;

namespace TorBoxSDK.Main.Integrations;

internal sealed class IntegrationsClient : IIntegrationsClient
{
	internal IntegrationsClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
