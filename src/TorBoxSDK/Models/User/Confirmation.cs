using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents the confirmation status response for the authenticated user.
/// </summary>
public sealed record Confirmation
{
    /// <summary>
    /// Gets the confirmation status, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets confirmation details, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("details")]
    public object? Details { get; init; }
}
