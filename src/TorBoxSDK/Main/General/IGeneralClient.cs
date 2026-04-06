using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;
using TorBoxSDK.Models.Notifications;

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
    Task<TorBoxResponse<object>> GetUpStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves aggregate TorBox service statistics.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current statistics.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<Stats>> GetStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves TorBox service statistics for the last 30 days.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The 30-day statistics.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<Stats>> Get30DayStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves speedtest file URLs for connection testing.</summary>
    /// <param name="options">Optional speedtest parameters, or <see langword="null"/> for defaults.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The speedtest file data.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<object>> GetSpeedtestFilesAsync(SpeedtestOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Gets the changelogs RSS feed URL.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The RSS feed URL as a string.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> GetChangelogsRssAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the changelogs as structured JSON data.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of changelog entries.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<Changelog>>> GetChangelogsJsonAsync(CancellationToken cancellationToken = default);
}
