using TorBoxSDK.Main;
using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorBoxSDK;

/// <summary>
/// Defines the root entry point for the TorBox SDK.
/// </summary>
/// <remarks>
/// Provides access to the Main, Search, and Relay API clients
/// that together cover the full TorBox platform surface.
/// </remarks>
public interface ITorBoxClient
{
    /// <summary>
    /// Gets the Main API client, which exposes resource clients for
    /// torrents, usenet, web downloads, user management, and more.
    /// </summary>
    IMainApiClient Main { get; }

    /// <summary>
    /// Gets the Search API client for querying torrent and usenet indexers.
    /// </summary>
    ISearchApiClient Search { get; }

    /// <summary>
    /// Gets the Relay API client for relay-based operations.
    /// </summary>
    IRelayApiClient Relay { get; }
}
