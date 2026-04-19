using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.IntegrationTests.Main.Torrents;

/// <summary>
/// Integration tests for the Torrents resource client against the live TorBox API.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
public sealed class TorrentsClientIntegrationTests(TorBoxIntegrationFixture fixture)
{
	private readonly TorBoxIntegrationFixture _fixture = fixture;

	[SkippableFact]
	public async Task GetMyTorrentListAsync_WithValidApiKey_ReturnsResponse()
	{
		Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

		// Arrange
		using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

		// Act
		TorBoxResponse<IReadOnlyList<Torrent>> response = await _fixture.Client.Main.Torrents
			.GetMyTorrentListAsync(cancellationToken: cts.Token);

		// Assert
		Assert.NotNull(response);
		Assert.True(response.Success);
		Assert.NotNull(response.Data);
	}

	[SkippableFact]
	public async Task CheckCachedAsync_WithKnownHash_ReturnsResponse()
	{
		Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

		// Arrange
		using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));
		// Ubuntu 24.04 LTS torrent info hash
		IReadOnlyList<string> hashes = ["3b245504cf5f11bbdbe1201cea6a6bf45aee1bc0"];

		// Act
		TorBoxResponse<CheckCached> response = await _fixture.Client.Main.Torrents
			.CheckCachedAsync(hashes, cancellationToken: cts.Token);

		// Assert
		Assert.NotNull(response);
		Assert.True(response.Success);
	}

	[SkippableFact]
	public async Task GetTorrentInfoAsync_WithKnownHash_ReturnsTorrentInfo()
	{
		Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

		// Arrange
		using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));
		// Ubuntu 24.04 LTS torrent info hash
		string hash = "3b245504cf5f11bbdbe1201cea6a6bf45aee1bc0";

		// Act
		TorBoxResponse<TorrentInfo> response = await _fixture.Client.Main.Torrents
			.GetTorrentInfoAsync(hash, cancellationToken: cts.Token);

		// Assert
		Assert.NotNull(response);
		Assert.True(response.Success);
		Assert.NotNull(response.Data);
	}

}
