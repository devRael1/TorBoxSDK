using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Notifications;

namespace TorboxSDK.UnitTests.Models.Notifications;

public sealed class NotificationModelTests
{
	// ──── Notification ────

	[Fact]
	public void Notification_Deserialize_PopulatesAllProperties()
	{
		// Arrange
		string json = """
            {
                "id": 1001,
                "auth_id": "auth-abc-123",
                "created_at": "2025-02-10T08:15:30Z",
                "title": "Download Complete",
                "message": "Your torrent has finished downloading.",
                "action": "url",
                "action_data": "https://torbox.app/downloads/42",
                "action_cta": "Download Now"
            }
            """;

		// Act
		Notification? result = JsonSerializer.Deserialize<Notification>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(1001L, result.Id);
		Assert.Equal("auth-abc-123", result.AuthId);
		Assert.NotNull(result.CreatedAt);
		Assert.Equal(new DateTimeOffset(2025, 2, 10, 8, 15, 30, TimeSpan.Zero), result.CreatedAt);
		Assert.Equal("Download Complete", result.Title);
		Assert.Equal("Your torrent has finished downloading.", result.Message);
		Assert.Equal("url", result.Action);
		Assert.Equal("https://torbox.app/downloads/42", result.ActionData);
		Assert.Equal("Download Now", result.ActionCta);
	}

	[Fact]
	public void Notification_Deserialize_WithNullOptionals_ReturnsNulls()
	{
		// Arrange
		string json = """
            {
                "id": 500
            }
            """;

		// Act
		Notification? result = JsonSerializer.Deserialize<Notification>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(500L, result.Id);
		Assert.Null(result.AuthId);
		Assert.Null(result.CreatedAt);
		Assert.Null(result.Title);
		Assert.Null(result.Message);
		Assert.Null(result.Action);
		Assert.Null(result.ActionData);
		Assert.Null(result.ActionCta);
	}

	// ──── IntercomHash ────

	[Fact]
	public void IntercomHash_Deserialize_PopulatesHash()
	{
		// Arrange
		string json = """
            {
                "hash": "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6"
            }
            """;

		// Act
		IntercomHash? result = JsonSerializer.Deserialize<IntercomHash>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6", result.Hash);
	}

	[Fact]
	public void IntercomHash_Deserialize_WithNullHash_ReturnsNull()
	{
		// Arrange
		string json = """
            {
                "hash": null
            }
            """;

		// Act
		IntercomHash? result = JsonSerializer.Deserialize<IntercomHash>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Null(result.Hash);
	}


}
