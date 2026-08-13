using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Stream;

namespace TorBoxSDK.Main.Stream;

/// <summary>
/// Defines the Main API streaming resource.
/// </summary>
public interface IStreamClient
{
	/// <summary>
	/// Creates a stream for a download.
	/// </summary>
	/// <param name="request">The stream creation query parameters.</param>
	/// <param name="cancellationToken">The token used to cancel the operation.</param>
	/// <returns>The TorBox response containing the stream URL.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">Thrown when the supplied stream type is whitespace.</exception>
	Task<TorBoxResponse<string>> CreateStreamAsync(
		CreateStreamRequest request,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves stream data using the supplied stream tokens.
	/// </summary>
	/// <param name="request">The stream data query parameters.</param>
	/// <param name="cancellationToken">The token used to cancel the operation.</param>
	/// <returns>The TorBox response containing typed stream data.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">Thrown when a required token is whitespace.</exception>
	Task<TorBoxResponse<StreamData>> GetStreamDataAsync(
		GetStreamDataRequest request,
		CancellationToken cancellationToken = default);
}
