using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Usenet;

namespace TorBoxSDK.Main.Usenet;

/// <summary>
/// Defines operations for managing usenet downloads through the TorBox Main API.
/// </summary>
public interface IUsenetClient
{
    /// <summary>Creates a new Usenet download from a link or NZB file.</summary>
    /// <param name="request">The Usenet download creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created Usenet download details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<UsenetDownload>> CreateUsenetDownloadAsync(CreateUsenetDownloadRequest request, CancellationToken cancellationToken = default);

    /// <summary>Performs a control operation on a Usenet download.</summary>
    /// <param name="request">The control operation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ControlUsenetDownloadAsync(ControlUsenetDownloadRequest request, CancellationToken cancellationToken = default);

    /// <summary>Requests a download link for a Usenet download.</summary>
    /// <param name="options">The download request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The download URL as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> RequestDownloadAsync(RequestUsenetDownloadOptions options, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the authenticated user's Usenet download list.</summary>
    /// <param name="options">Optional query parameters for filtering and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of Usenet downloads.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<UsenetDownload>>> GetMyUsenetListAsync(GetMyListOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Checks whether one or more Usenet hashes are cached on TorBox (GET).</summary>
    /// <param name="options">The cache check options containing hashes and optional parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cache status data as a dynamic object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> CheckCachedAsync(CheckCachedOptions options, CancellationToken cancellationToken = default);

    /// <summary>Checks whether one or more Usenet hashes are cached on TorBox (POST).</summary>
    /// <param name="request">The cache check request containing hashes and options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cache status data as a dynamic object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> CheckCachedByPostAsync(CheckUsenetCachedRequest request, CancellationToken cancellationToken = default);

    /// <summary>Edits the properties of an existing Usenet download.</summary>
    /// <param name="request">The edit request containing the new property values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> EditUsenetDownloadAsync(EditUsenetDownloadRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Usenet download with asynchronous server-side processing.</summary>
    /// <param name="request">The Usenet download creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created Usenet download details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<UsenetDownload>> AsyncCreateUsenetDownloadAsync(CreateUsenetDownloadRequest request, CancellationToken cancellationToken = default);
}
