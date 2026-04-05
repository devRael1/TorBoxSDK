using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.WebDownloads;

namespace TorBoxSDK.Main.WebDownloads;

/// <summary>
/// Default implementation of <see cref="IWebDownloadsClient"/> for managing
/// web downloads through the TorBox Main API.
/// </summary>
public sealed class WebDownloadsClient : IWebDownloadsClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDownloadsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public WebDownloadsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<WebDownload>> CreateWebDownloadAsync(CreateWebDownloadRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "webdl/createwebdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<WebDownload>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ControlWebDownloadAsync(ControlWebDownloadRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "webdl/controlwebdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> RequestDownloadAsync(RequestWebDownloadOptions options, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        string query = TorBoxApiHelper.BuildQuery(
            ("web_id", options.WebId.ToString()),
            ("file_id", options.FileId?.ToString()),
            ("zip_link", options.ZipLink?.ToString().ToLowerInvariant()),
            ("token", options.Token),
            ("user_ip", options.UserIp),
            ("redirect", options.Redirect?.ToString().ToLowerInvariant()),
            ("append_name", options.AppendName?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"webdl/requestdl{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<WebDownload>>> GetMyWebDownloadListAsync(long? id = null, int? offset = null, int? limit = null, bool? bypassCache = null, CancellationToken ct = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("bypass_cache", bypassCache?.ToString().ToLowerInvariant()),
            ("id", id?.ToString()),
            ("offset", offset?.ToString()),
            ("limit", limit?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"webdl/mylist{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<WebDownload>>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> CheckCachedAsync(IReadOnlyList<string> hashes, string? format = null, bool? listFiles = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(hashes);

        string hashParam = string.Join(",", hashes);
        string query = TorBoxApiHelper.BuildQuery(
            ("hash", hashParam),
            ("format", format),
            ("list_files", listFiles?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"webdl/checkcached{query}");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> CheckCachedByPostAsync(CheckWebCachedRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "webdl/checkcached")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Hoster>>> GetHostersAsync(CancellationToken ct = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "webdl/hosters");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Hoster>>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> EditWebDownloadAsync(EditWebDownloadRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "webdl/editwebdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<WebDownload>> AsyncCreateWebDownloadAsync(CreateWebDownloadRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "webdl/asynccreatewebdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<WebDownload>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }
}
