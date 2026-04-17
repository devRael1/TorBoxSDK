using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Integrations;

namespace TorboxSDK.UnitTests.Models.Integrations;

public sealed class IntegrationModelTests
{
    // ──── CreateIntegrationJobRequest ────

    [Fact]
    public void CreateIntegrationJobRequest_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        CreateIntegrationJobRequest request = new()
        {
            DownloadId = 42,
            DownloadType = "torrent",
            FileId = 7,
            Zip = true,
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal(42, root.GetProperty("id").GetInt64());
        Assert.Equal("torrent", root.GetProperty("type").GetString());
        Assert.Equal(7, root.GetProperty("file_id").GetInt64());
        Assert.True(root.GetProperty("zip").GetBoolean());
    }

    [Fact]
    public void CreateIntegrationJobRequest_Serialize_WithNullOptionals_OmitsNullProperties()
    {
        // Arrange
        CreateIntegrationJobRequest request = new();

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("id", out _));
        Assert.False(root.TryGetProperty("type", out _));
        Assert.False(root.TryGetProperty("file_id", out _));
        Assert.False(root.TryGetProperty("zip", out _));
    }

    // ──── IntegrationJob ────

    [Fact]
    public void IntegrationJob_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": 101,
                "auth_id": "auth-abc-123",
                "job_type": "google_drive",
                "status": "completed",
                "progress": 1.0,
                "detail": "Upload finished successfully",
                "created_at": "2025-01-20T12:00:00Z",
                "hash": "a1b2c3d4e5f6",
                "download_id": 55,
                "download_type": "torrent"
            }
            """;

        // Act
        IntegrationJob? result = JsonSerializer.Deserialize<IntegrationJob>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(101L, result.Id);
        Assert.Equal("auth-abc-123", result.AuthId);
        Assert.Equal("google_drive", result.JobType);
        Assert.Equal("completed", result.Status);
        Assert.Equal(1.0, result.Progress);
        Assert.Equal("Upload finished successfully", result.Detail);
        Assert.NotNull(result.CreatedAt);
        Assert.Equal(new DateTimeOffset(2025, 1, 20, 12, 0, 0, TimeSpan.Zero), result.CreatedAt);
        Assert.Equal("a1b2c3d4e5f6", result.Hash);
        Assert.Equal(55L, result.DownloadId);
        Assert.Equal("torrent", result.DownloadType);
    }

    [Fact]
    public void IntegrationJob_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {
                "id": 10,
                "progress": 0.0
            }
            """;

        // Act
        IntegrationJob? result = JsonSerializer.Deserialize<IntegrationJob>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10L, result.Id);
        Assert.Equal(0.0, result.Progress);
        Assert.Null(result.AuthId);
        Assert.Null(result.JobType);
        Assert.Null(result.Status);
        Assert.Null(result.Detail);
        Assert.Null(result.CreatedAt);
        Assert.Null(result.Hash);
        Assert.Null(result.DownloadId);
        Assert.Null(result.DownloadType);
    }

    // ──── LinkedRolesRequest ────

    [Fact]
    public void LinkedRolesRequest_Serialize_ProducesExpectedJson()
    {
        // Arrange
        LinkedRolesRequest request = new()
        {
            DiscordToken = "discord-oauth-token-xyz",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("discord-oauth-token-xyz", root.GetProperty("discord_token").GetString());
    }

    // ──── OAuthRegisterRequest ────

    [Fact]
    public void OAuthRegisterRequest_Serialize_IncludesTokenAndRefreshToken()
    {
        // Arrange
        OAuthRegisterRequest request = new()
        {
            Provider = "google",
            Token = "access-token-123",
            RefreshToken = "refresh-token-456",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("access-token-123", root.GetProperty("token").GetString());
        Assert.Equal("refresh-token-456", root.GetProperty("refresh_token").GetString());
    }

    [Fact]
    public void OAuthRegisterRequest_Serialize_ProviderIsJsonIgnored()
    {
        // Arrange
        OAuthRegisterRequest request = new()
        {
            Provider = "dropbox",
            Token = "token",
            RefreshToken = "refresh",
        };

        // Act
        string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("provider", out _));
    }
}
