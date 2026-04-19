using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Vendors;

/// <summary>
/// Represents a request to register a new user under a vendor account.
/// </summary>
public sealed record RegisterVendorUserRequest
{
	/// <summary>
	/// Gets the email address of the user to register.
	/// </summary>
	[JsonPropertyName("user_email")]
	public string UserEmail { get; init; } = string.Empty;
}
