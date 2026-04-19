using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;

namespace TorBoxSDK.Main.General;

/// <summary>
/// Defines general operations for the TorBox API root, including
/// status, statistics, and speedtest endpoints.
/// </summary>
public interface IGeneralClient
{
	/// <summary>Retrieves the API root status.</summary>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The API status data.</returns>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<UpStatus>> GetUpStatusAsync(CancellationToken cancellationToken = default);

	/// <summary>Retrieves aggregate TorBox service statistics.</summary>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The current statistics.</returns>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<Stats>> GetStatsAsync(CancellationToken cancellationToken = default);

	/// <summary>Retrieves TorBox service statistics for the last 30 days.</summary>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A list of daily statistics snapshots.</returns>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<IReadOnlyList<DailyStats>>> Get30DayStatsAsync(CancellationToken cancellationToken = default);

	/// <summary>Retrieves speedtest server URLs for connection testing.</summary>
	/// <param name="options">Optional speedtest parameters, or <see langword="null"/> for defaults.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A list of speedtest servers.</returns>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<IReadOnlyList<SpeedtestServer>>> GetSpeedtestFilesAsync(SpeedtestOptions? options = null, CancellationToken cancellationToken = default);

	/// <summary>Gets the changelogs as an RSS 2.0 feed.</summary>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The parsed RSS feed with channel metadata and changelog items.</returns>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<ChangelogsRssFeed>> GetChangelogsRssAsync(CancellationToken cancellationToken = default);

	/// <summary>Gets the changelogs as structured JSON data.</summary>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A list of changelog entries.</returns>
	/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
	Task<TorBoxResponse<IReadOnlyList<Changelog>>> GetChangelogsJsonAsync(CancellationToken cancellationToken = default);
}
