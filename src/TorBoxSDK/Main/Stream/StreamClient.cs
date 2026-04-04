using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Main.Stream;

/// <summary>
/// Default implementation of <see cref="IStreamClient"/> for streaming
/// content through the TorBox Main API.
/// </summary>
public sealed class StreamClient : IStreamClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public StreamClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> CreateStreamAsync(long id, long fileId, string type, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(type, nameof(type));

        string query = TorBoxApiHelper.BuildQuery(
            ("id", id.ToString()),
            ("file_id", fileId.ToString()),
            ("type", type));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"stream/createstream{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> GetStreamDataAsync(long id, long fileId, string type, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(type, nameof(type));

        string query = TorBoxApiHelper.BuildQuery(
            ("id", id.ToString()),
            ("file_id", fileId.ToString()),
            ("type", type));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"stream/getstreamdata{query}");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }
}
