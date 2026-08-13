using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Http;

internal interface ITorBoxApiTransport
{
	Task<TorBoxResponse<T>> SendAsync<T>(
		HttpClient httpClient,
		HttpRequestMessage request,
		CancellationToken cancellationToken);

	Task<TorBoxResponse> SendAsync(
		HttpClient httpClient,
		HttpRequestMessage request,
		CancellationToken cancellationToken);

	Task<TorBoxStreamResponse> SendStreamAsync(
		HttpClient httpClient,
		HttpRequestMessage request,
		CancellationToken cancellationToken);
}
