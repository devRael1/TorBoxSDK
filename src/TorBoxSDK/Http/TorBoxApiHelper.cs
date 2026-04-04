using System.Text.Json;

using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Http;

/// <summary>
/// Internal helper for processing TorBox API HTTP responses and building requests.
/// </summary>
internal static class TorBoxApiHelper
{
    /// <summary>Sends a request and deserializes the TorBox response envelope.</summary>
    internal static async Task<TorBoxResponse<T>> SendAsync<T>(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.SendAsync(request, ct).ConfigureAwait(false);

        string json = await response.Content.ReadAsStringAsync(
#if NET7_0_OR_GREATER
            ct
#endif
        ).ConfigureAwait(false);

        TorBoxResponse<T>? result = JsonSerializer.Deserialize<TorBoxResponse<T>>(json, TorBoxJsonOptions.Default);

        if (result is null)
        {
            throw new TorBoxException(
                "Failed to deserialize API response.",
                TorBoxErrorCode.UnknownError,
                json);
        }

        if (!result.Success)
        {
            TorBoxErrorCode errorCode = ParseErrorCode(result.Error);
            throw new TorBoxException(
                result.Detail ?? result.Error ?? "Unknown API error.",
                errorCode,
                result.Detail);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new TorBoxException(
                $"HTTP {(int)response.StatusCode}: {result.Detail ?? response.ReasonPhrase}",
                TorBoxErrorCode.ServerError,
                result.Detail);
        }

        return result;
    }

    /// <summary>Sends a request and deserializes the non-generic TorBox response envelope.</summary>
    internal static async Task<TorBoxResponse> SendAsync(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.SendAsync(request, ct).ConfigureAwait(false);

        string json = await response.Content.ReadAsStringAsync(
#if NET7_0_OR_GREATER
            ct
#endif
        ).ConfigureAwait(false);

        TorBoxResponse? result = JsonSerializer.Deserialize<TorBoxResponse>(json, TorBoxJsonOptions.Default);

        if (result is null)
        {
            throw new TorBoxException(
                "Failed to deserialize API response.",
                TorBoxErrorCode.UnknownError,
                json);
        }

        if (!result.Success)
        {
            TorBoxErrorCode errorCode = ParseErrorCode(result.Error);
            throw new TorBoxException(
                result.Detail ?? result.Error ?? "Unknown API error.",
                errorCode,
                result.Detail);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new TorBoxException(
                $"HTTP {(int)response.StatusCode}: {result.Detail ?? response.ReasonPhrase}",
                TorBoxErrorCode.ServerError,
                result.Detail);
        }

        return result;
    }

    /// <summary>Builds a query string from parameters, skipping null values.</summary>
    internal static string BuildQuery(params (string key, string? value)[] parameters)
    {
        var pairs = new List<string>();
        foreach ((string key, string? value) in parameters)
        {
            if (value is not null)
            {
                pairs.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
            }
        }

        return pairs.Count > 0 ? "?" + string.Join("&", pairs) : string.Empty;
    }

    /// <summary>Serializes a request body to JSON content.</summary>
    internal static StringContent JsonContent<T>(T body)
    {
        string json = JsonSerializer.Serialize(body, TorBoxJsonOptions.Default);
        return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    }

    private static TorBoxErrorCode ParseErrorCode(string? error)
    {
        if (string.IsNullOrEmpty(error))
        {
            return TorBoxErrorCode.Unknown;
        }

        return error.ToUpperInvariant().Replace(" ", "").Replace("_", "") switch
        {
            "DATABASE_ERROR" or "DATABASEERROR" => TorBoxErrorCode.DatabaseError,
            "BAD_TOKEN" or "BADTOKEN" => TorBoxErrorCode.BadToken,
            "NO_AUTH" or "NOAUTH" => TorBoxErrorCode.NoAuth,
            "INVALID_OPTION" or "INVALIDOPTION" => TorBoxErrorCode.InvalidOption,
            "PERMISSION_DENIED" or "PERMISSIONDENIED" => TorBoxErrorCode.PermissionDenied,
            "PLAN_RESTRICTED_FEATURE" or "PLANRESTRICTEDFEATURE" => TorBoxErrorCode.PlanRestrictedFeature,
            "DUPLICATE_ITEM" or "DUPLICATEITEM" => TorBoxErrorCode.DuplicateItem,
            "BREACH_OF_TOS" or "BREACHOFTOS" => TorBoxErrorCode.BreachOfTos,
            "ACTIVE_LIMIT" or "ACTIVELIMIT" => TorBoxErrorCode.ActiveLimit,
            "SEEDING_LIMIT" or "SEEDINGLIMIT" => TorBoxErrorCode.SeedingLimit,
            "BANNED_CONTENT_DETECTED" or "BANNEDCONTENTDETECTED" => TorBoxErrorCode.BannedContentDetected,
            "COULD_NOT_PERFORM_ACTION" or "COULDNOTPERFORMACTION" => TorBoxErrorCode.CouldNotPerformAction,
            "ITEM_NOT_FOUND" or "ITEMNOTFOUND" => TorBoxErrorCode.ItemNotFound,
            "INVALID_DEVICE" or "INVALIDDEVICE" => TorBoxErrorCode.InvalidDevice,
            "DEVICE_ALREADY_AUTHED" or "DEVICEALREADYAUTHED" => TorBoxErrorCode.DeviceAlreadyAuthed,
            "TOO_MANY_REQUESTS" or "TOOMANYREQUESTS" => TorBoxErrorCode.TooManyRequests,
            "DOWNLOAD_TOO_LARGE" or "DOWNLOADTOOLARGE" => TorBoxErrorCode.DownloadTooLarge,
            "MISSING_REQUIRED_OPTION" or "MISSINGREQUIREDOPTION" => TorBoxErrorCode.MissingRequiredOption,
            "BANNED_USER" or "BANNEDUSER" => TorBoxErrorCode.BannedUser,
            "SEARCH_ERROR" or "SEARCHERROR" => TorBoxErrorCode.SearchError,
            "SERVER_ERROR" or "SERVERERROR" => TorBoxErrorCode.ServerError,
            _ => TorBoxErrorCode.Unknown,
        };
    }
}
