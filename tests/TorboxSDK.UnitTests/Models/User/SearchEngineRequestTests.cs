using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.User;

namespace TorboxSDK.UnitTests.Models.User;

public sealed class SearchEngineRequestTests
{
	[Fact]
	public void AddSearchEnginesRequest_Serialize_ProducesExpectedJson()
	{
		// Arrange
		AddSearchEnginesRequest request = new()
		{
			Type = "torznab",
			Url = "https://indexer.example.com/api",
			Apikey = "my-api-key-123",
			DownloadType = "torrent",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("torznab", root.GetProperty("type").GetString());
		Assert.Equal("https://indexer.example.com/api", root.GetProperty("url").GetString());
		Assert.Equal("my-api-key-123", root.GetProperty("apikey").GetString());
		Assert.Equal("torrent", root.GetProperty("download_type").GetString());
	}

	[Fact]
	public void AddSearchEnginesRequest_Serialize_WithNullOptionals_OmitsThem()
	{
		// Arrange
		AddSearchEnginesRequest request = new()
		{
			Type = "torznab",
			Url = "https://indexer.example.com",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("torznab", root.GetProperty("type").GetString());
		Assert.Equal("https://indexer.example.com", root.GetProperty("url").GetString());
		Assert.False(root.TryGetProperty("apikey", out _));
		Assert.False(root.TryGetProperty("download_type", out _));
	}

	[Fact]
	public void ModifySearchEnginesRequest_Serialize_IncludesIdAndAllProperties()
	{
		// Arrange
		ModifySearchEnginesRequest request = new()
		{
			Id = 42,
			Type = "newznab",
			Url = "https://usenet-indexer.example.com/api",
			Apikey = "updated-key",
			DownloadType = "usenet",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal(42, root.GetProperty("id").GetInt64());
		Assert.Equal("newznab", root.GetProperty("type").GetString());
		Assert.Equal("https://usenet-indexer.example.com/api", root.GetProperty("url").GetString());
		Assert.Equal("updated-key", root.GetProperty("apikey").GetString());
		Assert.Equal("usenet", root.GetProperty("download_type").GetString());
	}

	[Fact]
	public void ModifySearchEnginesRequest_Serialize_WithNullOptionals_OmitsThem()
	{
		// Arrange
		ModifySearchEnginesRequest request = new()
		{
			Id = 10,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal(10, root.GetProperty("id").GetInt64());
		Assert.False(root.TryGetProperty("type", out _));
		Assert.False(root.TryGetProperty("url", out _));
		Assert.False(root.TryGetProperty("apikey", out _));
		Assert.False(root.TryGetProperty("download_type", out _));
	}

	[Fact]
	public void ControlSearchEnginesRequest_Serialize_WithAllProperties_ProducesExpectedJson()
	{
		// Arrange
		ControlSearchEnginesRequest request = new()
		{
			Operation = "delete",
			Id = 42,
			All = false,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("delete", root.GetProperty("operation").GetString());
		Assert.Equal(42, root.GetProperty("id").GetInt64());
		Assert.False(root.GetProperty("all").GetBoolean());
	}

	[Fact]
	public void ControlSearchEnginesRequest_Serialize_WithAllTrue_ProducesExpectedJson()
	{
		// Arrange
		ControlSearchEnginesRequest request = new()
		{
			Operation = "enable",
			All = true,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("enable", root.GetProperty("operation").GetString());
		Assert.True(root.GetProperty("all").GetBoolean());
		Assert.False(root.TryGetProperty("id", out _));
	}

	[Fact]
	public void ControlSearchEnginesRequest_Serialize_WithNullOptionals_OmitsThem()
	{
		// Arrange
		ControlSearchEnginesRequest request = new()
		{
			Operation = "disable",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("disable", root.GetProperty("operation").GetString());
		Assert.False(root.TryGetProperty("id", out _));
		Assert.False(root.TryGetProperty("all", out _));
	}

	[Fact]
	public void AddSearchEnginesRequest_Deserialize_PopulatesProperties()
	{
		// Arrange
		string json = """
            {
                "type": "torznab",
                "url": "https://indexer.example.com",
                "apikey": "key123",
                "download_type": "torrent"
            }
            """;

		// Act
		AddSearchEnginesRequest? result = JsonSerializer.Deserialize<AddSearchEnginesRequest>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("torznab", result.Type);
		Assert.Equal("https://indexer.example.com", result.Url);
		Assert.Equal("key123", result.Apikey);
		Assert.Equal("torrent", result.DownloadType);
	}

	[Fact]
	public void ModifySearchEnginesRequest_Deserialize_PopulatesProperties()
	{
		// Arrange
		string json = """
            {
                "id": 99,
                "type": "newznab",
                "url": "https://usenet.example.com",
                "apikey": "usenet-key",
                "download_type": "usenet"
            }
            """;

		// Act
		ModifySearchEnginesRequest? result = JsonSerializer.Deserialize<ModifySearchEnginesRequest>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(99, result.Id);
		Assert.Equal("newznab", result.Type);
		Assert.Equal("https://usenet.example.com", result.Url);
		Assert.Equal("usenet-key", result.Apikey);
		Assert.Equal("usenet", result.DownloadType);
	}
}
