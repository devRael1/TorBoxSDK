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
    /// Gets the date and time when the notification was created,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets a value indicating whether the notification has been read.
    /// </summary>
    [JsonPropertyName("read")]
    public bool Read { get; init; }
}
