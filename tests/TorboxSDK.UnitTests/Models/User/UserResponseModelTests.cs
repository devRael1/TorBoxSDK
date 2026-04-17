using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.User;

namespace TorboxSDK.UnitTests.Models.User;

public sealed class UserResponseModelTests
{
    // ──── DashboardFilter ────

    [Fact]
    public void DashboardFilter_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "active": true,
                "cached": false,
                "queued": true,
                "private": false,
                "borrowed": true
            }
            """;

        // Act
        DashboardFilter? result = JsonSerializer.Deserialize<DashboardFilter>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Active);
        Assert.False(result.Cached);
        Assert.True(result.Queued);
        Assert.False(result.Private);
        Assert.True(result.Borrowed);
    }

    [Fact]
    public void DashboardFilter_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {}
            """;

        // Act
        DashboardFilter? result = JsonSerializer.Deserialize<DashboardFilter>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Active);
        Assert.Null(result.Cached);
        Assert.Null(result.Queued);
        Assert.Null(result.Private);
        Assert.Null(result.Borrowed);
    }

    // ──── UserSettings ────

    [Fact]
    public void UserSettings_Deserialize_PopulatesRepresentativeSubset()
    {
        // Arrange
        string json = """
            {
                "email_notifications": true,
                "web_notifications": false,
                "mobile_notifications": true,
                "rss_notifications": false,
                "discord_notifications": true,
                "jdownloader_notifications": false,
                "webhook_notifications": true,
                "telegram_notifications": false,
                "webhook_url": "https://hooks.example.com/torbox",
                "telegram_id": "123456789",
                "discord_id": "987654321012345678",
                "stremio_quality": [1, 2, 3],
                "stremio_resolution": [1080, 720],
                "stremio_language": [1, 2],
                "stremio_cache": [1],
                "stremio_size_lower": 104857600,
                "stremio_size_upper": 10737418240,
                "stremio_allow_adult": false,
                "stremio_seed_torrents": 2,
                "stremio_sort": "quality",
                "stremio_use_custom_search_engines": true,
                "stremio_result_sort": "seeders",
                "stremio_legacy_your_media": false,
                "stremio_only_your_media_streams": true,
                "stremio_disable_your_media_streams": false,
                "stremio_limit_per_resolution_torrent": 5,
                "stremio_limit_per_resolution_usenet": 3,
                "stremio_torrent_seeders_cutoff": 10,
                "stremio_wait_for_download_usenet": true,
                "stremio_wait_for_download_torrent": false,
                "stremio_disable_filtered_note": true,
                "stremio_emoji_in_description": false,
                "stremio_allow_zipped": true,
                "seed_torrents": 1,
                "allow_zipped": true,
                "cdn_selection": "auto",
                "google_drive_folder_id": "1AbCdEfGhIjKlMnOpQrStUvWxYz",
                "onedrive_save_path": "/TorBox/Downloads",
                "onefichier_folder_id": "folder123",
                "gofile_folder_id": "gofile456",
                "pixeldrain_api_key": "pd-key-abc",
                "onefichier_api_key": "of-key-def",
                "gofile_api_key": "gf-key-ghi",
                "mega_email": "user@mega.nz",
                "mega_password": "securepassword",
                "patreon_id": "patreon-12345",
                "download_speed_in_tab": true,
                "show_tracker_in_torrents": false,
                "dashboard_filter": {
                    "active": true,
                    "cached": true,
                    "queued": false,
                    "private": false,
                    "borrowed": true
                },
                "dashboard_sort": "created_at",
                "append_filename_to_links": true,
                "web_player_always_transcode": false,
                "web_player_always_skip_intro": true,
                "web_player_audio_preferred_language": "en",
                "web_player_subtitle_preferred_language": "es",
                "web_player_disable_prestream_selector": false,
                "web_player_disable_next_up_dialogue": true,
                "web_player_enable_scrobbling": true,
                "webdav_use_local_files": false,
                "webdav_use_folder_view": true,
                "webdav_flatten": false
            }
            """;

        // Act
        UserSettings? result = JsonSerializer.Deserialize<UserSettings>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);

        // Notifications
        Assert.True(result.EmailNotifications);
        Assert.False(result.WebNotifications);
        Assert.True(result.MobileNotifications);
        Assert.False(result.RssNotifications);
        Assert.True(result.DiscordNotifications);
        Assert.False(result.JdownloaderNotifications);
        Assert.True(result.WebhookNotifications);
        Assert.False(result.TelegramNotifications);

        // Webhook / Telegram / Discord
        Assert.Equal("https://hooks.example.com/torbox", result.WebhookUrl);
        Assert.Equal("123456789", result.TelegramId);
        Assert.Equal("987654321012345678", result.DiscordId);

        // Stremio
        Assert.NotNull(result.StremioQuality);
        Assert.Equal(3, result.StremioQuality.Count);
        Assert.Equal(1, result.StremioQuality[0]);
        Assert.NotNull(result.StremioResolution);
        Assert.Equal(2, result.StremioResolution.Count);
        Assert.Equal(1080, result.StremioResolution[0]);
        Assert.NotNull(result.StremioLanguage);
        Assert.Equal(2, result.StremioLanguage.Count);
        Assert.NotNull(result.StremioCache);
        Assert.Single(result.StremioCache);
        Assert.Equal(104857600L, result.StremioSizeLower);
        Assert.Equal(10737418240L, result.StremioSizeUpper);
        Assert.False(result.StremioAllowAdult);
        Assert.Equal(2, result.StremioSeedTorrents);
        Assert.Equal("quality", result.StremioSort);
        Assert.True(result.StremioUseCustomSearchEngines);
        Assert.Equal("seeders", result.StremioResultSort);
        Assert.False(result.StremioLegacyYourMedia);
        Assert.True(result.StremioOnlyYourMediaStreams);
        Assert.False(result.StremioDisableYourMediaStreams);
        Assert.Equal(5, result.StremioLimitPerResolutionTorrent);
        Assert.Equal(3, result.StremioLimitPerResolutionUsenet);
        Assert.Equal(10, result.StremioTorrentSeedersCutoff);
        Assert.True(result.StremioWaitForDownloadUsenet);
        Assert.False(result.StremioWaitForDownloadTorrent);
        Assert.True(result.StremioDisableFilteredNote);
        Assert.False(result.StremioEmojiInDescription);
        Assert.True(result.StremioAllowZipped);

        // Downloads
        Assert.Equal(1, result.SeedTorrents);
        Assert.True(result.AllowZipped);
        Assert.Equal("auto", result.CdnSelection);

        // Cloud Storage
        Assert.Equal("1AbCdEfGhIjKlMnOpQrStUvWxYz", result.GoogleDriveFolderId);
        Assert.Equal("/TorBox/Downloads", result.OnedriveSavePath);
        Assert.Equal("folder123", result.OnefichierFolderId);
        Assert.Equal("gofile456", result.GofileFolderId);
        Assert.Equal("pd-key-abc", result.PixeldrainApiKey);
        Assert.Equal("of-key-def", result.OnefichierApiKey);
        Assert.Equal("gf-key-ghi", result.GofileApiKey);
        Assert.Equal("user@mega.nz", result.MegaEmail);
        Assert.Equal("securepassword", result.MegaPassword);
        Assert.Equal("patreon-12345", result.PatreonId);

        // UI
        Assert.True(result.DownloadSpeedInTab);
        Assert.False(result.ShowTrackerInTorrents);
        Assert.NotNull(result.DashboardFilter);
        Assert.True(result.DashboardFilter.Active);
        Assert.True(result.DashboardFilter.Cached);
        Assert.False(result.DashboardFilter.Queued);
        Assert.False(result.DashboardFilter.Private);
        Assert.True(result.DashboardFilter.Borrowed);
        Assert.Equal("created_at", result.DashboardSort);
        Assert.True(result.AppendFilenameToLinks);

        // Web Player
        Assert.False(result.WebPlayerAlwaysTranscode);
        Assert.True(result.WebPlayerAlwaysSkipIntro);
        Assert.Equal("en", result.WebPlayerAudioPreferredLanguage);
        Assert.Equal("es", result.WebPlayerSubtitlePreferredLanguage);
        Assert.False(result.WebPlayerDisablePrestreamSelector);
        Assert.True(result.WebPlayerDisableNextUpDialogue);
        Assert.True(result.WebPlayerEnableScrobbling);

        // WebDAV
        Assert.False(result.WebdavUseLocalFiles);
        Assert.True(result.WebdavUseFolderView);
        Assert.False(result.WebdavFlatten);
    }

    [Fact]
    public void UserSettings_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {}
            """;

        // Act
        UserSettings? result = JsonSerializer.Deserialize<UserSettings>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.EmailNotifications);
        Assert.Null(result.WebNotifications);
        Assert.Null(result.WebhookUrl);
        Assert.Null(result.SeedTorrents);
        Assert.Null(result.DashboardFilter);
        Assert.Null(result.DashboardSort);
        Assert.Null(result.AppendFilenameToLinks);
        Assert.Null(result.WebPlayerAlwaysTranscode);
        Assert.Null(result.StremioQuality);
        Assert.Null(result.StremioResolution);
        Assert.Null(result.CdnSelection);
        Assert.Null(result.GoogleDriveFolderId);
        Assert.Null(result.WebdavFlatten);
    }

    // ──── UserProfile ────

    [Fact]
    public void UserProfile_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": 42,
                "auth_id": "auth|abc123def456",
                "created_at": "2024-01-15T09:30:00Z",
                "updated_at": "2025-06-01T18:45:00Z",
                "plan": 3,
                "total_downloaded": 107374182400,
                "customer": "cus_abc123xyz",
                "is_subscribed": true,
                "premium_expires_at": "2026-01-15T09:30:00Z",
                "cooldown_until": null,
                "email": "user@example.com",
                "user_referral": "REF-XYZ-789",
                "base_email": "user@example.com",
                "total_bytes_downloaded": 214748364800,
                "total_bytes_uploaded": 53687091200,
                "torrents_downloaded": 1500,
                "web_downloads_downloaded": 250,
                "usenet_downloads_downloaded": 100,
                "additional_concurrent_slots": 2,
                "long_term_seeding": true,
                "long_term_storage": false,
                "is_vendor": false,
                "vendor_id": null,
                "purchases_referred": 5,
                "settings": {
                    "email_notifications": true,
                    "seed_torrents": 1,
                    "dashboard_sort": "name"
                }
            }
            """;

        // Act
        UserProfile? result = JsonSerializer.Deserialize<UserProfile>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42L, result.Id);
        Assert.Equal("auth|abc123def456", result.AuthId);
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 9, 30, 0, TimeSpan.Zero), result.CreatedAt);
        Assert.Equal(new DateTimeOffset(2025, 6, 1, 18, 45, 0, TimeSpan.Zero), result.UpdatedAt);
        Assert.Equal(3, result.Plan);
        Assert.Equal(107374182400L, result.TotalDownloaded);
        Assert.Equal("cus_abc123xyz", result.Customer);
        Assert.True(result.IsSubscribed);
        Assert.Equal(new DateTimeOffset(2026, 1, 15, 9, 30, 0, TimeSpan.Zero), result.PremiumExpiresAt);
        Assert.Null(result.CooldownUntil);
        Assert.Equal("user@example.com", result.Email);
        Assert.Equal("REF-XYZ-789", result.UserReferral);
        Assert.Equal("user@example.com", result.BaseEmail);
        Assert.Equal(214748364800L, result.TotalBytesDownloaded);
        Assert.Equal(53687091200L, result.TotalBytesUploaded);
        Assert.Equal(1500L, result.TorrentsDownloaded);
        Assert.Equal(250L, result.WebDownloadsDownloaded);
        Assert.Equal(100L, result.UsenetDownloadsDownloaded);
        Assert.Equal(2, result.AdditionalConcurrentSlots);
        Assert.True(result.LongTermSeeding);
        Assert.False(result.LongTermStorage);
        Assert.False(result.IsVendor);
        Assert.Null(result.VendorId);
        Assert.Equal(5, result.PurchasesReferred);
        Assert.NotNull(result.Settings);
        Assert.True(result.Settings.EmailNotifications);
        Assert.Equal(1, result.Settings.SeedTorrents);
        Assert.Equal("name", result.Settings.DashboardSort);
    }

    [Fact]
    public void UserProfile_Deserialize_WithNullOptionals_DefaultsCorrectly()
    {
        // Arrange
        string json = """
            {
                "id": 1,
                "plan": 0,
                "total_downloaded": 0,
                "is_subscribed": false,
                "total_bytes_downloaded": 0,
                "total_bytes_uploaded": 0,
                "torrents_downloaded": 0,
                "web_downloads_downloaded": 0,
                "usenet_downloads_downloaded": 0,
                "additional_concurrent_slots": 0,
                "long_term_seeding": false,
                "long_term_storage": false,
                "is_vendor": false,
                "purchases_referred": 0
            }
            """;

        // Act
        UserProfile? result = JsonSerializer.Deserialize<UserProfile>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1L, result.Id);
        Assert.Null(result.AuthId);
        Assert.Null(result.CreatedAt);
        Assert.Null(result.UpdatedAt);
        Assert.Null(result.Customer);
        Assert.Null(result.PremiumExpiresAt);
        Assert.Null(result.CooldownUntil);
        Assert.Null(result.Email);
        Assert.Null(result.UserReferral);
        Assert.Null(result.BaseEmail);
        Assert.Null(result.VendorId);
        Assert.Null(result.Settings);
    }

    // ──── Subscription ────

    [Fact]
    public void Subscription_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": 10001,
                "plan_name": "Pro Annual",
                "amount": 59.99,
                "currency": "USD",
                "status": "active",
                "created_at": "2025-01-01T00:00:00Z",
                "expires_at": "2026-01-01T00:00:00Z"
            }
            """;

        // Act
        Subscription? result = JsonSerializer.Deserialize<Subscription>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10001L, result.Id);
        Assert.Equal("Pro Annual", result.PlanName);
        Assert.Equal(59.99, result.Amount);
        Assert.Equal("USD", result.Currency);
        Assert.Equal("active", result.Status);
        Assert.Equal(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), result.CreatedAt);
        Assert.Equal(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), result.ExpiresAt);
    }

    [Fact]
    public void Subscription_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {
                "id": 1,
                "amount": 0.0
            }
            """;

        // Act
        Subscription? result = JsonSerializer.Deserialize<Subscription>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1L, result.Id);
        Assert.Equal(0.0, result.Amount);
        Assert.Null(result.PlanName);
        Assert.Null(result.Currency);
        Assert.Null(result.Status);
        Assert.Null(result.CreatedAt);
        Assert.Null(result.ExpiresAt);
    }

    // ──── Transaction ────

    [Fact]
    public void Transaction_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "at": "2025-03-15T14:30:00Z",
                "type": "sellix",
                "amount": 9.99,
                "transaction_id": "TXN-abc123def456"
            }
            """;

        // Act
        Transaction? result = JsonSerializer.Deserialize<Transaction>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new DateTimeOffset(2025, 3, 15, 14, 30, 0, TimeSpan.Zero), result.At);
        Assert.Equal("sellix", result.Type);
        Assert.Equal(9.99, result.Amount);
        Assert.Equal("TXN-abc123def456", result.TransactionId);
    }

    [Fact]
    public void Transaction_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {
                "amount": 0.0
            }
            """;

        // Act
        Transaction? result = JsonSerializer.Deserialize<Transaction>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.0, result.Amount);
        Assert.Null(result.At);
        Assert.Null(result.Type);
        Assert.Null(result.TransactionId);
    }

    // ──── ReferralData ────

    [Fact]
    public void ReferralData_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "referred_accounts": 15,
                "referral_code": "TORBOX-REF-ABC123",
                "purchases_referred": 7
            }
            """;

        // Act
        ReferralData? result = JsonSerializer.Deserialize<ReferralData>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.ReferredAccounts);
        Assert.Equal("TORBOX-REF-ABC123", result.ReferralCode);
        Assert.Equal(7, result.PurchasesReferred);
    }

    [Fact]
    public void ReferralData_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {
                "referred_accounts": 0,
                "purchases_referred": 0
            }
            """;

        // Act
        ReferralData? result = JsonSerializer.Deserialize<ReferralData>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.ReferredAccounts);
        Assert.Null(result.ReferralCode);
        Assert.Equal(0, result.PurchasesReferred);
    }

    // ──── DeviceCodeResponse ────

    [Fact]
    public void DeviceCodeResponse_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "device_code": "dev_abc123def456ghi789",
                "code": "ABCD-1234",
                "verification_url": "https://torbox.app/device",
                "friendly_verification_url": "https://torbox.app/d",
                "expires_at": "2025-06-15T10:15:00Z",
                "interval": 5
            }
            """;

        // Act
        DeviceCodeResponse? result = JsonSerializer.Deserialize<DeviceCodeResponse>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("dev_abc123def456ghi789", result.DeviceCode);
        Assert.Equal("ABCD-1234", result.Code);
        Assert.Equal("https://torbox.app/device", result.VerificationUrl);
        Assert.Equal("https://torbox.app/d", result.FriendlyVerificationUrl);
        Assert.Equal(new DateTimeOffset(2025, 6, 15, 10, 15, 0, TimeSpan.Zero), result.ExpiresAt);
        Assert.Equal(5, result.Interval);
    }

    [Fact]
    public void DeviceCodeResponse_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {
                "interval": 10
            }
            """;

        // Act
        DeviceCodeResponse? result = JsonSerializer.Deserialize<DeviceCodeResponse>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.DeviceCode);
        Assert.Null(result.Code);
        Assert.Null(result.VerificationUrl);
        Assert.Null(result.FriendlyVerificationUrl);
        Assert.Null(result.ExpiresAt);
        Assert.Equal(10, result.Interval);
    }
}
