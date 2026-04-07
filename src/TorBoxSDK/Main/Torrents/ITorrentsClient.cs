using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Main.Torrents;

/// <summary>
/// Defines operations for managing torrents through the TorBox Main API.
/// </summary>
public interface ITorrentsClient
{
    /// <summary>Creates a new torrent download from a magnet URI or torrent file.</summary>
    /// <param name="request">The torrent creation request containing the magnet URI or file bytes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created torrent details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<Torrent>> CreateTorrentAsync(CreateTorrentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Performs a control operation on a torrent (delete, pause, resume, etc.).</summary>
    /// <param name="request">The control operation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ControlTorrentAsync(ControlTorrentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Requests a download link for a torrent.</summary>
    /// <param name="options">The download request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The download URL as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> RequestDownloadAsync(RequestDownloadOptions options, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the authenticated user's torrent list.</summary>
    /// <param name="options">Optional query parameters for filtering and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of torrents, or a single torrent if an ID is specified in <paramref name="options"/>.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Torrent>>> GetMyTorrentListAsync(GetMyListOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Checks whether one or more torrent hashes are cached on TorBox (GET).</summary>
    /// <param name="options">The cache check options containing hashes and optional parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cache status data as a dynamic object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> CheckCachedAsync(CheckCachedOptions options, CancellationToken cancellationToken = default);

    /// <summary>Checks whether one or more torrent hashes are cached on TorBox (POST).</summary>
    /// <param name="request">The cache check request containing hashes and options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cache status data as a dynamic object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> CheckCachedByPostAsync(CheckCachedRequest request, CancellationToken cancellationToken = default);

    /// <summary>Exports data for a torrent in the specified format.</summary>
    /// <param name="options">The export options containing the torrent ID and optional format.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The exported data as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> ExportDataAsync(ExportDataOptions options, CancellationToken cancellationToken = default);

    /// <summary>Retrieves torrent metadata from a hash.</summary>
    /// <param name="options">The torrent info options containing the hash and optional parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The torrent metadata information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the hash in <paramref name="options"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<TorrentInfo>> GetTorrentInfoAsync(GetTorrentInfoOptions options, CancellationToken cancellationToken = default);

    /// <summary>Retrieves torrent metadata from a torrent file, magnet URI, or info hash via the POST endpoint.</summary>
    /// <param name="request">The torrent info request containing the file bytes, magnet URI, hash, and options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The torrent metadata information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<TorrentInfo>> GetTorrentInfoByFileAsync(TorrentInfoRequest request, CancellationToken cancellationToken = default);

    /// <summary>Edits the properties of an existing torrent.</summary>
    /// <param name="request">The edit request containing the new property values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> EditTorrentAsync(EditTorrentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a new torrent download with asynchronous server-side processing.</summary>
    /// <param name="request">The torrent creation request containing the magnet URI or file bytes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created torrent details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<Torrent>> AsyncCreateTorrentAsync(CreateTorrentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Converts a magnet link to a torrent file.</summary>
    /// <param name="request">The request containing the magnet link to convert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The torrent file data as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> MagnetToFileAsync(MagnetToFileRequest request, CancellationToken cancellationToken = default);
}
