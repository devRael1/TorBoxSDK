using TorBoxSDK.Http;
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
    public async Task<TorBoxResponse<string>> CreateStreamAsync(CreateStreamOptions options, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(options);
        Guard.ThrowIfNullOrEmpty(options.Type, nameof(options.Type));

        string query = TorBoxApiHelper.BuildQuery(
            ("id", options.Id.ToString()),
            ("file_id", options.FileId.ToString()),
            ("type", options.Type),
            ("chosen_subtitle_index", options.ChosenSubtitleIndex?.ToString()),
            ("chosen_audio_index", options.ChosenAudioIndex?.ToString()),
            ("chosen_resolution_index", options.ChosenResolutionIndex?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"stream/createstream{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> GetStreamDataAsync(GetStreamDataOptions options, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(options);
        Guard.ThrowIfNullOrEmpty(options.PresignedToken, nameof(options.PresignedToken));
        Guard.ThrowIfNullOrEmpty(options.Token, nameof(options.Token));

        string query = TorBoxApiHelper.BuildQuery(
            ("presigned_token", options.PresignedToken),
            ("token", options.Token),
            ("chosen_subtitle_index", options.ChosenSubtitleIndex?.ToString()),
            ("chosen_audio_index", options.ChosenAudioIndex?.ToString()),
            ("chosen_resolution_index", options.ChosenResolutionIndex?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"stream/getstreamdata{query}");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
