using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;

namespace TorBoxSDK.Main.Integrations;

/// <summary>
/// Defines operations for managing third-party integrations through the TorBox Main API.
/// </summary>
public interface IIntegrationsClient
{
    /// <summary>Retrieves the authenticated user's connected OAuth integrations.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of connected OAuth integrations.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<OAuthIntegration>>> GetOAuthMeAsync(CancellationToken ct = default);

    /// <summary>Creates a Google Drive integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateGoogleDriveJobAsync(CreateIntegrationJobRequest request, CancellationToken ct = default);

    /// <summary>Creates a Dropbox integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateDropboxJobAsync(CreateIntegrationJobRequest request, CancellationToken ct = default);

    /// <summary>Creates a OneDrive integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateOnedriveJobAsync(CreateIntegrationJobRequest request, CancellationToken ct = default);

    /// <summary>Creates a GoFile integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateGofileJobAsync(CreateIntegrationJobRequest request, CancellationToken ct = default);

    /// <summary>Creates a 1Fichier integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateOneFichierJobAsync(CreateIntegrationJobRequest request, CancellationToken ct = default);

    /// <summary>Creates a Pixeldrain integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreatePixeldrainJobAsync(CreateIntegrationJobRequest request, CancellationToken ct = default);

    /// <summary>Retrieves the status of a specific integration job.</summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The integration job details.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> GetJobAsync(long jobId, CancellationToken ct = default);

    /// <summary>Deletes a specific integration job.</summary>
    /// <param name="jobId">The unique identifier of the job to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> DeleteJobAsync(long jobId, CancellationToken ct = default);

    /// <summary>Retrieves all integration jobs for the authenticated user.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of integration jobs.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<IntegrationJob>>> GetJobsAsync(CancellationToken ct = default);

    /// <summary>Retrieves integration jobs associated with a specific hash.</summary>
    /// <param name="hash">The hash to search jobs for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of integration jobs for the specified hash.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="hash"/> is null or empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<IntegrationJob>>> GetJobsByHashAsync(string hash, CancellationToken ct = default);
}
