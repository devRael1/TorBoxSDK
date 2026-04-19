using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents the response from retrieving linked Discord roles.
/// </summary>
public sealed record LinkedDiscordRoles
{
	/// <summary>
	/// Gets the list of linked Discord roles.
	/// </summary>
	[JsonPropertyName("roles")]
	public IReadOnlyList<DiscordRoleInfo> Roles { get; init; } = [];

	/// <summary>
	/// Gets additional response data, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("extra")]
	public object? Extra { get; init; }
}
