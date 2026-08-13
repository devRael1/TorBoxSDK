using TorBoxSDK.Http;

namespace TorBoxSDK.Main.Vendors;

internal sealed class VendorsClient : IVendorsClient
{
	internal VendorsClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
