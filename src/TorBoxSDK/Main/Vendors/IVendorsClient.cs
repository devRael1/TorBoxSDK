using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Vendors;

namespace TorBoxSDK.Main.Vendors;

/// <summary>
/// Defines operations for managing vendors through the TorBox Main API.
/// </summary>
public interface IVendorsClient
{
    /// <summary>Registers a new vendor account.</summary>
    /// <param name="request">The vendor registration request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created vendor account details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<VendorAccount>> RegisterAsync(RegisterVendorRequest request, CancellationToken ct = default);

    /// <summary>Retrieves the authenticated vendor's account details.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The vendor account details.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<VendorAccount>> GetAccountAsync(CancellationToken ct = default);

    /// <summary>Updates the authenticated vendor's account details.</summary>
    /// <param name="request">The update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated vendor account details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<VendorAccount>> UpdateAccountAsync(UpdateVendorAccountRequest request, CancellationToken ct = default);

    /// <summary>Retrieves all user accounts managed by the vendor.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of vendor accounts.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<VendorAccount>>> GetAccountsAsync(CancellationToken ct = default);

    /// <summary>Retrieves a specific user account by email.</summary>
    /// <param name="userEmail">The email address of the user to look up.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The vendor account details for the user.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="userEmail"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userEmail"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<VendorAccount>> GetAccountByEmailAsync(string userEmail, CancellationToken ct = default);

    /// <summary>Registers a new user under the vendor's account.</summary>
    /// <param name="request">The user registration request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> RegisterUserAsync(RegisterVendorUserRequest request, CancellationToken ct = default);

    /// <summary>Removes a user from the vendor's account.</summary>
    /// <param name="request">The user removal request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> RemoveUserAsync(RemoveVendorUserRequest request, CancellationToken ct = default);

    /// <summary>Refreshes the vendor's API credentials.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated vendor account with new credentials.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<VendorAccount>> RefreshAsync(CancellationToken ct = default);
}
