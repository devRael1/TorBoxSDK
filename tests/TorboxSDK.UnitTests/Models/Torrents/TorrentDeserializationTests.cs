using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Torrents;

namespace TorboxSDK.UnitTests.Models.Torrents;

public sealed class TorrentDeserializationTests
{
    // ──── Torrent ────

    [Fact]
    public void Deserialize_WithFullPayload_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": 12345,
                "auth_id": "auth-user-001",
                "hash": "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0",
                "name": "Ubuntu 24.04 LTS Desktop",
                "magnet": "magnet:?xt=urn:btih:a1b2c3d4e5f6",
                "size": 4700000000,
                "active": true,
                "created_at": "2025-06-01T10:30:00Z",
                "updated_at": "2025-06-01T12:00:00Z",
                "expires_at": "2025-07-01T10:30:00Z",
                "download_state": "downloading",
                "download_speed": 5242880,
                "upload_speed": 1048576,
                "seeds": 150,
                "peers": 42,
                "ratio": 1.5,
                "progress": 0.75,
                "availability": 1.0,
                "eta": 3600,
                "download_finished": false,
                "download_present": false,
                "torrent_file": true,
                "inactive_check": 300,
                "server": 7,
                "files": [
                    {
                        "id": 1001,
                        "md5": "d41d8cd98f00b204e9800998ecf8427e",
                        "mimetype": "application/x-iso9660-image",
                        "name": "ubuntu-24.04-desktop-amd64.iso",
                        "s3_path": "downloads/abc/ubuntu.iso",
                        "short_name": "ubuntu.iso",
                        "size": 4700000000,
                        "hash": "a1b2c3d4e5f6",
                        "zipped": false,
                        "infected": false,
                        "absolute_path": "/mnt/storage/ubuntu.iso",
                        "opensubtitles_hash": null
                    }
                ],
                "download_path": "/downloads/12345",
                "tracker": "udp://tracker.opentrackr.org:1337/announce",
                "total_uploaded": 7050000000,
                "total_downloaded": 4700000000,
                "cached": true,
                "owner": "550e8400-e29b-41d4-a716-446655440000",
                "seed_torrent": true,
                "allow_zipped": false,
                "long_term_seeding": true,
                "tracker_message": "Registered torrent",
                "cached_at": "2025-06-01T11:00:00Z",
                "private": false,
                "alternative_hashes": ["hash1abc", "hash2def"],
                "tags": ["linux", "iso", "ubuntu"]
            }
            """;

        // Act
        Torrent? result = JsonSerializer.Deserialize<Torrent>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(12345L, result.Id);
        Assert.Equal("auth-user-001", result.AuthId);
        Assert.Equal("a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0", result.Hash);
        Assert.Equal("Ubuntu 24.04 LTS Desktop", result.Name);
        Assert.Equal("magnet:?xt=urn:btih:a1b2c3d4e5f6", result.Magnet);
        Assert.Equal(4700000000L, result.Size);
        Assert.True(result.Active);
        Assert.Equal(new DateTimeOffset(2025, 6, 1, 10, 30, 0, TimeSpan.Zero), result.CreatedAt);
        Assert.Equal(new DateTimeOffset(2025, 6, 1, 12, 0, 0, TimeSpan.Zero), result.UpdatedAt);
        Assert.Equal(new DateTimeOffset(2025, 7, 1, 10, 30, 0, TimeSpan.Zero), result.ExpiresAt);
        Assert.Equal("downloading", result.DownloadState);
        Assert.Equal(5242880L, result.DownloadSpeed);
        Assert.Equal(1048576L, result.UploadSpeed);
        Assert.Equal(150, result.Seeds);
        Assert.Equal(42, result.Peers);
        Assert.Equal(1.5, result.Ratio);
        Assert.Equal(0.75, result.Progress);
        Assert.Equal(1.0, result.Availability);
        Assert.Equal(3600L, result.Eta);
        Assert.False(result.DownloadFinished);
        Assert.False(result.DownloadPresent);
        Assert.True(result.TorrentFile);
        Assert.Equal(300L, result.InactiveCheck);
        Assert.Equal(7, result.Server);
        Assert.Single(result.Files);
        Assert.Equal(1001L, result.Files[0].Id);
        Assert.Equal("ubuntu-24.04-desktop-amd64.iso", result.Files[0].Name);
        Assert.Equal(4700000000L, result.Files[0].Size);
        Assert.Equal("/downloads/12345", result.DownloadPath);
        Assert.Equal("udp://tracker.opentrackr.org:1337/announce", result.Tracker);
        Assert.Equal(7050000000L, result.TotalUploaded);
        Assert.Equal(4700000000L, result.TotalDownloaded);
        Assert.True(result.Cached);
        Assert.Equal("550e8400-e29b-41d4-a716-446655440000", result.Owner);
        Assert.True(result.SeedTorrent);
        Assert.False(result.AllowZipped);
        Assert.True(result.LongTermSeeding);
        Assert.Equal("Registered torrent", result.TrackerMessage);
        Assert.Equal(new DateTimeOffset(2025, 6, 1, 11, 0, 0, TimeSpan.Zero), result.CachedAt);
        Assert.False(result.IsPrivate);
        Assert.Equal(2, result.AlternativeHashes.Count);
        Assert.Equal("hash1abc", result.AlternativeHashes[0]);
        Assert.Equal("hash2def", result.AlternativeHashes[1]);
        Assert.Equal(3, result.Tags.Count);
        Assert.Equal("linux", result.Tags[0]);
        Assert.Equal("iso", result.Tags[1]);
        Assert.Equal("ubuntu", result.Tags[2]);
    }

    [Fact]
    public void Deserialize_WithNullOptionals_DefaultsCorrectly()
    {
        // Arrange
        string json = """
            {
                "id": 1,
                "name": "Minimal Torrent",
                "size": 0,
                "active": false,
                "download_speed": 0,
                "upload_speed": 0,
                "seeds": 0,
                "peers": 0,
                "ratio": 0.0,
                "progress": 0.0,
                "availability": 0.0,
                "eta": 0,
                "download_finished": false,
                "download_present": false,
                "torrent_file": false,
                "inactive_check": 0,
                "server": 0,
                "total_uploaded": 0,
                "total_downloaded": 0,
                "cached": false,
                "seed_torrent": false,
                "allow_zipped": false,
                "long_term_seeding": false,
                "private": false
            }
            """;

        // Act
        Torrent? result = JsonSerializer.Deserialize<Torrent>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1L, result.Id);
        Assert.Null(result.AuthId);
        Assert.Null(result.Hash);
        Assert.Equal("Minimal Torrent", result.Name);
        Assert.Null(result.Magnet);
        Assert.Null(result.CreatedAt);
        Assert.Null(result.UpdatedAt);
        Assert.Null(result.ExpiresAt);
        Assert.Null(result.DownloadState);
        Assert.Null(result.DownloadPath);
        Assert.Null(result.Tracker);
        Assert.Null(result.Owner);
        Assert.Null(result.TrackerMessage);
        Assert.Null(result.CachedAt);
        Assert.Empty(result.Files);
        Assert.Empty(result.AlternativeHashes);
        Assert.Empty(result.Tags);
    }

    // ──── TorrentFile ────

    [Fact]
    public void Deserialize_TorrentFile_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "name": "video/movie.mkv",
                "size": 2147483648
            }
            """;

        // Act
        TorrentFile? result = JsonSerializer.Deserialize<TorrentFile>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("video/movie.mkv", result.Name);
        Assert.Equal(2147483648L, result.Size);
    }

    // ──── TorrentInfo ────

    [Fact]
    public void Deserialize_TorrentInfo_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "hash": "abc123def456789012345678901234567890abcd",
                "name": "Big Buck Bunny 1080p",
                "size": 276445467,
                "peers": 35,
                "seeds": 120,
                "files": [
                    {
                        "name": "Big Buck Bunny.mp4",
                        "size": 276445467
                    },
                    {
                        "name": "README.txt",
                        "size": 1024
                    }
                ],
                "trackers": [
                    "udp://tracker.opentrackr.org:1337/announce",
                    "udp://open.stealth.si:80/announce"
                ]
            }
            """;

        // Act
        TorrentInfo? result = JsonSerializer.Deserialize<TorrentInfo>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123def456789012345678901234567890abcd", result.Hash);
        Assert.Equal("Big Buck Bunny 1080p", result.Name);
        Assert.Equal(276445467L, result.Size);
        Assert.Equal(35, result.Peers);
        Assert.Equal(120, result.Seeds);
        Assert.Equal(2, result.Files.Count);
        Assert.Equal("Big Buck Bunny.mp4", result.Files[0].Name);
        Assert.Equal(276445467L, result.Files[0].Size);
        Assert.Equal("README.txt", result.Files[1].Name);
        Assert.Equal(1024L, result.Files[1].Size);
        Assert.Equal(2, result.Trackers.Count);
        Assert.Equal("udp://tracker.opentrackr.org:1337/announce", result.Trackers[0]);
        Assert.Equal("udp://open.stealth.si:80/announce", result.Trackers[1]);
    }

    [Fact]
    public void Deserialize_TorrentInfo_WithNullOptionals_DefaultsCorrectly()
    {
        // Arrange
        string json = """
            {
                "name": "Unknown Torrent",
                "size": 0,
                "peers": 0,
                "seeds": 0
            }
            """;

        // Act
        TorrentInfo? result = JsonSerializer.Deserialize<TorrentInfo>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Hash);
        Assert.Equal("Unknown Torrent", result.Name);
        Assert.Empty(result.Files);
        Assert.Empty(result.Trackers);
    }
}
