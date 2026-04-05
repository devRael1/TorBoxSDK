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
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created torrent details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<Torrent>> CreateTorrentAsync(CreateTorrentRequest request, CancellationToken ct = default);

    /// <summary>Performs a control operation on a torrent (delete, pause, resume, etc.).</summary>
    /// <param name="request">The control operation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ControlTorrentAsync(ControlTorrentRequest request, CancellationToken ct = default);

    /// <summary>Requests a download link for a torrent.</summary>
    /// <param name="options">The download request options.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The download URL as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> RequestDownloadAsync(RequestDownloadOptions options, CancellationToken ct = default);

    /// <summary>Retrieves the authenticated user's torrent list.</summary>
    /// <param name="id">Optional torrent ID to retrieve a single torrent.</param>
    /// <param name="offset">Optional offset for pagination.</param>
    /// <param name="limit">Optional limit for pagination.</param>
    /// <param name="bypassCache">Optional flag to bypass the cache.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of torrents, or a single torrent if <paramref name="id"/> is provided.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Torrent>>> GetMyTorrentListAsync(long? id = null, int? offset = null, int? limit = null, bool? bypassCache = null, CancellationToken ct = default);

    /// <summary>Checks whether one or more torrent hashes are cached on TorBox (GET).</summary>
    /// <param name="hashes">The list of torrent info hashes to check.</param>
    /// <param name="format">Optional response format.</param>
    /// <param name="listFiles">Optional flag to include file listings.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The cache status data as a dynamic object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hashes"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> CheckCachedAsync(IReadOnlyList<string> hashes, string? format = null, bool? listFiles = null, CancellationToken ct = default);

    /// <summary>Checks whether one or more torrent hashes are cached on TorBox (POST).</summary>
    /// <param name="request">The cache check request containing hashes and options.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The cache status data as a dynamic object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> CheckCachedByPostAsync(CheckCachedRequest request, CancellationToken ct = default);

    /// <summary>Exports data for a torrent in the specified format.</summary>
    /// <param name="torrentId">The unique identifier of the torrent.</param>
    /// <param name="type">Optional export type format.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The exported data as a string.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> ExportDataAsync(long torrentId, string? type = null, CancellationToken ct = default);

    /// <summary>Retrieves torrent metadata from a hash.</summary>
    /// <param name="hash">The info hash of the torrent.</param>
    /// <param name="timeout">Optional timeout in seconds for metadata retrieval.</param>
    /// <param name="useCacheLookup">Optional flag to enable cache lookup for the torrent info.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The torrent metadata information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hash"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="hash"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<TorrentInfo>> GetTorrentInfoAsync(string hash, int? timeout = null, bool? useCacheLookup = null, CancellationToken ct = default);

    /// <summary>Retrieves torrent metadata from a torrent file, magnet URI, or info hash via the POST endpoint.</summary>
    /// <param name="request">The torrent info request containing the file bytes, magnet URI, hash, and options.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The torrent metadata information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<TorrentInfo>> GetTorrentInfoByFileAsync(TorrentInfoRequest request, CancellationToken ct = default);

    /// <summary>Edits the properties of an existing torrent.</summary>
    /// <param name="request">The edit request containing the new property values.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> EditTorrentAsync(EditTorrentRequest request, CancellationToken ct = default);

    /// <summary>Creates a new torrent download with asynchronous server-side processing.</summary>
    /// <param name="request">The torrent creation request containing the magnet URI or file bytes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created torrent details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<Torrent>> AsyncCreateTorrentAsync(CreateTorrentRequest request, CancellationToken ct = default);

    /// <summary>Converts a magnet link to a torrent file.</summary>
    /// <param name="request">The request containing the magnet link to convert.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The torrent file data as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> MagnetToFileAsync(MagnetToFileRequest request, CancellationToken ct = default);
}
