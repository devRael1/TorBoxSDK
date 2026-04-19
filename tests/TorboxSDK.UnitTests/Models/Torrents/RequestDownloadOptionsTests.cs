using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Torrents;

namespace TorboxSDK.UnitTests.Models.Torrents;

public sealed class RequestDownloadOptionsTests
{
	[Fact]
	public void Serialize_WithAllNewProperties_ProducesExpectedJson()
	{
		// Arrange
		RequestDownloadOptions options = new()
		{
			FileId = 5,
			ZipLink = true,
			UserIp = "192.168.1.1",
			Redirect = false,
			Token = "my-api-token",
			AppendName = true,
		};

		// Act
		string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("my-api-token", root.GetProperty("token").GetString());
		Assert.True(root.GetProperty("append_name").GetBoolean());
		Assert.False(root.GetProperty("redirect").GetBoolean());
	}

	[Fact]
	public void Serialize_WithNullNewProperties_OmitsThem()
	{
		// Arrange
		RequestDownloadOptions options = new();

		// Act
		string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.False(root.TryGetProperty("token", out _));
		Assert.False(root.TryGetProperty("append_name", out _));
	}
}
