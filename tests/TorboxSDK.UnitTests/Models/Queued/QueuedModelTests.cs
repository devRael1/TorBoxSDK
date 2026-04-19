using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Queued;

namespace TorboxSDK.UnitTests.Models.Queued;

public sealed class QueuedModelTests
{
	// ──── ControlQueuedRequest ────

	[Fact]
	public void ControlQueuedRequest_Serialize_WithAllProperties_ProducesExpectedJson()
	{
		// Arrange
		ControlQueuedRequest request = new()
		{
			QueuedId = 99,
			Operation = ControlOperation.Delete,
			All = false,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal(99, root.GetProperty("queued_id").GetInt64());
		Assert.True(root.TryGetProperty("operation", out _));
		Assert.False(root.GetProperty("all").GetBoolean());
	}

	[Fact]
	public void ControlQueuedRequest_Serialize_WithNullOptionals_OmitsNullProperties()
	{
		// Arrange
		ControlQueuedRequest request = new()
		{
			Operation = ControlOperation.Resume,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.False(root.TryGetProperty("queued_id", out _));
		Assert.False(root.TryGetProperty("all", out _));
		Assert.True(root.TryGetProperty("operation", out _));
	}

	// ──── QueuedDownload ────

	[Fact]
	public void QueuedDownload_Deserialize_PopulatesAllProperties()
	{
		// Arrange
		string json = """
            {
                "id": 42,
                "created_at": "2025-03-10T09:00:00Z",
                "magnet": "magnet:?xt=urn:btih:abc123def456",
                "torrent_file": "/path/to/file.torrent",
                "hash": "abc123def456",
                "name": "Ubuntu 24.04 LTS",
                "type": "torrent",
                "name_override": "ubuntu-custom",
                "seed_torrent_override": 1
            }
            """;

		// Act
		QueuedDownload? result = JsonSerializer.Deserialize<QueuedDownload>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(42L, result.Id);
		Assert.NotNull(result.CreatedAt);
		Assert.Equal(new DateTimeOffset(2025, 3, 10, 9, 0, 0, TimeSpan.Zero), result.CreatedAt);
		Assert.Equal("magnet:?xt=urn:btih:abc123def456", result.Magnet);
		Assert.Equal("/path/to/file.torrent", result.TorrentFile);
		Assert.Equal("abc123def456", result.Hash);
		Assert.Equal("Ubuntu 24.04 LTS", result.Name);
		Assert.Equal("torrent", result.Type);
		Assert.Equal("ubuntu-custom", result.NameOverride);
		Assert.Equal(1, result.SeedTorrentOverride);
	}

	[Fact]
	public void QueuedDownload_Deserialize_WithNullOptionals_ReturnsNulls()
	{
		// Arrange
		string json = """
            {
                "id": 1
            }
            """;

		// Act
		QueuedDownload? result = JsonSerializer.Deserialize<QueuedDownload>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(1L, result.Id);
		Assert.Null(result.CreatedAt);
		Assert.Null(result.Magnet);
		Assert.Null(result.TorrentFile);
		Assert.Null(result.Hash);
		Assert.Null(result.Name);
		Assert.Null(result.Type);
		Assert.Null(result.NameOverride);
		Assert.Null(result.SeedTorrentOverride);
	}

	// ──── GetQueuedOptions ────

	[Fact]
	public void GetQueuedOptions_Serialize_WithAllProperties_ProducesExpectedJson()
	{
		// Arrange
		GetQueuedOptions options = new()
		{
			Id = 5,
			Offset = 10,
			Limit = 50,
			BypassCache = true,
			Type = "torrent",
		};

		// Act
		string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal(5, root.GetProperty("id").GetInt64());
		Assert.Equal(10, root.GetProperty("offset").GetInt32());
		Assert.Equal(50, root.GetProperty("limit").GetInt32());
		Assert.True(root.GetProperty("bypass_cache").GetBoolean());
		Assert.Equal("torrent", root.GetProperty("type").GetString());
	}

	[Fact]
	public void GetQueuedOptions_Serialize_WithNullOptionals_OmitsNullProperties()
	{
		// Arrange
		GetQueuedOptions options = new();

		// Act
		string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.False(root.TryGetProperty("id", out _));
		Assert.False(root.TryGetProperty("offset", out _));
		Assert.False(root.TryGetProperty("limit", out _));
		Assert.False(root.TryGetProperty("bypass_cache", out _));
		Assert.False(root.TryGetProperty("type", out _));
	}
}
