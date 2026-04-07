using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Queued;

namespace TorBoxSDK.Main.Queued;

/// <summary>
/// Defines operations for managing queued items through the TorBox Main API.
/// </summary>
public interface IQueuedClient
{
    /// <summary>Retrieves the authenticated user's queued downloads.</summary>
    /// <param name="options">Optional query parameters for filtering and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of queued downloads.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<QueuedDownload>>> GetQueuedAsync(GetQueuedOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Performs a control operation on queued downloads.</summary>
    /// <param name="request">The control operation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ControlQueuedAsync(ControlQueuedRequest request, CancellationToken cancellationToken = default);
}
