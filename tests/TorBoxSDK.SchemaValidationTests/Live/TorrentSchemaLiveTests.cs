using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;
using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.Live;

/// <summary>
/// Live schema tests for the Torrents resource client.
/// Calls the real TorBox API and verifies that every JSON field in the response
/// is mapped by a <c>[JsonPropertyName]</c> attribute in the SDK model.
/// </summary>
/// <remarks>
/// These tests are skipped when the <c>TORBOX_API_KEY</c> environment variable is not set.
/// Run them with a valid key to detect API drift not captured by the static OpenAPI tests.
/// </remarks>
[Collection("SchemaLive")]
[Trait("Category", "Live")]
public sealed class TorrentSchemaLiveTests(SchemaLiveTestFixture fixture)
{
	private readonly SchemaLiveTestFixture _fixture = fixture;

	[SkippableFact]
	public async Task GetMyTorrentList_ResponseFields_AllMappedInSdkModel()
	{
		Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

		// Arrange
		using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

		// Act
		using HttpResponseMessage response = await _fixture.HttpClient
			.GetAsync("/v1/api/torrents/mylist", cts.Token)
			.ConfigureAwait(false);

		response.EnsureSuccessStatusCode();

		string json = await response.Content
			.ReadAsStringAsync(cts.Token)
			.ConfigureAwait(false);

		IReadOnlyList<string> unmapped =
			UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<Torrent>>>(json);

		// Assert
		Assert.True(
			unmapped.Count == 0,
			BuildMessage("GET /v1/api/torrents/mylist", typeof(Torrent), unmapped));
	}

	[SkippableFact]
	public async Task GetTorrentInfo_ResponseFields_AllMappedInSdkModel()
	{
		Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

		// Arrange
		using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

		// Act
		using HttpResponseMessage response = await _fixture.HttpClient
			.GetAsync("/v1/api/torrents/torrentinfo?hash=3b245504cf5f11bbdbe1201cea6a6bf45aee1bc0", cts.Token)
			.ConfigureAwait(false);

		response.EnsureSuccessStatusCode();

		string json = await response.Content
			.ReadAsStringAsync(cts.Token)
			.ConfigureAwait(false);

		IReadOnlyList<string> unmapped =
			UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<TorrentInfo>>(json);

		// Assert
		Assert.True(
			unmapped.Count == 0,
			BuildMessage("GET /v1/api/torrents/torrentinfo", typeof(TorrentInfo), unmapped));
	}

	private static string BuildMessage(
		string endpoint,
		Type modelType,
		IReadOnlyList<string> unmapped) =>
		$"Endpoint '{endpoint}' returned {unmapped.Count} unmapped field path(s) " +
		$"not covered by '{modelType.Name}' (including nested types):{Environment.NewLine}" +
		string.Join(Environment.NewLine, unmapped.Select(f => $"  - {f}"));
}
