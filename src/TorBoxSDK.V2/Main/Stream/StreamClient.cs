using TorBoxSDK.Http;

namespace TorBoxSDK.Main.Stream;

internal sealed class StreamClient : IStreamClient
{
	internal StreamClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
