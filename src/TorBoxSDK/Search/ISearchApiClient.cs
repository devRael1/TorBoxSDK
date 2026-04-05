using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;

namespace TorBoxSDK.Search;

/// <summary>
/// Defines the Search API client for querying torrent and usenet indexers
/// and metadata through the TorBox Search API.
/// </summary>
public interface ISearchApiClient
{
    /// <summary>Retrieves the torrent search tutorial and information page.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tutorial content as a string.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> GetTorrentSearchTutorialAsync(CancellationToken ct = default);

    /// <summary>Searches for torrents matching the specified query.</summary>
    /// <param name="query">The search query string.</param>
    /// <param name="options">Optional search options to filter and customize results.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of matching torrent search results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="query"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<TorrentSearchResult>>> SearchTorrentsAsync(string query, TorrentSearchOptions? options = null, CancellationToken ct = default);

    /// <summary>Retrieves a specific torrent by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the torrent.</param>
    /// <param name="options">Optional search options to filter and customize results.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The torrent search result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<TorrentSearchResult>> GetTorrentByIdAsync(string id, TorrentSearchOptions? options = null, CancellationToken ct = default);

    /// <summary>Retrieves the usenet search tutorial and information page.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tutorial content as a string.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> GetUsenetSearchTutorialAsync(CancellationToken ct = default);

    /// <summary>Searches for usenet articles matching the specified query.</summary>
    /// <param name="query">The search query string.</param>
    /// <param name="options">Optional search options to filter and customize results.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of matching usenet search results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="query"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<UsenetSearchResult>>> SearchUsenetAsync(string query, UsenetSearchOptions? options = null, CancellationToken ct = default);

    /// <summary>Retrieves a specific usenet article by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the usenet article.</param>
    /// <param name="options">Optional search options to filter and customize results.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The usenet search result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<UsenetSearchResult>> GetUsenetByIdAsync(string id, UsenetSearchOptions? options = null, CancellationToken ct = default);

    /// <summary>Downloads an NZB file for a usenet article.</summary>
    /// <param name="id">The unique identifier of the usenet article.</param>
    /// <param name="guid">The GUID of the NZB file to download.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The NZB download data as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> or <paramref name="guid"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> or <paramref name="guid"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> DownloadUsenetAsync(string id, string guid, CancellationToken ct = default);

    /// <summary>Retrieves the meta search tutorial and information page.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tutorial content as a string.</returns>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> GetMetaSearchTutorialAsync(CancellationToken ct = default);

    /// <summary>Searches for metadata entries matching the specified query.</summary>
    /// <param name="query">The search query string.</param>
    /// <param name="options">Optional search options to filter and customize results.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of matching metadata search results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="query"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<IReadOnlyList<MetaSearchResult>>> SearchMetaAsync(string query, MetaSearchOptions? options = null, CancellationToken ct = default);

    /// <summary>Retrieves a specific metadata entry by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the metadata entry.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The metadata search result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<MetaSearchResult>> GetMetaByIdAsync(string id, CancellationToken ct = default);

    /// <summary>Searches the Torznab API for torrents matching the specified query.</summary>
    /// <param name="query">The search query string.</param>
    /// <param name="apiKey">Optional API key override for the Torznab endpoint.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The search results as an XML string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="query"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> SearchTorznabAsync(string query, string? apiKey = null, CancellationToken ct = default);

    /// <summary>Searches the Newznab API for usenet articles matching the specified query.</summary>
    /// <param name="query">The search query string.</param>
    /// <param name="apiKey">Optional API key override for the Newznab endpoint.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The search results as an XML string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="query"/> is empty.</exception>
    /// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
    Task<TorBoxResponse<string>> SearchNewznabAsync(string query, string? apiKey = null, CancellationToken ct = default);
}
