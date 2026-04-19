using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;
using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.Live;

/// <summary>
/// Live schema tests for the Search endpoints.
/// Calls the real TorBox API and verifies that every JSON field in the response
/// is mapped by a <c>[JsonPropertyName]</c> attribute in the SDK model.
/// </summary>
[Collection("SchemaLive")]
[Trait("Category", "Live")]
public sealed class SearchSchemaLiveTests(SchemaLiveTestFixture fixture)
{
	private readonly SchemaLiveTestFixture _fixture = fixture;

	[SkippableFact]
	public async Task SearchTorrents_ResponseFields_AllMappedInSdkModel()
	{
		Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

		// Arrange
		using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

		// Act
		using HttpResponseMessage response = await _fixture.HttpClient
			.GetAsync("/v1/api/search/torrents/ubuntu", cts.Token)
			.ConfigureAwait(false);

		response.EnsureSuccessStatusCode();

		string json = await response.Content
			.ReadAsStringAsync(cts.Token)
			.ConfigureAwait(false);

		IReadOnlyList<string> unmapped =
			UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<TorrentSearchResponse>>(json);

		// Assert
		Assert.True(
			unmapped.Count == 0,
			BuildMessage("GET /v1/api/search/torrents/ubuntu", typeof(TorrentSearchResponse), unmapped));
	}

	[SkippableFact]
	public async Task SearchUsenet_ResponseFields_AllMappedInSdkModel()
	{
		Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

		// Arrange
		using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

		// Act
		using HttpResponseMessage response = await _fixture.HttpClient
			.GetAsync("/v1/api/search/usenet/ubuntu", cts.Token)
			.ConfigureAwait(false);

		response.EnsureSuccessStatusCode();

		string json = await response.Content
			.ReadAsStringAsync(cts.Token)
			.ConfigureAwait(false);

		IReadOnlyList<string> unmapped =
			UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<UsenetSearchResponse>>(json);

		// Assert
		Assert.True(
			unmapped.Count == 0,
			BuildMessage("GET /v1/api/search/usenet/ubuntu", typeof(UsenetSearchResponse), unmapped));
	}

	[SkippableFact]
	public async Task SearchMeta_ResponseFields_AllMappedInSdkModel()
	{
		Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

		// Arrange
		using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

		// Act
		using HttpResponseMessage response = await _fixture.HttpClient
			.GetAsync("/v1/api/search/meta/inception", cts.Token)
			.ConfigureAwait(false);

		response.EnsureSuccessStatusCode();

		string json = await response.Content
			.ReadAsStringAsync(cts.Token)
			.ConfigureAwait(false);

		IReadOnlyList<string> unmapped = UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<MetaSearchResult>>>(json);

		// Assert
		Assert.True(unmapped.Count == 0, BuildMessage("GET /v1/api/search/meta/inception", typeof(MetaSearchResult), unmapped));
	}

	private static string BuildMessage(
		string endpoint,
		Type modelType,
		IReadOnlyList<string> unmapped) =>
		$"Endpoint '{endpoint}' returned {unmapped.Count} unmapped field path(s) " +
		$"not covered by '{modelType.Name}' (including nested types):{Environment.NewLine}" +
		string.Join(Environment.NewLine, unmapped.Select(f => $"  - {f}"));
}
