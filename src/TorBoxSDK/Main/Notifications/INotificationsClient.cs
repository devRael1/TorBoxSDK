using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;

namespace TorBoxSDK.Main.Notifications;

/// <summary>
/// Defines operations for managing notifications through the TorBox Main API.
/// </summary>
public interface INotificationsClient
{
    /// <summary>Gets the notifications RSS feed.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed RSS feed.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<NotificationRssFeed>> GetNotificationRssAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves the authenticated user's notifications.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of notifications.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Notification>>> GetMyNotificationsAsync(CancellationToken cancellationToken = default);

    /// <summary>Clears all notifications for the authenticated user.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ClearAllNotificationsAsync(CancellationToken cancellationToken = default);

    /// <summary>Clears a specific notification by its identifier.</summary>
    /// <param name="notificationId">The unique identifier of the notification to clear.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ClearNotificationAsync(long notificationId, CancellationToken cancellationToken = default);

    /// <summary>Sends a test notification to the authenticated user.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> SendTestNotificationAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the Intercom identity verification hash for the authenticated user.</summary>
    /// <param name="options">The options containing the auth ID and email.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The Intercom hash data.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="options"/> is <see langword="null"/>, or when
    /// <paramref name="options"/>.<see cref="GetIntercomHashOptions.AuthId"/> or
    /// <paramref name="options"/>.<see cref="GetIntercomHashOptions.Email"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="options"/>.<see cref="GetIntercomHashOptions.AuthId"/> or
    /// <paramref name="options"/>.<see cref="GetIntercomHashOptions.Email"/> is empty.
    /// </exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntercomHash>> GetIntercomHashAsync(GetIntercomHashOptions options, CancellationToken cancellationToken = default);
}
