using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;

namespace TorBoxSDK.Main.Notifications;

/// <summary>
/// Defines operations for managing notifications through the TorBox Main API.
/// </summary>
public interface INotificationsClient
{
    /// <summary>Gets the notifications RSS feed URL.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The RSS feed URL as a string.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> GetNotificationRssAsync(CancellationToken ct = default);

    /// <summary>Retrieves the authenticated user's notifications.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of notifications.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Notification>>> GetMyNotificationsAsync(CancellationToken ct = default);

    /// <summary>Clears all notifications for the authenticated user.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ClearAllNotificationsAsync(CancellationToken ct = default);

    /// <summary>Clears a specific notification by its identifier.</summary>
    /// <param name="notificationId">The unique identifier of the notification to clear.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ClearNotificationAsync(long notificationId, CancellationToken ct = default);

    /// <summary>Sends a test notification to the authenticated user.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> SendTestNotificationAsync(CancellationToken ct = default);

    /// <summary>Gets the Intercom identity verification hash for the authenticated user.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The Intercom hash data.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IntercomHash>> GetIntercomHashAsync(CancellationToken ct = default);

    /// <summary>Gets the changelogs RSS feed URL.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The RSS feed URL as a string.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> GetChangelogsRssAsync(CancellationToken ct = default);

    /// <summary>Gets the changelogs as structured JSON data.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of changelog entries.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Changelog>>> GetChangelogsJsonAsync(CancellationToken ct = default);
}
