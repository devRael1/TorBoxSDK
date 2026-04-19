using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>
/// Represents geographic coordinates of a speedtest server.
/// </summary>
public sealed record ServerCoordinates
{
	/// <summary>
	/// Gets the latitude of the server location.
	/// </summary>
	[JsonPropertyName("lat")]
	public double Lat { get; init; }

	/// <summary>
	/// Gets the longitude of the server location.
	/// </summary>
	[JsonPropertyName("lng")]
	public double Lng { get; init; }
}
