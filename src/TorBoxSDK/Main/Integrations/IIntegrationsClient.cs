using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;

namespace TorBoxSDK.Main.Integrations;

/// <summary>
/// Defines operations for managing third-party integrations through the TorBox Main API.
/// </summary>
public interface IIntegrationsClient
{
    /// <summary>Retrieves the authenticated user's connected OAuth integrations.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of provider names to connection status.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyDictionary<string, bool>>> GetOAuthMeAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a Google Drive integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateGoogleDriveJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a Dropbox integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateDropboxJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a OneDrive integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateOnedriveJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a GoFile integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateGofileJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a 1Fichier integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreateOneFichierJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a Pixeldrain integration job.</summary>
    /// <param name="request">The integration job request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created job details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> CreatePixeldrainJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the status of a specific integration job.</summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The integration job details.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntegrationJob>> GetJobAsync(long jobId, CancellationToken cancellationToken = default);

    /// <summary>Deletes a specific integration job.</summary>
    /// <param name="jobId">The unique identifier of the job to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> DeleteJobAsync(long jobId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all integration jobs for the authenticated user.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of integration jobs.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<IntegrationJob>>> GetJobsAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves integration jobs associated with a specific hash.</summary>
    /// <param name="hash">The hash to search jobs for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of integration jobs for the specified hash.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hash"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="hash"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<IntegrationJob>>> GetJobsByHashAsync(string hash, CancellationToken cancellationToken = default);

    /// <summary>Initiates an OAuth redirect for the specified provider.</summary>
    /// <param name="provider">The OAuth provider name (e.g., "google", "dropbox").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The redirect URL as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="provider"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> OAuthRedirectAsync(string provider, CancellationToken cancellationToken = default);

    /// <summary>Handles the OAuth callback for the specified provider.</summary>
    /// <param name="provider">The OAuth provider name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The callback response data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="provider"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<OAuthCallback>> OAuthCallbackAsync(string provider, CancellationToken cancellationToken = default);

    /// <summary>Handles the OAuth success endpoint for the specified provider.</summary>
    /// <param name="provider">The OAuth provider name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The success response data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="provider"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<OAuthSuccess>> OAuthSuccessAsync(string provider, CancellationToken cancellationToken = default);

    /// <summary>Registers an OAuth integration with the specified provider.</summary>
    /// <param name="request">The registration request containing the provider, authorization code and redirect URI.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>, or when the provider in <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the provider in <paramref name="request"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> OAuthRegisterAsync(OAuthRegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Unregisters an OAuth integration with the specified provider.</summary>
    /// <param name="provider">The OAuth provider name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="provider"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> OAuthUnregisterAsync(string provider, CancellationToken cancellationToken = default);

    /// <summary>Retrieves linked Discord roles for the authenticated user.</summary>
    /// <param name="request">The linked roles request containing the Discord token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The linked Discord roles data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<LinkedDiscordRoles>> GetLinkedDiscordRolesAsync(LinkedRolesRequest request, CancellationToken cancellationToken = default);
}
