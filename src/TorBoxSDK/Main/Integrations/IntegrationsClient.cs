using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;

namespace TorBoxSDK.Main.Integrations;

/// <summary>
/// Default implementation of <see cref="IIntegrationsClient"/> for managing
/// third-party integrations through the TorBox Main API.
/// </summary>
public sealed class IntegrationsClient : IIntegrationsClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public IntegrationsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyDictionary<string, bool>>> GetOAuthMeAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "integration/oauth/me");
        return await TorBoxApiHelper.SendAsync<IReadOnlyDictionary<string, bool>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IntegrationJob>> CreateGoogleDriveJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "integration/googledrive")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<IntegrationJob>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IntegrationJob>> CreateDropboxJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "integration/dropbox")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<IntegrationJob>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IntegrationJob>> CreateOnedriveJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "integration/onedrive")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<IntegrationJob>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IntegrationJob>> CreateGofileJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "integration/gofile")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<IntegrationJob>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IntegrationJob>> CreateOneFichierJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "integration/1fichier")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<IntegrationJob>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IntegrationJob>> CreatePixeldrainJobAsync(CreateIntegrationJobRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "integration/pixeldrain")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<IntegrationJob>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IntegrationJob>> GetJobAsync(long jobId, CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"integration/job/{jobId}");
        return await TorBoxApiHelper.SendAsync<IntegrationJob>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> DeleteJobAsync(long jobId, CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"integration/job/{jobId}");
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<IntegrationJob>>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "integration/jobs");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<IntegrationJob>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<IntegrationJob>>> GetJobsByHashAsync(string hash, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(hash, nameof(hash));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"integration/jobs/{Uri.EscapeDataString(hash)}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<IntegrationJob>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> OAuthRedirectAsync(string provider, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(provider, nameof(provider));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"integration/oauth/{Uri.EscapeDataString(provider)}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> OAuthCallbackAsync(string provider, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(provider, nameof(provider));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"integration/oauth/{Uri.EscapeDataString(provider)}/callback");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> OAuthSuccessAsync(string provider, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(provider, nameof(provider));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"integration/oauth/{Uri.EscapeDataString(provider)}/success");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> OAuthRegisterAsync(OAuthRegisterRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        Guard.ThrowIfNullOrEmpty(request.Provider, nameof(request.Provider));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"integration/oauth/{Uri.EscapeDataString(request.Provider)}/register")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> OAuthUnregisterAsync(string provider, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(provider, nameof(provider));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"integration/oauth/{Uri.EscapeDataString(provider)}/unregister");
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> GetLinkedDiscordRolesAsync(LinkedRolesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "integration/oauth/discord/linked_roles")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
