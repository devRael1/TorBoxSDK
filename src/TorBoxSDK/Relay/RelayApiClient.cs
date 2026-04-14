using System.Text.Json;
using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Relay;

namespace TorBoxSDK.Relay;

/// <summary>
/// Default implementation of <see cref="IRelayApiClient"/> for relay-based
/// operations through the TorBox Relay API.
/// </summary>
internal sealed class RelayApiClient : IRelayApiClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _rootUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Relay API.</param>
    /// <param name="baseUrl">The root Relay API URL (without version), used for the status endpoint.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    internal RelayApiClient(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (!string.IsNullOrEmpty(baseUrl))
        {
            _rootUrl = new Uri(baseUrl);
        }
        else if (httpClient.BaseAddress is not null)
        {
            _rootUrl = new Uri($"{httpClient.BaseAddress.Scheme}://{httpClient.BaseAddress.Host}/");
        }
        else
        {
            throw new ArgumentException("A base URL must be provided when HttpClient.BaseAddress is not set.", nameof(baseUrl));
        }
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<RelayStatus>> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        // Use the root Relay URL (without version path) as an absolute URI.
        using var request = new HttpRequestMessage(HttpMethod.Get, _rootUrl);
        using HttpResponseMessage httpResponse = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        string content = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new TorBoxException(
                $"HTTP {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}",
                TorBoxErrorCode.ServerError,
                content);
        }

        // The relay status endpoint returns a non-standard response
        // (no TorBoxResponse envelope), so we deserialize directly.
        RelayStatus status = JsonSerializer.Deserialize<RelayStatus>(content, TorBoxJsonOptions.Default)
            ?? throw new TorBoxException("Failed to deserialize relay status response.", TorBoxErrorCode.UnknownError, content);

        return new TorBoxResponse<RelayStatus>
        {
            Success = true,
            Data = status
        };
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<InactiveCheckResult>> CheckForInactiveAsync(CheckInactiveOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        Guard.ThrowIfNullOrEmpty(options.AuthId, nameof(options.AuthId));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"inactivecheck/torrent/{Uri.EscapeDataString(options.AuthId)}/{options.TorrentId}");
        return await TorBoxApiHelper.SendAsync<InactiveCheckResult>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }
}
