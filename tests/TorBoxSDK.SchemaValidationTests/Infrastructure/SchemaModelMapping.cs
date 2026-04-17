using TorBoxSDK.Models.Integrations;
using TorBoxSDK.Models.Queued;
using TorBoxSDK.Models.Rss;
using TorBoxSDK.Models.Torrents;
using TorBoxSDK.Models.Usenet;
using TorBoxSDK.Models.User;
using TorBoxSDK.Models.Vendors;
using TorBoxSDK.Models.WebDownloads;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

/// <summary>
/// Defines the explicit mapping between OpenAPI schema names and their corresponding
/// SDK model types, together with known intentional discrepancies.
/// </summary>
/// <remarks>
/// <para>
/// Only schemas with a direct C# model counterpart are listed here.
/// Framework schemas (<c>HTTPValidationError</c>, <c>ValidationError</c>) and enum schemas
/// (<c>ControlSearchEngineOperation</c>, <c>SearchEngineType</c>, <c>SearchEngineDownloadType</c>)
/// are intentionally excluded because they do not map to standalone model records.
/// </para>
/// <para>
/// When the OpenAPI specification is updated (new API fields added), the coverage tests will
/// fail and list the missing field names so developers know exactly what to add to the SDK.
/// </para>
/// </remarks>
internal static class SchemaModelMapping
{
    /// <summary>
    /// Gets the mapping from OpenAPI schema name to the SDK C# type that represents it.
    /// </summary>
    public static IReadOnlyDictionary<string, Type> SchemaToType { get; } =
        new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            // ── RSS ─────────────────────────────────────────────────────────────────────
            ["AddRss"]    = typeof(AddRssRequest),
            ["ModifyRss"] = typeof(ModifyRssRequest),
            ["ControlRss"] = typeof(ControlRssRequest),

            // ── Integrations ─────────────────────────────────────────────────────────
            // All AddToXxx schemas share CreateIntegrationJobRequest for the common fields.
            // Service-specific auth tokens are listed in KnownOpenApiFieldsNotInSdk.
            ["AddTo1Fichier"]    = typeof(CreateIntegrationJobRequest),
            ["AddToDropbox"]     = typeof(CreateIntegrationJobRequest),
            ["AddToGofile"]      = typeof(CreateIntegrationJobRequest),
            ["AddToGoogleDrive"] = typeof(CreateIntegrationJobRequest),
            ["AddToOnedrive"]    = typeof(CreateIntegrationJobRequest),
            ["AddToPixeldrain"]  = typeof(CreateIntegrationJobRequest),
            ["LinkedRolesData"]     = typeof(LinkedRolesRequest),
            ["OAuthRegisterRequest"] = typeof(OAuthRegisterRequest),

            // ── User ─────────────────────────────────────────────────────────────────
            ["BaseSettingsModel"]   = typeof(UserSettings),
            ["DeleteAccountSchema"] = typeof(DeleteAccountRequest),
            ["DeviceTokenSchema"]   = typeof(DeviceTokenRequest),
            ["RefreshTokenSchema"]  = typeof(RefreshTokenRequest),
            ["SearchEngineEditModel"] = typeof(ModifySearchEnginesRequest),
            ["SearchEngineModel"]     = typeof(AddSearchEnginesRequest),
            ["ControlSearchEngine"]   = typeof(ControlSearchEnginesRequest),

            // ── Torrents ─────────────────────────────────────────────────────────────
            ["Body_create_torrent_v1_api_torrents_createtorrent_post"] =
                typeof(CreateTorrentRequest),
            ["Body_async_create_torrent_v1_api_torrents_asynccreatetorrent_post"] =
                typeof(CreateTorrentRequest),
            ["CheckCached"]  = typeof(CheckCachedRequest),
            ["ControlTorrent"] = typeof(ControlTorrentRequest),
            ["EditTorrent"]    = typeof(EditTorrentRequest),
            ["MagnetToTorrent"] = typeof(MagnetToFileRequest),
            ["Body_get_torrent_info_post_v1_api_torrents_torrentinfo_post"] =
                typeof(TorrentInfoRequest),

            // ── Usenet ───────────────────────────────────────────────────────────────
            ["Body_create_usenet_download_v1_api_usenet_createusenetdownload_post"] =
                typeof(CreateUsenetDownloadRequest),
            ["Body_async_create_usenet_download_v1_api_usenet_asynccreateusenetdownload_post"] =
                typeof(CreateUsenetDownloadRequest),
            ["ControlUsenetDownload"] = typeof(ControlUsenetDownloadRequest),
            ["EditUsenetDownload"]    = typeof(EditUsenetDownloadRequest),

            // ── Web Downloads ─────────────────────────────────────────────────────────
            ["Body_create_web_download_v1_api_webdl_createwebdownload_post"] =
                typeof(CreateWebDownloadRequest),
            ["Body_async_create_web_download_v1_api_webdl_asynccreatewebdownload_post"] =
                typeof(CreateWebDownloadRequest),
            ["ControlWebDownload"] = typeof(ControlWebDownloadRequest),
            ["EditWebDownload"]    = typeof(EditWebDownloadRequest),

            // ── Queued ───────────────────────────────────────────────────────────────
            ["ControlQueuedDownload"] = typeof(ControlQueuedRequest),

