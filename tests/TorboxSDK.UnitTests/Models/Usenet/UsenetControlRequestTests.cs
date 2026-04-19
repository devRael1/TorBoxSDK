using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Usenet;

namespace TorboxSDK.UnitTests.Models.Usenet;

public sealed class UsenetControlRequestTests
{
	// ──── ControlUsenetDownloadRequest ────

	[Fact]
	public void ControlUsenetDownloadRequest_Serialize_WithAllProperties_ProducesExpectedJson()
	{
		// Arrange
		ControlUsenetDownloadRequest request = new()
		{
			UsenetId = 88,
			Operation = ControlOperation.Resume,
			All = false,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal(88, root.GetProperty("usenet_id").GetInt64());
		Assert.True(root.TryGetProperty("operation", out _));
		Assert.False(root.GetProperty("all").GetBoolean());
	}

	[Fact]
	public void ControlUsenetDownloadRequest_Serialize_WithNullOptionals_OmitsNullProperties()
	{
		// Arrange
		ControlUsenetDownloadRequest request = new()
		{
			Operation = ControlOperation.Delete,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.False(root.TryGetProperty("usenet_id", out _));
		Assert.False(root.TryGetProperty("all", out _));
		Assert.True(root.TryGetProperty("operation", out _));
	}

	// ──── CheckUsenetCachedRequest ────

	[Fact]
	public void CheckUsenetCachedRequest_Serialize_WithAllProperties_ProducesExpectedJson()
	{
		// Arrange
		CheckUsenetCachedRequest request = new()
		{
			Hashes = ["nzb-hash-1", "nzb-hash-2"],
			Format = "list",
			ListFiles = true,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;

		JsonElement hashes = root.GetProperty("hashes");
		Assert.Equal(2, hashes.GetArrayLength());
		Assert.Equal("nzb-hash-1", hashes[0].GetString());
		Assert.Equal("nzb-hash-2", hashes[1].GetString());

		Assert.Equal("list", root.GetProperty("format").GetString());
		Assert.True(root.GetProperty("list_files").GetBoolean());
	}

	[Fact]
	public void CheckUsenetCachedRequest_Serialize_WithNullOptionals_OmitsNullProperties()
	{
		// Arrange
		CheckUsenetCachedRequest request = new()
		{
			Hashes = ["nzb-hash-1"],
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.True(root.TryGetProperty("hashes", out _));
		Assert.False(root.TryGetProperty("format", out _));
		Assert.False(root.TryGetProperty("list_files", out _));
	}
}
