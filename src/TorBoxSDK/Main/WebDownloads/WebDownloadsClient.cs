using TorBoxSDK.Http;
using TorBoxSDK.Http.Validation;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.WebDownloads;

namespace TorBoxSDK.Main.WebDownloads;

/// <summary>
/// Default implementation of <see cref="IWebDownloadsClient"/> for managing
/// web downloads through the TorBox Main API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WebDownloadsClient"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client configured for the Main API.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class WebDownloadsClient(HttpClient httpClient) : IWebDownloadsClient
{
    private readonly HttpClient _httpClient = Guard.ThrowIfNull(httpClient);

    /// <inheritdoc />
    public async Task<TorBoxResponse<WebDownload>> CreateWebDownloadAsync(CreateWebDownloadRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "webdl/createwebdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<WebDownload>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ControlWebDownloadAsync(ControlWebDownloadRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "webdl/controlwebdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> RequestDownloadAsync(long webId, RequestWebDownloadOptions? options = null, CancellationToken cancellationToken = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("web_id", webId.ToString()),
            ("file_id", options?.FileId?.ToString()),
            ("zip_link", options?.ZipLink?.ToString().ToLowerInvariant()),
            ("token", options?.Token),
            ("user_ip", options?.UserIp),
            ("redirect", options?.Redirect?.ToString().ToLowerInvariant()),
            ("append_name", options?.AppendName?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"webdl/requestdl{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<WebDownload>>> GetMyWebDownloadListAsync(GetMyListOptions? options = null, CancellationToken cancellationToken = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("bypass_cache", options?.BypassCache?.ToString().ToLowerInvariant()),
            ("id", options?.Id?.ToString()),
            ("offset", options?.Offset?.ToString()),
            ("limit", options?.Limit?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"webdl/mylist{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<WebDownload>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<CheckWebCached>> CheckCachedAsync(IReadOnlyList<string> hashes, CheckCachedOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(hashes, nameof(hashes));

        string hashParam = string.Join(",", hashes);
        string query = TorBoxApiHelper.BuildQuery(
            ("hash", hashParam),
            ("format", options?.Format),
            ("list_files", options?.ListFiles?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"webdl/checkcached{query}");
        return await TorBoxApiHelper.SendAsync<CheckWebCached>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<CheckWebCached>> CheckCachedByPostAsync(CheckWebCachedRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "webdl/checkcached")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<CheckWebCached>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Hoster>>> GetHostersAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "webdl/hosters");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Hoster>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> EditWebDownloadAsync(EditWebDownloadRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "webdl/editwebdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<WebDownload>> AsyncCreateWebDownloadAsync(CreateWebDownloadRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "webdl/asynccreatewebdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<WebDownload>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
