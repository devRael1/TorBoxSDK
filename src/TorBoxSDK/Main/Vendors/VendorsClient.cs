using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Vendors;

namespace TorBoxSDK.Main.Vendors;

/// <summary>
/// Default implementation of <see cref="IVendorsClient"/> for managing
/// vendors through the TorBox Main API.
/// </summary>
public sealed class VendorsClient : IVendorsClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="VendorsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public VendorsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> RegisterAsync(RegisterVendorRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = new MultipartFormDataContent();
        content.Add(new StringContent(request.VendorName), "vendor_name");

        if (request.VendorUrl is not null)
        {
            content.Add(new StringContent(request.VendorUrl), "vendor_url");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "vendors/register") { Content = content };
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> GetAccountAsync(CancellationToken ct = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "vendors/account");
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> UpdateAccountAsync(UpdateVendorAccountRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = new MultipartFormDataContent();

        if (request.VendorName is not null)
        {
            content.Add(new StringContent(request.VendorName), "vendor_name");
        }

        if (request.VendorUrl is not null)
        {
            content.Add(new StringContent(request.VendorUrl), "vendor_url");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "vendors/updateaccount") { Content = content };
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<VendorAccount>>> GetAccountsAsync(CancellationToken ct = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "vendors/getaccounts");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<VendorAccount>>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> GetAccountByAuthIdAsync(string userAuthId, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(userAuthId, nameof(userAuthId));

        string query = TorBoxApiHelper.BuildQuery(
            ("user_auth_id", userAuthId));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"vendors/getaccount{query}");
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> RegisterUserAsync(RegisterVendorUserRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = new MultipartFormDataContent();
        content.Add(new StringContent(request.UserEmail), "user_email");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "vendors/registeruser") { Content = content };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> RemoveUserAsync(RemoveVendorUserRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "vendors/removeuser")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<VendorAccount>> RefreshAsync(CancellationToken ct = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Patch, "vendors/refresh");
        return await TorBoxApiHelper.SendAsync<VendorAccount>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }
}
