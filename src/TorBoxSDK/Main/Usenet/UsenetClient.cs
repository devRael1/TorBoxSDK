using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Usenet;

namespace TorBoxSDK.Main.Usenet;

/// <summary>
/// Default implementation of <see cref="IUsenetClient"/> for managing
/// usenet downloads through the TorBox Main API.
/// </summary>
public sealed class UsenetClient : IUsenetClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsenetClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public UsenetClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<UsenetDownload>> CreateUsenetDownloadAsync(CreateUsenetDownloadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Link is null && request.File is null)
        {
            throw new ArgumentException("Either Link or File must be provided.", nameof(request));
        }

        var content = new MultipartFormDataContent();
        if (request.Link is not null)
        {
            content.Add(new StringContent(request.Link), "link");
        }

        if (request.File is not null)
        {
            content.Add(new ByteArrayContent(request.File), "file", "download.nzb");
        }

        if (request.Name is not null)
        {
            content.Add(new StringContent(request.Name), "name");
        }

        if (request.Password is not null)
        {
            content.Add(new StringContent(request.Password), "password");
        }

        if (request.PostProcessing is not null)
        {
            content.Add(new StringContent(request.PostProcessing), "post_processing");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "usenet/createusenetdownload") { Content = content };
        return await TorBoxApiHelper.SendAsync<UsenetDownload>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ControlUsenetDownloadAsync(ControlUsenetDownloadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "usenet/controlusenetdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> RequestDownloadAsync(RequestUsenetDownloadOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        string query = TorBoxApiHelper.BuildQuery(
            ("usenet_id", options.UsenetId.ToString()),
            ("file_id", options.FileId?.ToString()),
            ("zip_link", options.ZipLink?.ToString().ToLowerInvariant()),
            ("token", options.Token),
            ("user_ip", options.UserIp),
            ("redirect", options.Redirect?.ToString().ToLowerInvariant()),
            ("append_name", options.AppendName?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"usenet/requestdl{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<UsenetDownload>>> GetMyUsenetListAsync(long? id = null, int? offset = null, int? limit = null, bool? bypassCache = null, CancellationToken cancellationToken = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("bypass_cache", bypassCache?.ToString().ToLowerInvariant()),
            ("id", id?.ToString()),
            ("offset", offset?.ToString()),
            ("limit", limit?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"usenet/mylist{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<UsenetDownload>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> CheckCachedAsync(IReadOnlyList<string> hashes, string? format = null, bool? listFiles = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(hashes);

        string hashParam = string.Join(",", hashes);
        string query = TorBoxApiHelper.BuildQuery(
            ("hash", hashParam),
            ("format", format),
            ("list_files", listFiles?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"usenet/checkcached{query}");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> CheckCachedByPostAsync(CheckUsenetCachedRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "usenet/checkcached")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> EditUsenetDownloadAsync(EditUsenetDownloadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "usenet/editusenetdownload")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<UsenetDownload>> AsyncCreateUsenetDownloadAsync(CreateUsenetDownloadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Link is null && request.File is null)
        {
            throw new ArgumentException("Either Link or File must be provided.", nameof(request));
        }

        var content = new MultipartFormDataContent();
        if (request.Link is not null)
        {
            content.Add(new StringContent(request.Link), "link");
        }

        if (request.File is not null)
        {
            content.Add(new ByteArrayContent(request.File), "file", "download.nzb");
        }

        if (request.Name is not null)
        {
            content.Add(new StringContent(request.Name), "name");
        }

        if (request.Password is not null)
        {
            content.Add(new StringContent(request.Password), "password");
        }

        if (request.PostProcessing is not null)
        {
            content.Add(new StringContent(request.PostProcessing), "post_processing");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "usenet/asynccreateusenetdownload") { Content = content };
        return await TorBoxApiHelper.SendAsync<UsenetDownload>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