            // ── Vendors ──────────────────────────────────────────────────────────────
            ["Body_register_new_user_v1_api_vendors_registeruser_post"] =
                typeof(RegisterVendorUserRequest),
            ["Body_register_vendor_v1_api_vendors_register_post"] =
                typeof(RegisterVendorRequest),
            ["Body_update_vendor_account_v1_api_vendors_updateaccount_put"] =
                typeof(UpdateVendorAccountRequest),
        };

    /// <summary>
    /// Gets the set of OpenAPI field names that are intentionally not mapped in the SDK
    /// for each schema. The coverage tests will not flag these as "missing in SDK".
    /// </summary>
    /// <remarks>
    /// Typical reasons for intentional exclusion:
    /// <list type="bullet">
    ///   <item>
    ///     Service-specific auth tokens (e.g., <c>onefichier_token</c>) that are not
    ///     part of the shared <see cref="CreateIntegrationJobRequest"/> model.
    ///   </item>
    ///   <item>
    ///     File upload fields decorated with <c>[JsonIgnore]</c> because they are sent
    ///     as multipart form data rather than as JSON body properties.
    ///   </item>
    /// </list>
    /// </remarks>
    public static IReadOnlyDictionary<string, IReadOnlySet<string>> KnownOpenApiFieldsNotInSdk { get; } =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            // Integration service-specific auth tokens are not part of the shared request model.
            ["AddTo1Fichier"]    = Set("onefichier_token"),
            ["AddToDropbox"]     = Set("dropbox_token"),
            ["AddToGofile"]      = Set("gofile_token"),
            ["AddToGoogleDrive"] = Set("google_token"),
            ["AddToOnedrive"]    = Set("onedrive_token"),
            ["AddToPixeldrain"]  = Set("pixeldrain_token"),

            // File upload fields are decorated with [JsonIgnore] and sent as multipart form data.
            ["Body_create_torrent_v1_api_torrents_createtorrent_post"] = Set("file"),
            ["Body_async_create_torrent_v1_api_torrents_asynccreatetorrent_post"] = Set("file"),
            ["Body_create_usenet_download_v1_api_usenet_createusenetdownload_post"] = Set("file"),
            ["Body_async_create_usenet_download_v1_api_usenet_asynccreateusenetdownload_post"] = Set("file"),
            ["Body_get_torrent_info_post_v1_api_torrents_torrentinfo_post"] = Set("file"),
        };

    /// <summary>
    /// Gets the set of SDK JSON property names that are intentionally not in the OpenAPI
    /// schema for each schema. The coverage tests will not flag these as "extra in SDK".
    /// </summary>
    /// <remarks>
    /// These fields exist in the SDK because the real API accepts them even though they
    /// are not yet documented in the OpenAPI specification.
    /// </remarks>
    public static IReadOnlyDictionary<string, IReadOnlySet<string>> KnownSdkFieldsNotInOpenApi { get; } =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            // The API accepts format and list_files on the cache check endpoints
            // even though the OpenAPI CheckCached schema only documents hashes.
            ["CheckCached"] = Set("format", "list_files"),

            // The API accepts an `all` flag to apply the operation to every RSS feed,
            // but the OpenAPI ControlRss schema only documents operation and rss_feed_id.
            ["ControlRss"] = Set("all"),
        };

    /// <summary>
    /// Returns the equivalent OpenAPI type string for a .NET property type,
    /// used for basic type-compatibility checks in <c>OpenApiTypeMappingTests</c>.
    /// </summary>
    public static string MapDotNetTypeToOpenApiType(Type dotNetType)
    {
        Type underlying = Nullable.GetUnderlyingType(dotNetType) ?? dotNetType;

        if (underlying == typeof(string)) return "string";
        if (underlying == typeof(bool)) return "boolean";
        if (underlying == typeof(int) || underlying == typeof(long) ||
            underlying == typeof(short) || underlying == typeof(uint) ||
            underlying == typeof(ulong)) return "integer";
        if (underlying == typeof(double) || underlying == typeof(float) ||
            underlying == typeof(decimal)) return "number";
        if (underlying == typeof(DateTimeOffset) || underlying == typeof(DateTime))
            return "string";  // ISO-8601 strings in JSON
        if (underlying.IsEnum) return IsJsonStringEnum(underlying) ? "string" : "integer";
        if (IsCollectionType(underlying)) return "array";

        return "object";
    }

    private static bool IsCollectionType(Type type)
    {
        if (!type.IsGenericType) return false;
        Type def = type.GetGenericTypeDefinition();
        return def == typeof(IReadOnlyList<>)
            || def == typeof(List<>)
            || def == typeof(IEnumerable<>)
            || def == typeof(IReadOnlyCollection<>)
            || def == typeof(ICollection<>);
    }

    /// <summary>
    /// Returns <see langword="true"/> when the enum type is decorated with
    /// <c>[JsonConverter(typeof(JsonStringEnumConverter))]</c> and therefore serialises as a string.
    /// Returns <see langword="false"/> for enums that serialise as their underlying integer value.
    /// </summary>
    private static bool IsJsonStringEnum(Type enumType)
    {
        foreach (System.Reflection.CustomAttributeData attr in enumType.CustomAttributes)
        {
            if (attr.AttributeType != typeof(System.Text.Json.Serialization.JsonConverterAttribute))
                continue;

            if (attr.ConstructorArguments.Count > 0 &&
                attr.ConstructorArguments[0].Value is Type converterType &&
                converterType == typeof(System.Text.Json.Serialization.JsonStringEnumConverter))
            {
                return true;
            }
        }

        return false;
    }

    private static IReadOnlySet<string> Set(params string[] values) =>
        new HashSet<string>(values, StringComparer.Ordinal);
}
