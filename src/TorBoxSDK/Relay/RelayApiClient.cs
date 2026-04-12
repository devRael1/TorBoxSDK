using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Relay;

namespace TorBoxSDK.Relay;

/// <summary>
/// Default implementation of <see cref="IRelayApiClient"/> for relay-based
/// operations through the TorBox Relay API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RelayApiClient"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client configured for the Relay API.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class RelayApiClient(HttpClient httpClient) : IRelayApiClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<TorBoxResponse<RelayStatus>> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        // An empty relative URI resolves to the relay base address itself (GET /).
        using var request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
        return await TorBoxApiHelper.SendAsync<RelayStatus>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<InactiveCheckResult>> CheckForInactiveAsync(CheckInactiveOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        Guard.ThrowIfNullOrEmpty(options.AuthId, nameof(options.AuthId));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"v1/inactivecheck/torrent/{Uri.EscapeDataString(options.AuthId)}/{options.TorrentId}");
        return await TorBoxApiHelper.SendAsync<InactiveCheckResult>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }
}
