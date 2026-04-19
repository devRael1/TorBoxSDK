using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.User;

namespace TorboxSDK.UnitTests.Models.User;

public sealed class UserModelTests
{
	// ──── EditSettingsRequest ────

	[Fact]
	public void EditSettingsRequest_Serialize_WithSelectedProperties_ProducesExpectedJson()
	{
		// Arrange
		EditSettingsRequest request = new()
		{
			EmailNotifications = true,
			WebhookUrl = "https://hooks.example.com/notify",
			SeedTorrents = 2,
			DashboardSort = "date_desc",
			WebPlayerAlwaysTranscode = false,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.True(root.GetProperty("email_notifications").GetBoolean());
		Assert.Equal("https://hooks.example.com/notify", root.GetProperty("webhook_url").GetString());
		Assert.Equal(2, root.GetProperty("seed_torrents").GetInt32());
		Assert.Equal("date_desc", root.GetProperty("dashboard_sort").GetString());
		Assert.False(root.GetProperty("web_player_always_transcode").GetBoolean());
	}

	[Fact]
	public void EditSettingsRequest_Serialize_WithNullOptionals_OmitsNullProperties()
	{
		// Arrange
		EditSettingsRequest request = new();

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.False(root.TryGetProperty("email_notifications", out _));
		Assert.False(root.TryGetProperty("web_notifications", out _));
		Assert.False(root.TryGetProperty("webhook_url", out _));
		Assert.False(root.TryGetProperty("seed_torrents", out _));
		Assert.False(root.TryGetProperty("dashboard_sort", out _));
		Assert.False(root.TryGetProperty("web_player_always_transcode", out _));
		Assert.False(root.TryGetProperty("allow_zipped", out _));
		Assert.False(root.TryGetProperty("cdn_selection", out _));
		Assert.False(root.TryGetProperty("telegram_id", out _));
		Assert.False(root.TryGetProperty("discord_id", out _));
	}

	[Fact]
	public void EditSettingsRequest_Serialize_WithStremioProperties_ProducesExpectedJson()
	{
		// Arrange
		EditSettingsRequest request = new()
		{
			StremioQuality = [1, 2, 3],
			StremioResolution = [720, 1080],
			StremioSizeLower = 100000000L,
			StremioSizeUpper = 5000000000L,
			StremioAllowAdult = false,
			StremioSort = "quality",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;

		JsonElement quality = root.GetProperty("stremio_quality");
		Assert.Equal(3, quality.GetArrayLength());
		Assert.Equal(1, quality[0].GetInt32());

		JsonElement resolution = root.GetProperty("stremio_resolution");
		Assert.Equal(2, resolution.GetArrayLength());
		Assert.Equal(720, resolution[0].GetInt32());

		Assert.Equal(100000000L, root.GetProperty("stremio_size_lower").GetInt64());
		Assert.Equal(5000000000L, root.GetProperty("stremio_size_upper").GetInt64());
		Assert.False(root.GetProperty("stremio_allow_adult").GetBoolean());
		Assert.Equal("quality", root.GetProperty("stremio_sort").GetString());
	}

	// ──── DeleteAccountRequest ────

	[Fact]
	public void DeleteAccountRequest_Serialize_ProducesExpectedJson()
	{
		// Arrange
		DeleteAccountRequest request = new()
		{
			SessionToken = "session-abc-123",
			ConfirmationCode = 987654,
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("session-abc-123", root.GetProperty("session_token").GetString());
		Assert.Equal(987654, root.GetProperty("confirmation_code").GetInt32());
	}

	// ──── DeviceTokenRequest ────

	[Fact]
	public void DeviceTokenRequest_Serialize_ProducesExpectedJson()
	{
		// Arrange
		DeviceTokenRequest request = new()
		{
			DeviceCode = "device-code-xyz-789",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("device-code-xyz-789", root.GetProperty("device_code").GetString());
	}

	// ──── RefreshTokenRequest ────

	[Fact]
	public void RefreshTokenRequest_Serialize_ProducesExpectedJson()
	{
		// Arrange
		RefreshTokenRequest request = new()
		{
			SessionToken = "current-session-token-abc",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("current-session-token-abc", root.GetProperty("session_token").GetString());
	}
}
