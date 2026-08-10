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
public interface ITorBoxClient : IDisposable
{
	/// <summary>
	/// Gets the Main API client, which exposes resource clients for
	/// torrents, usenet, web downloads, user management, and more.
	/// </summary>
	IMainApiClient Main { get; }

	/// <summary>
	/// Gets the Search API client for querying torrent and usenet indexers.
	/// </summary>
	/// <remarks>
	/// TorBox restricts the Search API to approved projects and whitelisted
	/// source IP addresses. Access is not included automatically with this SDK
	/// or with a TorBox account.
	/// </remarks>
	ISearchApiClient Search { get; }

	/// <summary>
	/// Gets the Relay API client for relay-based operations.
	/// </summary>
	IRelayApiClient Relay { get; }
}
