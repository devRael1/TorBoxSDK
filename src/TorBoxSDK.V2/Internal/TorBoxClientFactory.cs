using TorBoxSDK.Http;

namespace TorBoxSDK.Internal;

internal static class TorBoxClientFactory
{
	internal static ITorBoxClient Create(
		HttpClient mainHttpClient,
		HttpClient searchHttpClient,
		HttpClient relayHttpClient,
		ITorBoxApiTransport transport,
		bool ownsHttpClients) => new TorBoxClient(
			mainHttpClient,
			searchHttpClient,
			relayHttpClient,
			transport,
			ownsHttpClients);
}
