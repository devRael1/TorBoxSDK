using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Rss;

namespace TorBoxSDK.Main.Rss;

/// <summary>
/// Defines operations for managing RSS feeds through the TorBox Main API.
/// </summary>
public interface IRssClient
{
    /// <summary>Adds a new RSS feed to the user's account.</summary>
    /// <param name="request">The RSS feed creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> AddRssAsync(AddRssRequest request, CancellationToken cancellationToken = default);

    /// <summary>Performs a control operation on an RSS feed.</summary>
    /// <param name="request">The control operation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ControlRssAsync(ControlRssRequest request, CancellationToken cancellationToken = default);

    /// <summary>Modifies the properties of an existing RSS feed.</summary>
    /// <param name="request">The modification request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse> ModifyRssAsync(ModifyRssRequest request, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the user's configured RSS feeds.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of RSS feeds.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<RssFeed>>> GetFeedsAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves items from a specific RSS feed.</summary>
    /// <param name="rssFeedId">The identifier of the RSS feed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of RSS feed items.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<RssFeedItem>>> GetFeedItemsAsync(long rssFeedId, CancellationToken cancellationToken = default);
}
