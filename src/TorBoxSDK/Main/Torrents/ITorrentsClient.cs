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
	/// <param name="torrentId">The unique identifier of the torrent to download.</param>
	/// <param name="options">Optional download request options.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The download URL as a string.</returns>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<string>> RequestDownloadAsync(long torrentId, RequestDownloadOptions? options = null, CancellationToken cancellationToken = default);

	/// <summary>Retrieves the authenticated user's torrent list.</summary>
	/// <param name="options">Optional query parameters for filtering and pagination.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A list of torrents, or a single torrent if an ID is specified in <paramref name="options"/>.</returns>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<IReadOnlyList<Torrent>>> GetMyTorrentListAsync(GetMyListOptions? options = null, CancellationToken cancellationToken = default);

	/// <summary>Checks whether one or more torrent hashes are cached on TorBox (GET).</summary>
	/// <param name="hashes">The list of hashes to check for cache availability.</param>
	/// <param name="options">Optional cache check parameters.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The cache status data.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="hashes"/> is <see langword="null"/>.</exception>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<CheckCached>> CheckCachedAsync(IReadOnlyList<string> hashes, CheckCachedOptions? options = null, CancellationToken cancellationToken = default);

	/// <summary>Checks whether one or more torrent hashes are cached on TorBox (POST).</summary>
	/// <param name="request">The cache check request containing hashes and options.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The cache status data.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<CheckCached>> CheckCachedByPostAsync(CheckCachedRequest request, CancellationToken cancellationToken = default);

	/// <summary>Exports data for a torrent in the specified format.</summary>
	/// <param name="torrentId">The unique identifier of the torrent to export data for.</param>
	/// <param name="exportType">The export type format, or <see langword="null"/> for the default format.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The exported data as a string.</returns>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<string>> ExportDataAsync(long torrentId, string? exportType = null, CancellationToken cancellationToken = default);

	/// <summary>Retrieves torrent metadata from a hash.</summary>
	/// <param name="hash">The info hash of the torrent.</param>
	/// <param name="options">Optional torrent info parameters.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The torrent metadata information.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="hash"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="hash"/> is empty.</exception>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<TorrentInfo>> GetTorrentInfoAsync(string hash, GetTorrentInfoOptions? options = null, CancellationToken cancellationToken = default);

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
