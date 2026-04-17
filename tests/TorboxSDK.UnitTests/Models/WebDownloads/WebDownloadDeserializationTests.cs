using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.WebDownloads;

namespace TorboxSDK.UnitTests.Models.WebDownloads;

public sealed class WebDownloadDeserializationTests
{
    // ──── WebDownload ────

    [Fact]
    public void WebDownload_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": 5001,
                "auth_id": "auth-web-user-001",
                "hash": "web-hash-abc123",
                "name": "document-pack.zip",
                "size": 524288000,
                "active": true,
                "created_at": "2025-05-20T08:00:00Z",
                "updated_at": "2025-05-20T08:30:00Z",
                "expires_at": "2025-06-20T08:00:00Z",
                "download_state": "completed",
                "download_speed": 10485760,
                "upload_speed": 0,
                "progress": 1.0,
                "availability": 1.0,
                "eta": 0,
                "download_finished": true,
                "download_present": true,
                "inactive_check": 600,
                "server": 3,
                "error": null,
                "files": [
                    {
                        "id": 2001,
                        "md5": "e99a18c428cb38d5f260853678922e03",
                        "mimetype": "application/zip",
                        "name": "document-pack.zip",
                        "s3_path": "downloads/web/doc-pack.zip",
                        "short_name": "doc-pack.zip",
                        "size": 524288000,
                        "hash": "web-hash-abc123",
                        "zipped": true,
                        "infected": false,
                        "absolute_path": "/mnt/storage/web/doc-pack.zip",
                        "opensubtitles_hash": null
                    }
                ]
            }
            """;

        // Act
        WebDownload? result = JsonSerializer.Deserialize<WebDownload>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5001L, result.Id);
        Assert.Equal("auth-web-user-001", result.AuthId);
        Assert.Equal("web-hash-abc123", result.Hash);
        Assert.Equal("document-pack.zip", result.Name);
        Assert.Equal(524288000L, result.Size);
        Assert.True(result.Active);
        Assert.Equal(new DateTimeOffset(2025, 5, 20, 8, 0, 0, TimeSpan.Zero), result.CreatedAt);
        Assert.Equal(new DateTimeOffset(2025, 5, 20, 8, 30, 0, TimeSpan.Zero), result.UpdatedAt);
        Assert.Equal(new DateTimeOffset(2025, 6, 20, 8, 0, 0, TimeSpan.Zero), result.ExpiresAt);
        Assert.Equal("completed", result.DownloadState);
        Assert.Equal(10485760L, result.DownloadSpeed);
        Assert.Equal(0L, result.UploadSpeed);
        Assert.Equal(1.0, result.Progress);
        Assert.Equal(1.0, result.Availability);
        Assert.Equal(0L, result.Eta);
        Assert.True(result.DownloadFinished);
        Assert.True(result.DownloadPresent);
        Assert.Equal(600L, result.InactiveCheck);
        Assert.Equal(3, result.Server);
        Assert.Null(result.Error);
        Assert.Single(result.Files);
        Assert.Equal(2001L, result.Files[0].Id);
        Assert.Equal("application/zip", result.Files[0].Mimetype);
        Assert.Equal("document-pack.zip", result.Files[0].Name);
        Assert.Equal(524288000L, result.Files[0].Size);
        Assert.True(result.Files[0].Zipped);
        Assert.False(result.Files[0].Infected);
    }

    [Fact]
    public void WebDownload_Deserialize_WithNullOptionals_DefaultsCorrectly()
    {
        // Arrange
        string json = """
            {
                "id": 1,
                "name": "minimal.bin",
                "size": 0,
                "active": false,
                "download_speed": 0,
                "upload_speed": 0,
                "progress": 0.0,
                "availability": 0.0,
                "eta": 0,
                "download_finished": false,
                "download_present": false,
                "inactive_check": 0,
                "server": 0
            }
            """;

        // Act
        WebDownload? result = JsonSerializer.Deserialize<WebDownload>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1L, result.Id);
        Assert.Null(result.AuthId);
        Assert.Null(result.Hash);
        Assert.Equal("minimal.bin", result.Name);
        Assert.Null(result.CreatedAt);
        Assert.Null(result.UpdatedAt);
        Assert.Null(result.ExpiresAt);
        Assert.Null(result.DownloadState);
        Assert.Null(result.Error);
        Assert.Empty(result.Files);
    }

    // ──── Hoster ────

    [Fact]
    public void Hoster_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": 10,
                "name": "Mega",
                "domains": ["mega.nz", "mega.co.nz"],
                "url": "https://mega.nz",
                "icon": "https://mega.nz/favicon.ico",
                "status": true,
                "type": "hoster",
                "note": "50GB daily limit",
                "nsfw": false,
                "daily_link_limit": 100,
                "daily_link_used": 23,
                "daily_bandwidth_limit": 53687091200,
                "daily_bandwidth_used": 10737418240,
                "per_link_size_limit": 5368709120,
                "regex": "https?://(www\\.)?mega\\.nz/.*"
            }
            """;

        // Act
        Hoster? result = JsonSerializer.Deserialize<Hoster>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Mega", result.Name);
        Assert.Equal(2, result.Domains.Count);
        Assert.Equal("mega.nz", result.Domains[0]);
        Assert.Equal("mega.co.nz", result.Domains[1]);
        Assert.Equal("https://mega.nz", result.Url);
        Assert.Equal("https://mega.nz/favicon.ico", result.Icon);
        Assert.True(result.Status);
        Assert.Equal("hoster", result.Type);
        Assert.Equal("50GB daily limit", result.Note);
        Assert.False(result.Nsfw);
        Assert.Equal(100, result.DailyLinkLimit);
        Assert.Equal(23, result.DailyLinkUsed);
        Assert.Equal(53687091200L, result.DailyBandwidthLimit);
        Assert.Equal(10737418240L, result.DailyBandwidthUsed);
        Assert.Equal(5368709120L, result.PerLinkSizeLimit);
        Assert.Equal("https?://(www\\.)?mega\\.nz/.*", result.Regex);
    }

    [Fact]
    public void Hoster_Deserialize_WithNullOptionals_DefaultsCorrectly()
    {
        // Arrange
        string json = """
            {
                "id": 1,
                "name": "Unknown Hoster",
                "status": false,
                "nsfw": false,
                "daily_link_limit": 0,
                "daily_link_used": 0
            }
            """;

        // Act
        Hoster? result = JsonSerializer.Deserialize<Hoster>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Unknown Hoster", result.Name);
        Assert.Empty(result.Domains);
        Assert.Null(result.Url);
        Assert.Null(result.Icon);
        Assert.False(result.Status);
        Assert.Null(result.Type);
        Assert.Null(result.Note);
        Assert.Null(result.DailyBandwidthLimit);
        Assert.Null(result.DailyBandwidthUsed);
        Assert.Null(result.PerLinkSizeLimit);
        Assert.Null(result.Regex);
    }
}
