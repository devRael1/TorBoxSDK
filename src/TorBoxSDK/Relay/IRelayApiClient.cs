using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Relay;

namespace TorBoxSDK.Relay;

/// <summary>
/// Defines the Relay API client for relay-based operations through the TorBox Relay API.
/// </summary>
public interface IRelayApiClient
{
    /// <summary>Retrieves the current status of the TorBox relay service.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The relay status information.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<RelayStatus>> GetStatusAsync(CancellationToken ct = default);

    /// <summary>Checks whether a torrent is inactive on the relay.</summary>
    /// <param name="authId">The authentication identifier of the user.</param>
    /// <param name="torrentId">The unique identifier of the torrent to check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The inactivity check result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="authId"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="authId"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<InactiveCheckResult>> CheckForInactiveAsync(string authId, long torrentId, CancellationToken ct = default);
}
