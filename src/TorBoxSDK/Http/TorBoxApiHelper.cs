using System.Text.Json;
using TorBoxSDK.Http.Json;
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
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        string json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        TorBoxResponse<T>? result = JsonSerializer.Deserialize<TorBoxResponse<T>>(json, TorBoxJsonOptions.Default) ?? throw new TorBoxException("Failed to deserialize API response.", TorBoxErrorCode.UnknownError, json);

        if (!result.Success)
        {
            TorBoxErrorCode errorCode = ParseErrorCode(result.Error);
            throw new TorBoxException(result.Detail ?? result.Error ?? "Unknown API error.", errorCode, result.Detail);
        }

        return !response.IsSuccessStatusCode
            ? throw new TorBoxException($"HTTP {(int)response.StatusCode}: {result.Detail ?? response.ReasonPhrase}", TorBoxErrorCode.ServerError, result.Detail)
            : result;
    }

    /// <summary>Sends a request and deserializes the non-generic TorBox response envelope.</summary>
    internal static async Task<TorBoxResponse> SendAsync(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        string json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        TorBoxResponse? result = JsonSerializer.Deserialize<TorBoxResponse>(json, TorBoxJsonOptions.Default) ?? throw new TorBoxException("Failed to deserialize API response.", TorBoxErrorCode.UnknownError, json);

        if (!result.Success)
        {
            TorBoxErrorCode errorCode = ParseErrorCode(result.Error);
            throw new TorBoxException(
                result.Detail ?? result.Error ?? "Unknown API error.",
                errorCode,
                result.Detail);
        }

        return !response.IsSuccessStatusCode
            ? throw new TorBoxException($"HTTP {(int)response.StatusCode}: {result.Detail ?? response.ReasonPhrase}", TorBoxErrorCode.ServerError, result.Detail)
            : result;
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

        string normalizedError = error.ToUpperInvariant().Replace(" ", "").Replace("_", "");

        return normalizedError switch
        {
            "UNKNOWNERROR" or "UNKNOWN" => TorBoxErrorCode.UnknownError,
            "DATABASEERROR" => TorBoxErrorCode.DatabaseError,
            "BADTOKEN" => TorBoxErrorCode.BadToken,
            "NOAUTH" => TorBoxErrorCode.NoAuth,
            "INVALIDOPTION" => TorBoxErrorCode.InvalidOption,
            "PERMISSIONDENIED" => TorBoxErrorCode.PermissionDenied,
            "PLANRESTRICTEDFEATURE" => TorBoxErrorCode.PlanRestrictedFeature,
            "DUPLICATEITEM" => TorBoxErrorCode.DuplicateItem,
            "BREACHOFTOS" => TorBoxErrorCode.BreachOfTos,
            "ACTIVELIMIT" => TorBoxErrorCode.ActiveLimit,
            "SEEDINGLIMIT" => TorBoxErrorCode.SeedingLimit,
            "BANNEDCONTENTDETECTED" => TorBoxErrorCode.BannedContentDetected,
            "COULDNOTPERFORMACTION" => TorBoxErrorCode.CouldNotPerformAction,
            "ITEMNOTFOUND" => TorBoxErrorCode.ItemNotFound,
            "INVALIDDEVICE" => TorBoxErrorCode.InvalidDevice,
            "DEVICEALREADYAUTHED" => TorBoxErrorCode.DeviceAlreadyAuthed,
            "TOOMANYREQUESTS" => TorBoxErrorCode.TooManyRequests,
            "DOWNLOADTOOLARGE" => TorBoxErrorCode.DownloadTooLarge,
            "MISSINGREQUIREDOPTION" => TorBoxErrorCode.MissingRequiredOption,
            "BANNEDUSER" => TorBoxErrorCode.BannedUser,
            "SEARCHERROR" => TorBoxErrorCode.SearchError,
            "SERVERERROR" => TorBoxErrorCode.ServerError,
            _ => TorBoxErrorCode.Unknown,
        };
    }
}
