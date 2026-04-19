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
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created web download details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<WebDownload>> CreateWebDownloadAsync(CreateWebDownloadRequest request, CancellationToken cancellationToken = default);

    /// <summary>Performs a control operation on a web download.</summary>
    /// <param name="request">The control operation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ControlWebDownloadAsync(ControlWebDownloadRequest request, CancellationToken cancellationToken = default);

    /// <summary>Requests a download link for a web download.</summary>
    /// <param name="webId">The unique identifier of the web download.</param>
    /// <param name="options">Optional download request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The download URL as a string.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> RequestDownloadAsync(long webId, RequestWebDownloadOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the authenticated user's web download list.</summary>
    /// <param name="options">Optional query parameters for filtering and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of web downloads.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<WebDownload>>> GetMyWebDownloadListAsync(GetMyListOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Checks whether one or more web download hashes are cached on TorBox (GET).</summary>
    /// <param name="hashes">The list of hashes to check for cache availability.</param>
    /// <param name="options">Optional cache check parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cache status data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hashes"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<CheckWebCached>> CheckCachedAsync(IReadOnlyList<string> hashes, CheckCachedOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Checks whether one or more web download hashes are cached on TorBox (POST).</summary>
    /// <param name="request">The cache check request containing hashes and options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cache status data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<CheckWebCached>> CheckCachedByPostAsync(CheckWebCachedRequest request, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the list of supported hosters and their status.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of supported hosters.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Hoster>>> GetHostersAsync(CancellationToken cancellationToken = default);

    /// <summary>Edits the properties of an existing web download.</summary>
    /// <param name="request">The edit request containing the new property values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> EditWebDownloadAsync(EditWebDownloadRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a new web download with asynchronous server-side processing.</summary>
    /// <param name="request">The web download creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created web download details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<WebDownload>> AsyncCreateWebDownloadAsync(CreateWebDownloadRequest request, CancellationToken cancellationToken = default);
}
