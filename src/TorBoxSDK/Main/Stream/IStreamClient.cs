using TorBoxSDK.Models.Common;

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
    /// <param name="chosenSubtitleIndex">Optional index of the chosen subtitle track.</param>
    /// <param name="chosenAudioIndex">Optional index of the chosen audio track.</param>
    /// <param name="chosenResolutionIndex">Optional index of the chosen resolution.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stream URL or data as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> CreateStreamAsync(long id, long fileId, string type, int? chosenSubtitleIndex = null, int? chosenAudioIndex = null, int? chosenResolutionIndex = null, CancellationToken cancellationToken = default);

    /// <summary>Gets stream data for a download item.</summary>
    /// <param name="presignedToken">The presigned token for stream authentication.</param>
    /// <param name="token">The authentication token.</param>
    /// <param name="chosenSubtitleIndex">Optional index of the chosen subtitle track.</param>
    /// <param name="chosenAudioIndex">Optional index of the chosen audio track.</param>
    /// <param name="chosenResolutionIndex">Optional index of the chosen resolution.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stream data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="presignedToken"/> or <paramref name="token"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="presignedToken"/> or <paramref name="token"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> GetStreamDataAsync(string presignedToken, string token, int? chosenSubtitleIndex = null, int? chosenAudioIndex = null, int? chosenResolutionIndex = null, CancellationToken cancellationToken = default);
}
