using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Vendors;

/// <summary>
/// Represents a request to remove a user from a vendor account.
/// </summary>
public sealed record RemoveVendorUserRequest
{
    /// <summary>
    /// Gets the email address of the user to remove.
    /// </summary>
    [JsonPropertyName("user_email")]
    public string UserEmail { get; init; } = string.Empty;
}
