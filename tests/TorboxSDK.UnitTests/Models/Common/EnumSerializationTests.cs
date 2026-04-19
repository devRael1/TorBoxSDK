using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;
using TorBoxSDK.Models.Torrents;

namespace TorboxSDK.UnitTests.Models.Common;

public sealed class EnumSerializationTests
{
	// ──── DownloadStatus ────
	// TorBoxJsonOptions.Default includes a global JsonStringEnumConverter with
	// SnakeCaseLower naming policy, so all enums serialize as lowercase snake_case strings.

	[Theory]
	[InlineData(DownloadStatus.Downloading, "downloading")]
	[InlineData(DownloadStatus.Completed, "completed")]
	[InlineData(DownloadStatus.Cached, "cached")]
	[InlineData(DownloadStatus.Error, "error")]
	[InlineData(DownloadStatus.Queued, "queued")]
	[InlineData(DownloadStatus.Metadl, "metadl")]
	public void DownloadStatus_Serialize_ProducesSnakeCaseString(DownloadStatus status, string expected)
	{
		// Arrange & Act
		string json = JsonSerializer.Serialize(status, TorBoxJsonOptions.Default);

		// Assert
		Assert.Equal($"\"{expected}\"", json);
	}

	[Theory]
	[InlineData("\"downloading\"", DownloadStatus.Downloading)]
	[InlineData("\"completed\"", DownloadStatus.Completed)]
	[InlineData("\"cached\"", DownloadStatus.Cached)]
	public void DownloadStatus_Deserialize_ParsesCorrectly(string json, DownloadStatus expected)
	{
		// Arrange & Act
		DownloadStatus result = JsonSerializer.Deserialize<DownloadStatus>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.Equal(expected, result);
	}

	// ──── SearchType ────

	[Theory]
	[InlineData(SearchType.Torrent, "torrent")]
	[InlineData(SearchType.Usenet, "usenet")]
	[InlineData(SearchType.Meta, "meta")]
	public void SearchType_Serialize_ProducesSnakeCaseString(SearchType searchType, string expected)
	{
		// Arrange & Act
		string json = JsonSerializer.Serialize(searchType, TorBoxJsonOptions.Default);

		// Assert
		Assert.Equal($"\"{expected}\"", json);
	}

	[Theory]
	[InlineData("\"torrent\"", SearchType.Torrent)]
	[InlineData("\"usenet\"", SearchType.Usenet)]
	[InlineData("\"meta\"", SearchType.Meta)]
	public void SearchType_Deserialize_ParsesCorrectly(string json, SearchType expected)
	{
		// Arrange & Act
		SearchType result = JsonSerializer.Deserialize<SearchType>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.Equal(expected, result);
	}

	// ──── SeedPreference ────
	// Also string-serialized via the global converter in TorBoxJsonOptions.Default.

	[Theory]
	[InlineData(SeedPreference.Auto, "auto")]
	[InlineData(SeedPreference.Seed, "seed")]
	[InlineData(SeedPreference.NoSeed, "no_seed")]
	public void SeedPreference_Serialize_ProducesSnakeCaseString(SeedPreference pref, string expected)
	{
		// Arrange & Act
		string json = JsonSerializer.Serialize(pref, TorBoxJsonOptions.Default);

		// Assert
		Assert.Equal($"\"{expected}\"", json);
	}

	[Theory]
	[InlineData("\"auto\"", SeedPreference.Auto)]
	[InlineData("\"seed\"", SeedPreference.Seed)]
	[InlineData("\"no_seed\"", SeedPreference.NoSeed)]
	public void SeedPreference_Deserialize_ParsesCorrectly(string json, SeedPreference expected)
	{
		// Arrange & Act
		SeedPreference result = JsonSerializer.Deserialize<SeedPreference>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.Equal(expected, result);
	}
}
