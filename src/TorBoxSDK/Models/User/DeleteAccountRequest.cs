using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to delete the authenticated user's account.
/// </summary>
public sealed record DeleteAccountRequest
{
	/// <summary>
	/// Gets the user's session token from the website, required for account deletion.
	/// </summary>
	[JsonPropertyName("session_token")]
	public string SessionToken { get; init; } = string.Empty;

	/// <summary>
	/// Gets the confirmation code sent to the user's email.
	/// </summary>
	[JsonPropertyName("confirmation_code")]
	public int ConfirmationCode { get; init; }
}
