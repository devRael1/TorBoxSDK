using TorBoxSDK.Http;
using TorBoxSDK.Http.Validation;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Queued;

namespace TorBoxSDK.Main.Queued;

/// <summary>
/// Default implementation of <see cref="IQueuedClient"/> for managing
/// queued items through the TorBox Main API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="QueuedClient"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client configured for the Main API.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class QueuedClient(HttpClient httpClient) : IQueuedClient
{
	private readonly HttpClient _httpClient = Guard.ThrowIfNull(httpClient);

	/// <inheritdoc />
	public async Task<TorBoxResponse<IReadOnlyList<QueuedDownload>>> GetQueuedAsync(GetQueuedOptions? options = null, CancellationToken cancellationToken = default)
	{
		string query = TorBoxApiHelper.BuildQuery(
			("id", options?.Id?.ToString()),
			("offset", options?.Offset?.ToString()),
			("limit", options?.Limit?.ToString()),
			("bypass_cache", options?.BypassCache?.ToString().ToLowerInvariant()),
			("type", options?.Type));

		using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"queued/getqueued{query}");
		return await TorBoxApiHelper.SendAsync<IReadOnlyList<QueuedDownload>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public async Task<TorBoxResponse> ControlQueuedAsync(ControlQueuedRequest request, CancellationToken cancellationToken = default)
	{
		Guard.ThrowIfNull(request);

		using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "queued/controlqueued")
		{
			Content = TorBoxApiHelper.JsonContent(request),
		};
		return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
	}
}
