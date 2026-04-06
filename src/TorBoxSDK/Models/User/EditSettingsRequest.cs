using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to edit the user's account settings.
/// </summary>
public sealed record EditSettingsRequest
{
    // ──── Notifications ────

    /// <summary>
    /// Gets a value indicating whether email notifications are enabled,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("email_notifications")]
    public bool? EmailNotifications { get; init; }

    /// <summary>
    /// Gets a value indicating whether web notifications are enabled,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("web_notifications")]
    public bool? WebNotifications { get; init; }

    /// <summary>
    /// Gets a value indicating whether mobile notifications are enabled,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("mobile_notifications")]
    public bool? MobileNotifications { get; init; }

    /// <summary>
    /// Gets a value indicating whether RSS notifications are enabled,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("rss_notifications")]
    public bool? RssNotifications { get; init; }

    /// <summary>
    /// Gets a value indicating whether Discord notifications are enabled,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("discord_notifications")]
    public bool? DiscordNotifications { get; init; }

    /// <summary>
    /// Gets a value indicating whether JDownloader notifications are enabled,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("jdownloader_notifications")]
    public bool? JdownloaderNotifications { get; init; }

    /// <summary>
    /// Gets a value indicating whether webhook notifications are enabled,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("webhook_notifications")]
    public bool? WebhookNotifications { get; init; }

    /// <summary>
    /// Gets a value indicating whether Telegram notifications are enabled,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("telegram_notifications")]
    public bool? TelegramNotifications { get; init; }

    // ──── Webhook / Telegram / Discord ────

    /// <summary>
    /// Gets the webhook URL for notifications,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("webhook_url")]
    public string? WebhookUrl { get; init; }

    /// <summary>
    /// Gets the Telegram ID for notifications,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("telegram_id")]
    public string? TelegramId { get; init; }

    /// <summary>
    /// Gets the Discord ID for notifications,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("discord_id")]
    public string? DiscordId { get; init; }

    // ──── Stremio ────

    /// <summary>
    /// Gets the Stremio quality preferences,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_quality")]
    public int[]? StremioQuality { get; init; }

    /// <summary>
    /// Gets the Stremio resolution preferences,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_resolution")]
    public int[]? StremioResolution { get; init; }

    /// <summary>
    /// Gets the Stremio language preferences,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_language")]
    public int[]? StremioLanguage { get; init; }

    /// <summary>
    /// Gets the Stremio cache settings,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_cache")]
    public int[]? StremioCache { get; init; }

    /// <summary>
    /// Gets the Stremio minimum size in bytes,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_size_lower")]
    public long? StremioSizeLower { get; init; }

    /// <summary>
    /// Gets the Stremio maximum size in bytes,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_size_upper")]
    public long? StremioSizeUpper { get; init; }

    /// <summary>
    /// Gets a value indicating whether adult content is allowed in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_allow_adult")]
    public bool? StremioAllowAdult { get; init; }

    /// <summary>
    /// Gets the Stremio torrent seeding setting,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_seed_torrents")]
    public int? StremioSeedTorrents { get; init; }

    /// <summary>
    /// Gets the Stremio sort preference,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_sort")]
    public string? StremioSort { get; init; }

    /// <summary>
    /// Gets a value indicating whether custom search engines are used in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_use_custom_search_engines")]
    public bool? StremioUseCustomSearchEngines { get; init; }

    /// <summary>
    /// Gets the Stremio result sort preference,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_result_sort")]
    public string? StremioResultSort { get; init; }

    /// <summary>
    /// Gets a value indicating whether legacy your media is enabled in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_legacy_your_media")]
    public bool? StremioLegacyYourMedia { get; init; }

    /// <summary>
    /// Gets a value indicating whether only your media streams are shown in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_only_your_media_streams")]
    public bool? StremioOnlyYourMediaStreams { get; init; }

    /// <summary>
    /// Gets a value indicating whether your media streams are disabled in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_disable_your_media_streams")]
    public bool? StremioDisableYourMediaStreams { get; init; }

    /// <summary>
    /// Gets the limit per resolution for torrents in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_limit_per_resolution_torrent")]
    public int? StremioLimitPerResolutionTorrent { get; init; }

    /// <summary>
    /// Gets the limit per resolution for usenet in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_limit_per_resolution_usenet")]
    public int? StremioLimitPerResolutionUsenet { get; init; }

    /// <summary>
    /// Gets the torrent seeders cutoff in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_torrent_seeders_cutoff")]
    public int? StremioTorrentSeedersCutoff { get; init; }

    /// <summary>
    /// Gets a value indicating whether to wait for usenet download in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_wait_for_download_usenet")]
    public bool? StremioWaitForDownloadUsenet { get; init; }

    /// <summary>
    /// Gets a value indicating whether to wait for torrent download in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_wait_for_download_torrent")]
    public bool? StremioWaitForDownloadTorrent { get; init; }

    /// <summary>
    /// Gets a value indicating whether the filtered note is disabled in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_disable_filtered_note")]
    public bool? StremioDisableFilteredNote { get; init; }

