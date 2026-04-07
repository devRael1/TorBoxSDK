using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Notifications;

/// <summary>
/// Represents a notification sent to a TorBox user.
/// </summary>
public sealed record Notification
{
    /// <summary>
    /// Gets the unique identifier of the notification.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the authentication identifier of the user who owns this notification,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("auth_id")]
    public string? AuthId { get; init; }

    /// <summary>
    /// Gets the date and time when the notification was created,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the title of the notification, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Gets the message body of the notification, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>
    /// Gets the action type for the notification (e.g., "url"),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("action")]
    public string? Action { get; init; }

    /// <summary>
    /// Gets the action data associated with the notification (e.g., a URL),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("action_data")]
    public string? ActionData { get; init; }

    /// <summary>
    /// Gets the call-to-action text for the notification (e.g., "Download Now"),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("action_cta")]
    public string? ActionCta { get; init; }
}
