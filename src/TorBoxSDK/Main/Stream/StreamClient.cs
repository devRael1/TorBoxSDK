using TorBoxSDK.Http;
using TorBoxSDK.Http.Validation;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Stream;

namespace TorBoxSDK.Main.Stream;

/// <summary>
/// Default implementation of <see cref="IStreamClient"/> for streaming
/// content through the TorBox Main API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StreamClient"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client configured for the Main API.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class StreamClient(HttpClient httpClient) : IStreamClient
{
    private readonly HttpClient _httpClient = Guard.ThrowIfNull(httpClient);

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> CreateStreamAsync(long id, long fileId, string type, CreateStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(type, nameof(type));

        string query = TorBoxApiHelper.BuildQuery(
            ("id", id.ToString()),
            ("file_id", fileId.ToString()),
            ("type", type),
            ("chosen_subtitle_index", options?.ChosenSubtitleIndex?.ToString()),
            ("chosen_audio_index", options?.ChosenAudioIndex?.ToString()),
            ("chosen_resolution_index", options?.ChosenResolutionIndex?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"stream/createstream{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<StreamData>> GetStreamDataAsync(string presignedToken, string token, GetStreamDataOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(presignedToken, nameof(presignedToken));
        Guard.ThrowIfNullOrEmpty(token, nameof(token));

        string query = TorBoxApiHelper.BuildQuery(
            ("presigned_token", presignedToken),
            ("token", token),
            ("chosen_subtitle_index", options?.ChosenSubtitleIndex?.ToString()),
            ("chosen_audio_index", options?.ChosenAudioIndex?.ToString()),
            ("chosen_resolution_index", options?.ChosenResolutionIndex?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"stream/getstreamdata{query}");
        return await TorBoxApiHelper.SendAsync<StreamData>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
