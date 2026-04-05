using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Main.User;

/// <summary>
/// Defines operations for user account management through the TorBox Main API.
/// </summary>
public interface IUserClient
{
    /// <summary>Refreshes an authentication session token.</summary>
    /// <param name="request">The token refresh request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The refreshed token data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);

    /// <summary>Gets the confirmation status for the authenticated user.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The confirmation data.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> GetConfirmationAsync(CancellationToken ct = default);

    /// <summary>Retrieves the authenticated user's profile.</summary>
    /// <param name="settings">Whether to include account settings in the response.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The user profile data.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<UserProfile>> GetMeAsync(bool? settings = null, CancellationToken ct = default);

    /// <summary>Adds a referral code to the user's account.</summary>
    /// <param name="referralCode">The referral code to apply.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="referralCode"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> AddReferralAsync(string referralCode, CancellationToken ct = default);

    /// <summary>Starts the device authorization flow.</summary>
    /// <param name="app">The application identifier, or <see langword="null"/> to omit.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The device code response containing a user code and verification URL.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<DeviceCodeResponse>> StartDeviceAuthAsync(string? app = null, CancellationToken ct = default);

    /// <summary>Exchanges a device code for an access token.</summary>
    /// <param name="request">The device token request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The token data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> GetDeviceTokenAsync(DeviceTokenRequest request, CancellationToken ct = default);

    /// <summary>Deletes the authenticated user's account.</summary>
    /// <param name="request">The delete account request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> DeleteMeAsync(DeleteAccountRequest request, CancellationToken ct = default);

    /// <summary>Retrieves the authenticated user's referral data.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The referral data.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<ReferralData>> GetReferralDataAsync(CancellationToken ct = default);

    /// <summary>Retrieves the authenticated user's subscriptions.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of subscriptions.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Subscription>>> GetSubscriptionsAsync(CancellationToken ct = default);

    /// <summary>Retrieves the authenticated user's transactions.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of transactions.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Transaction>>> GetTransactionsAsync(CancellationToken ct = default);

    /// <summary>Retrieves a transaction PDF by its identifier.</summary>
    /// <param name="transactionId">The unique identifier of the transaction.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The PDF data as a string (typically a download URL).</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> GetTransactionPdfAsync(long transactionId, CancellationToken ct = default);

    /// <summary>Adds search engines to the user's settings.</summary>
    /// <param name="request">The request containing search engines to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> AddSearchEnginesAsync(AddSearchEnginesRequest request, CancellationToken ct = default);

    /// <summary>Retrieves the user's configured search engines.</summary>
    /// <param name="id">An optional search engine identifier to filter by.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of configured search engines.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<SearchEngine>>> GetSearchEnginesAsync(long? id = null, CancellationToken ct = default);

    /// <summary>Modifies the user's search engine configuration.</summary>
    /// <param name="request">The modification request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ModifySearchEnginesAsync(ModifySearchEnginesRequest request, CancellationToken ct = default);

    /// <summary>Performs a control operation on the user's search engine settings.</summary>
    /// <param name="request">The control operation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ControlSearchEnginesAsync(ControlSearchEnginesRequest request, CancellationToken ct = default);

    /// <summary>Edits the user's account settings.</summary>
    /// <param name="request">The settings edit request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> EditSettingsAsync(EditSettingsRequest request, CancellationToken ct = default);
}
