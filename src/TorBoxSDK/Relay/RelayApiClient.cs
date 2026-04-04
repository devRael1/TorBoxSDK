using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Relay;

namespace TorBoxSDK.Relay;

/// <summary>
/// Default implementation of <see cref="IRelayApiClient"/> for relay-based
/// operations through the TorBox Relay API.
/// </summary>
public sealed class RelayApiClient : IRelayApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Relay API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public RelayApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<RelayStatus>> GetStatusAsync(CancellationToken ct = default)
    {
        // An empty relative URI resolves to the relay base address itself (GET /).
        using var request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
        return await TorBoxApiHelper.SendAsync<RelayStatus>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<InactiveCheckResult>> CheckForInactiveAsync(string authId, long torrentId, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(authId, nameof(authId));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"v1/inactivecheck/torrent/{Uri.EscapeDataString(authId)}/{torrentId}");
        return await TorBoxApiHelper.SendAsync<InactiveCheckResult>(_httpClient, request, ct).ConfigureAwait(false);
    }
}
