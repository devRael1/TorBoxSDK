using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Notifications;

/// <summary>
/// Represents the query parameters for getting the Intercom identity verification hash.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record GetIntercomHashOptions
{
    /// <summary>
    /// Gets the user's auth identifier.
    /// </summary>
    [JsonPropertyName("auth_id")]
    public string AuthId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;
}
