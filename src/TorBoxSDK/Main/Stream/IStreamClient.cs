using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Stream;

namespace TorBoxSDK.Main.Stream;

/// <summary>
/// Defines operations for streaming content through the TorBox Main API.
/// </summary>
public interface IStreamClient
{
    /// <summary>Creates a stream for a download item.</summary>
    /// <param name="options">The stream creation options containing the item ID, file ID, type, and optional track indices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stream URL or data as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the type in <paramref name="options"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> CreateStreamAsync(CreateStreamOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets stream data for a download item.</summary>
    /// <param name="options">The stream data options containing the presigned token, token, and optional track indices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stream data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the presigned token or token in <paramref name="options"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> GetStreamDataAsync(GetStreamDataOptions options, CancellationToken cancellationToken = default);
}
