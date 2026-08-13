using TorBoxSDK.Main;
using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorBoxSDK;

/// <summary>
/// Defines the root entry point for navigating the TorBox API families.
/// </summary>
/// <remarks>
/// Dispose the client when direct construction is used to release the SDK-owned HTTP pipelines.
/// </remarks>
public interface ITorBoxClient : IDisposable
{
	/// <summary>
	/// Gets the Main API family and its resource clients.
	/// </summary>
	IMainApiClient Main { get; }

	/// <summary>
	/// Gets the Search API family.
	/// </summary>
	ISearchApiClient Search { get; }

	/// <summary>
	/// Gets the Relay API family.
	/// </summary>
	IRelayApiClient Relay { get; }
}
