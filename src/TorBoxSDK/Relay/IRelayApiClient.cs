using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Relay;

namespace TorBoxSDK.Relay;

/// <summary>
/// Defines the Relay API client for relay-based operations through the TorBox Relay API.
/// </summary>
public interface IRelayApiClient
{
    /// <summary>Retrieves the current status of the TorBox relay service.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The relay status information.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<RelayStatus>> GetStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>Checks whether a torrent is inactive on the relay.</summary>
    /// <param name="options">The options containing the auth ID and torrent ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The inactivity check result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the auth ID in <paramref name="options"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<InactiveCheckResult>> CheckForInactiveAsync(CheckInactiveOptions options, CancellationToken cancellationToken = default);
}
