using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents a single Discord role in the linked roles response.
/// </summary>
public sealed record DiscordRoleInfo
{
	/// <summary>
	/// Gets the Discord role ID, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; init; }

	/// <summary>
	/// Gets the role name, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>
	/// Gets additional role data, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("extra")]
	public object? Extra { get; init; }
}
