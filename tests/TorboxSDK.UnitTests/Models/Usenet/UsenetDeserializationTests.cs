using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Usenet;

namespace TorboxSDK.UnitTests.Models.Usenet;

public sealed class UsenetDeserializationTests
{
    // ──── UsenetDownload ────

    [Fact]
    public void UsenetDownload_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": 3001,
                "auth_id": "auth-usenet-user-001",
                "hash": "usenet-hash-xyz789",
                "name": "Ubuntu.24.04.LTS.Desktop.nzb",
                "size": 4700000000,
                "active": true,
                "created_at": "2025-06-10T16:00:00Z",
                "updated_at": "2025-06-10T16:45:00Z",
                "expires_at": "2025-07-10T16:00:00Z",
                "download_state": "downloading",
                "download_speed": 20971520,
                "upload_speed": 0,
                "progress": 0.65,
                "availability": 0.98,
                "eta": 1200,
                "download_finished": false,
                "download_present": false,
                "inactive_check": 300,
                "server": 5,
                "files": [
                    {
                        "id": 4001,
                        "md5": "098f6bcd4621d373cade4e832627b4f6",
                        "mimetype": "application/x-iso9660-image",
                        "name": "ubuntu-24.04-desktop-amd64.iso",
                        "s3_path": "downloads/usenet/ubuntu.iso",
                        "short_name": "ubuntu.iso",
                        "size": 4700000000,
                        "hash": "usenet-hash-xyz789",
                        "zipped": false,
                        "infected": false,
                        "absolute_path": "/mnt/storage/usenet/ubuntu.iso",
                        "opensubtitles_hash": "a1b2c3d4e5f6a7b8"
                    },
                    {
                        "id": 4002,
                        "md5": null,
                        "mimetype": "text/plain",
                        "name": "README.txt",
                        "s3_path": null,
                        "short_name": "README.txt",
                        "size": 2048,
                        "hash": "usenet-hash-xyz789",
                        "zipped": false,
                        "infected": false,
                        "absolute_path": null,
                        "opensubtitles_hash": null
                    }
                ]
            }
            """;

        // Act
        UsenetDownload? result = JsonSerializer.Deserialize<UsenetDownload>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3001L, result.Id);
        Assert.Equal("auth-usenet-user-001", result.AuthId);
        Assert.Equal("usenet-hash-xyz789", result.Hash);
        Assert.Equal("Ubuntu.24.04.LTS.Desktop.nzb", result.Name);
        Assert.Equal(4700000000L, result.Size);
        Assert.True(result.Active);
        Assert.Equal(new DateTimeOffset(2025, 6, 10, 16, 0, 0, TimeSpan.Zero), result.CreatedAt);
        Assert.Equal(new DateTimeOffset(2025, 6, 10, 16, 45, 0, TimeSpan.Zero), result.UpdatedAt);
        Assert.Equal(new DateTimeOffset(2025, 7, 10, 16, 0, 0, TimeSpan.Zero), result.ExpiresAt);
        Assert.Equal("downloading", result.DownloadState);
        Assert.Equal(20971520L, result.DownloadSpeed);
        Assert.Equal(0L, result.UploadSpeed);
        Assert.Equal(0.65, result.Progress);
        Assert.Equal(0.98, result.Availability);
        Assert.Equal(1200L, result.Eta);
        Assert.False(result.DownloadFinished);
        Assert.False(result.DownloadPresent);
        Assert.Equal(300L, result.InactiveCheck);
        Assert.Equal(5, result.Server);
        Assert.Equal(2, result.Files.Count);
        Assert.Equal(4001L, result.Files[0].Id);
        Assert.Equal("098f6bcd4621d373cade4e832627b4f6", result.Files[0].Md5);
        Assert.Equal("application/x-iso9660-image", result.Files[0].Mimetype);
        Assert.Equal("ubuntu-24.04-desktop-amd64.iso", result.Files[0].Name);
        Assert.Equal(4700000000L, result.Files[0].Size);
        Assert.False(result.Files[0].Zipped);
        Assert.False(result.Files[0].Infected);
        Assert.Equal("a1b2c3d4e5f6a7b8", result.Files[0].OpensubtitlesHash);
        Assert.Equal(4002L, result.Files[1].Id);
        Assert.Null(result.Files[1].Md5);
        Assert.Equal("text/plain", result.Files[1].Mimetype);
        Assert.Equal("README.txt", result.Files[1].Name);
        Assert.Equal(2048L, result.Files[1].Size);
    }

    [Fact]
    public void UsenetDownload_Deserialize_WithNullOptionals_DefaultsCorrectly()
    {
        // Arrange
        string json = """
            {
                "id": 1,
                "name": "minimal.nzb",
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
        UsenetDownload? result = JsonSerializer.Deserialize<UsenetDownload>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1L, result.Id);
        Assert.Null(result.AuthId);
        Assert.Null(result.Hash);
        Assert.Equal("minimal.nzb", result.Name);
        Assert.Null(result.CreatedAt);
        Assert.Null(result.UpdatedAt);
        Assert.Null(result.ExpiresAt);
        Assert.Null(result.DownloadState);
        Assert.Empty(result.Files);
    }
}