    /// <summary>
    /// Gets a value indicating whether emoji is shown in description in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_emoji_in_description")]
    public bool? StremioEmojiInDescription { get; init; }

    // ──── Downloads ────

    /// <summary>
    /// Gets the torrent seeding preference,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("seed_torrents")]
    public int? SeedTorrents { get; init; }

    /// <summary>
    /// Gets a value indicating whether zipped downloads are allowed,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("allow_zipped")]
    public bool? AllowZipped { get; init; }

    /// <summary>
    /// Gets a value indicating whether zipped downloads are allowed in Stremio,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("stremio_allow_zipped")]
    public bool? StremioAllowZipped { get; init; }

    /// <summary>
    /// Gets the CDN selection preference,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("cdn_selection")]
    public string? CdnSelection { get; init; }

    /// <summary>
    /// Gets a value indicating whether filenames are appended to download links,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("append_filename_to_links")]
    public bool? AppendFilenameToLinks { get; init; }

    // ──── Cloud Storage ────

    /// <summary>
    /// Gets the Google Drive folder ID,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("google_drive_folder_id")]
    public string? GoogleDriveFolderId { get; init; }

    /// <summary>
    /// Gets the OneDrive save path,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("onedrive_save_path")]
    public string? OnedriveSavePath { get; init; }

    /// <summary>
    /// Gets the 1Fichier folder ID,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("onefichier_folder_id")]
    public string? OnefichierFolderId { get; init; }

    /// <summary>
    /// Gets the GoFile folder ID,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("gofile_folder_id")]
    public string? GofileFolderId { get; init; }

    /// <summary>
    /// Gets the Pixeldrain API key,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("pixeldrain_api_key")]
    public string? PixeldrainApiKey { get; init; }

    /// <summary>
    /// Gets the 1Fichier API key,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("onefichier_api_key")]
    public string? OnefichierApiKey { get; init; }

    /// <summary>
    /// Gets the GoFile API key,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("gofile_api_key")]
    public string? GofileApiKey { get; init; }

    /// <summary>
    /// Gets the MEGA email,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("mega_email")]
    public string? MegaEmail { get; init; }

    /// <summary>
    /// Gets the MEGA password,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("mega_password")]
    public string? MegaPassword { get; init; }

    // ──── UI ────

    /// <summary>
    /// Gets a value indicating whether download speed is shown in the browser tab,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("download_speed_in_tab")]
    public bool? DownloadSpeedInTab { get; init; }

    /// <summary>
    /// Gets a value indicating whether tracker info is shown in torrents,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("show_tracker_in_torrents")]
    public bool? ShowTrackerInTorrents { get; init; }

    /// <summary>
    /// Gets the dashboard sort preference,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("dashboard_sort")]
    public string? DashboardSort { get; init; }

    /// <summary>
    /// Gets the dashboard filter settings,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("dashboard_filter")]
    public DashboardFilter? DashboardFilter { get; init; }

    /// <summary>
    /// Gets the Patreon ID,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("patreon_id")]
    public string? PatreonId { get; init; }

    // ──── Web Player ────

    /// <summary>
    /// Gets a value indicating whether the web player always transcodes,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("web_player_always_transcode")]
    public bool? WebPlayerAlwaysTranscode { get; init; }

    /// <summary>
    /// Gets a value indicating whether the web player always skips intros,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("web_player_always_skip_intro")]
    public bool? WebPlayerAlwaysSkipIntro { get; init; }

    /// <summary>
    /// Gets the preferred audio language in the web player,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("web_player_audio_preferred_language")]
    public string? WebPlayerAudioPreferredLanguage { get; init; }

    /// <summary>
    /// Gets the preferred subtitle language in the web player,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("web_player_subtitle_preferred_language")]
    public string? WebPlayerSubtitlePreferredLanguage { get; init; }

    /// <summary>
    /// Gets a value indicating whether the prestream selector is disabled in the web player,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("web_player_disable_prestream_selector")]
    public bool? WebPlayerDisablePrestreamSelector { get; init; }

    /// <summary>
    /// Gets a value indicating whether the next up dialogue is disabled in the web player,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("web_player_disable_next_up_dialogue")]
    public bool? WebPlayerDisableNextUpDialogue { get; init; }

    /// <summary>
    /// Gets a value indicating whether scrobbling is enabled in the web player,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("web_player_enable_scrobbling")]
    public bool? WebPlayerEnableScrobbling { get; init; }

    // ──── WebDAV ────

    /// <summary>
    /// Gets a value indicating whether local files are used for WebDAV,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("webdav_use_local_files")]
    public bool? WebdavUseLocalFiles { get; init; }

    /// <summary>
    /// Gets a value indicating whether folder view is used for WebDAV,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("webdav_use_folder_view")]
    public bool? WebdavUseFolderView { get; init; }

    /// <summary>
    /// Gets a value indicating whether the WebDAV structure is flattened,
    /// or <see langword="null"/> to leave the setting unchanged.
    /// </summary>
    [JsonPropertyName("webdav_flatten")]
    public bool? WebdavFlatten { get; init; }
}
