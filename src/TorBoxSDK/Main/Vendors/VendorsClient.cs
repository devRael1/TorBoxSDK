using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Vendors;

namespace TorBoxSDK.Main.Vendors;

/// <summary>
/// Default implementation of <see cref="IVendorsClient"/> for managing
/// vendors through the TorBox Main API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="VendorsClient"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client configured for the Main API.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class VendorsClient(HttpClient httpClient) : IVendorsClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> RegisterAsync(RegisterVendorRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        Guard.ThrowIfNullOrEmpty(request.VendorName, nameof(request.VendorName));

        MultipartFormDataContent content = new()
        {
            { new StringContent(request.VendorName), "vendor_name" }
        };

        if (request.VendorUrl is not null)
        {
            content.Add(new StringContent(request.VendorUrl), "vendor_url");
        }

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, "vendors/register") { Content = content };
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> GetAccountAsync(CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage httpRequest = new(HttpMethod.Get, "vendors/account");
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> UpdateAccountAsync(UpdateVendorAccountRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.VendorName is null && request.VendorUrl is null)
        {
            throw new ArgumentException("At least one of VendorName or VendorUrl must be provided.", nameof(request));
        }

        MultipartFormDataContent content = [];

        if (!string.IsNullOrEmpty(request.VendorName))
        {
            content.Add(new StringContent(request.VendorName), "vendor_name");
        }

        if (!string.IsNullOrEmpty(request.VendorUrl))
        {
            content.Add(new StringContent(request.VendorUrl), "vendor_url");
        }

        using HttpRequestMessage httpRequest = new(HttpMethod.Put, "vendors/updateaccount") { Content = content };
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<VendorAccount>>> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage httpRequest = new(HttpMethod.Get, "vendors/getaccounts");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<VendorAccount>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> GetAccountByAuthIdAsync(string userAuthId, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(userAuthId, nameof(userAuthId));

        string query = TorBoxApiHelper.BuildQuery(("user_auth_id", userAuthId));

        using HttpRequestMessage httpRequest = new(HttpMethod.Get, $"vendors/getaccount{query}");
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> RegisterUserAsync(RegisterVendorUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        Guard.ThrowIfNullOrEmpty(request.UserEmail, nameof(request.UserEmail));

        MultipartFormDataContent content = new()
        {
            { new StringContent(request.UserEmail), "user_email" }
        };

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, "vendors/registeruser") { Content = content };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> RemoveUserAsync(RemoveVendorUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using HttpRequestMessage httpRequest = new(HttpMethod.Delete, "vendors/removeuser")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> RefreshAsync(CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage httpRequest = new(HttpMethod.Patch, "vendors/refresh");
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
