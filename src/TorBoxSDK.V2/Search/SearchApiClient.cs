using TorBoxSDK.Http;

namespace TorBoxSDK.Search;

internal sealed class SearchApiClient : ISearchApiClient
{
	internal SearchApiClient(HttpClient httpClient, ITorBoxApiTransport transport)
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
