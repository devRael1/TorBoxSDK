using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.WebDownloads;

namespace TorBoxSDK.Main.WebDownloads;

/// <summary>
/// Defines operations for managing web downloads through the TorBox Main API.
/// </summary>
public interface IWebDownloadsClient
{
    /// <summary>Creates a new web download from a URL.</summary>
    /// <param name="request">The web download creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created web download details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<WebDownload>> CreateWebDownloadAsync(CreateWebDownloadRequest request, CancellationToken ct = default);

    /// <summary>Performs a control operation on a web download.</summary>
    /// <param name="request">The control operation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ControlWebDownloadAsync(ControlWebDownloadRequest request, CancellationToken ct = default);

    /// <summary>Requests a download link for a web download.</summary>
    /// <param name="options">The download request options.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The download URL as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> RequestDownloadAsync(RequestWebDownloadOptions options, CancellationToken ct = default);

    /// <summary>Retrieves the authenticated user's web download list.</summary>
    /// <param name="id">Optional download ID to retrieve a single download.</param>
    /// <param name="offset">Optional offset for pagination.</param>
    /// <param name="limit">Optional limit for pagination.</param>
    /// <param name="bypassCache">Optional flag to bypass the cache.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of web downloads.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<WebDownload>>> GetMyWebDownloadListAsync(long? id = null, int? offset = null, int? limit = null, bool? bypassCache = null, CancellationToken ct = default);

    /// <summary>Checks whether one or more web download hashes are cached on TorBox (GET).</summary>
    /// <param name="hashes">The list of hashes to check.</param>
    /// <param name="format">Optional response format.</param>
    /// <param name="listFiles">Optional flag to include file listings.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The cache status data as a dynamic object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hashes"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> CheckCachedAsync(IReadOnlyList<string> hashes, string? format = null, bool? listFiles = null, CancellationToken ct = default);

    /// <summary>Checks whether one or more web download hashes are cached on TorBox (POST).</summary>
    /// <param name="request">The cache check request containing hashes and options.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The cache status data as a dynamic object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> CheckCachedByPostAsync(CheckWebCachedRequest request, CancellationToken ct = default);

    /// <summary>Retrieves the list of supported hosters and their status.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of supported hosters.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Hoster>>> GetHostersAsync(CancellationToken ct = default);

    /// <summary>Edits the properties of an existing web download.</summary>
    /// <param name="request">The edit request containing the new property values.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> EditWebDownloadAsync(EditWebDownloadRequest request, CancellationToken ct = default);

    /// <summary>Creates a new web download with asynchronous server-side processing.</summary>
    /// <param name="request">The web download creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created web download details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<WebDownload>> AsyncCreateWebDownloadAsync(CreateWebDownloadRequest request, CancellationToken ct = default);
}
