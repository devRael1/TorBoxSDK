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
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The stream URL or data as a string.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is null or empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> CreateStreamAsync(long id, long fileId, string type, CancellationToken ct = default);

    /// <summary>Gets stream data for a download item.</summary>
    /// <param name="id">The identifier of the download.</param>
    /// <param name="fileId">The identifier of the file within the download.</param>
    /// <param name="type">The type of download (e.g., torrent, usenet).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The stream data.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is null or empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> GetStreamDataAsync(long id, long fileId, string type, CancellationToken ct = default);
}
