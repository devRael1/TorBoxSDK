using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Main.User;

/// <summary>
/// Default implementation of <see cref="IUserClient"/> for user account
/// management through the TorBox Main API.
/// </summary>
public sealed class UserClient : IUserClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public UserClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "user/refreshtoken")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> GetConfirmationAsync(CancellationToken ct = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "user/getconfirmation");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<UserProfile>> GetMeAsync(bool? settings = null, CancellationToken ct = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("settings", settings?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"user/me{query}");
        return await TorBoxApiHelper.SendAsync<UserProfile>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> AddReferralAsync(string referralCode, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(referralCode);

        string query = TorBoxApiHelper.BuildQuery(
            ("referral", referralCode));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"user/addreferral{query}");
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<DeviceCodeResponse>> StartDeviceAuthAsync(string? app = null, CancellationToken ct = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("app", app));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"user/auth/device/start{query}");
        return await TorBoxApiHelper.SendAsync<DeviceCodeResponse>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> GetDeviceTokenAsync(DeviceTokenRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "user/auth/device/token")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> DeleteMeAsync(DeleteAccountRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "user/deleteme")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<ReferralData>> GetReferralDataAsync(CancellationToken ct = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "user/referraldata");
        return await TorBoxApiHelper.SendAsync<ReferralData>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Subscription>>> GetSubscriptionsAsync(CancellationToken ct = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "user/subscriptions");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Subscription>>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Transaction>>> GetTransactionsAsync(CancellationToken ct = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "user/transactions");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Transaction>>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> GetTransactionPdfAsync(long transactionId, CancellationToken ct = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("transaction_id", transactionId.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"user/transaction/pdf{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> AddSearchEnginesAsync(AddSearchEnginesRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "user/settings/addsearchengines")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<SearchEngine>>> GetSearchEnginesAsync(long? id = null, CancellationToken ct = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("id", id?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"user/settings/searchengines{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<SearchEngine>>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ModifySearchEnginesAsync(ModifySearchEnginesRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "user/settings/modifysearchengines")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ControlSearchEnginesAsync(ControlSearchEnginesRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "user/settings/controlsearchengines")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> EditSettingsAsync(EditSettingsRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "user/settings/editsettings")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }
}
