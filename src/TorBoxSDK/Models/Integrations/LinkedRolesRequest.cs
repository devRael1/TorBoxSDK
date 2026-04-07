using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents a request to link Discord roles.
/// </summary>
public sealed record LinkedRolesRequest
{
    /// <summary>
    /// Gets the Discord OAuth token for linked roles.
    /// </summary>
    [JsonPropertyName("discord_token")]
    public string DiscordToken { get; init; } = string.Empty;
}
