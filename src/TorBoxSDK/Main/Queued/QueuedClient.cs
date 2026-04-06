using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Queued;

namespace TorBoxSDK.Main.Queued;

/// <summary>
/// Default implementation of <see cref="IQueuedClient"/> for managing
/// queued items through the TorBox Main API.
/// </summary>
public sealed class QueuedClient : IQueuedClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueuedClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public QueuedClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<QueuedDownload>>> GetQueuedAsync(long? id = null, int? offset = null, int? limit = null, bool? bypassCache = null, string? type = null, CancellationToken cancellationToken = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("id", id?.ToString()),
            ("offset", offset?.ToString()),
            ("limit", limit?.ToString()),
            ("bypass_cache", bypassCache?.ToString().ToLowerInvariant()),
            ("type", type));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"queued/getqueued{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<QueuedDownload>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ControlQueuedAsync(ControlQueuedRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "queued/controlqueued")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
