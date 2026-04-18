using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Stream;

namespace TorBoxSDK.Main.Stream;

/// <summary>
/// Defines operations for streaming content through the TorBox Main API.
/// </summary>
public interface IStreamClient
{
    /// <summary>Creates a stream for a download item.</summary>
    /// <param name="id">The identifier of the download.</param>
    /// <param name="fileId">The identifier of the file within the download.</param>
    /// <param name="type">The type of download (e.g., torrent, usenet).</param>
    /// <param name="options">Optional stream creation options for track indices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stream URL or data as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> CreateStreamAsync(long id, long fileId, string type, CreateStreamOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Gets stream data for a download item.</summary>
    /// <param name="presignedToken">The presigned token for stream authentication.</param>
    /// <param name="token">The authentication token.</param>
    /// <param name="options">Optional stream data options for track indices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stream data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="presignedToken"/> or <paramref name="token"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="presignedToken"/> or <paramref name="token"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<StreamData>> GetStreamDataAsync(string presignedToken, string token, GetStreamDataOptions? options = null, CancellationToken cancellationToken = default);
}
