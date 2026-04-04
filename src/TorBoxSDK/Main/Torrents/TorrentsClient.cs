using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Main.Torrents;

/// <summary>
/// Default implementation of <see cref="ITorrentsClient"/> for managing
/// torrents through the TorBox Main API.
/// </summary>
public sealed class TorrentsClient : ITorrentsClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TorrentsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public TorrentsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<Torrent>> CreateTorrentAsync(CreateTorrentRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = new MultipartFormDataContent();
        if (request.Magnet is not null)
        {
            content.Add(new StringContent(request.Magnet), "magnet");
        }

        if (request.File is not null)
        {
            content.Add(new ByteArrayContent(request.File), "file", "torrent.torrent");
        }

        if (request.Name is not null)
        {
            content.Add(new StringContent(request.Name), "name");
        }

        if (request.Seed is not null)
        {
            content.Add(new StringContent(((int)request.Seed.Value).ToString()), "seed");
        }

        if (request.AllowZip is not null)
        {
            content.Add(new StringContent(request.AllowZip.Value.ToString().ToLowerInvariant()), "allow_zip");
        }

        if (request.AsQueued is not null)
        {
            content.Add(new StringContent(request.AsQueued.Value.ToString().ToLowerInvariant()), "as_queued");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/createtorrent") { Content = content };
        return await TorBoxApiHelper.SendAsync<Torrent>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ControlTorrentAsync(ControlTorrentRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/controltorrent")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> RequestDownloadAsync(RequestDownloadOptions options, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        string query = TorBoxApiHelper.BuildQuery(
            ("torrent_id", options.TorrentId.ToString()),
            ("file_id", options.FileId?.ToString()),
            ("zip_link", options.ZipLink?.ToString().ToLowerInvariant()),
            ("user_ip", options.UserIp),
            ("redirect", options.Redirect?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/requestdl{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Torrent>>> GetMyTorrentListAsync(long? id = null, int? offset = null, int? limit = null, bool? bypassCache = null, CancellationToken ct = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("bypass_cache", bypassCache?.ToString().ToLowerInvariant()),
            ("id", id?.ToString()),
            ("offset", offset?.ToString()),
            ("limit", limit?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/mylist{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Torrent>>(_httpClient, httpRequest, ct).ConfigureAwait(false);
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

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/checkcached{query}");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> CheckCachedByPostAsync(CheckCachedRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/checkcached")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> ExportDataAsync(long torrentId, string? type = null, CancellationToken ct = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("torrent_id", torrentId.ToString()),
            ("type", type));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/exportdata{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<TorrentInfo>> GetTorrentInfoAsync(string hash, int? timeout = null, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(hash, nameof(hash));

        string query = TorBoxApiHelper.BuildQuery(
            ("hash", hash),
            ("timeout", timeout?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/torrentinfo{query}");
        return await TorBoxApiHelper.SendAsync<TorrentInfo>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<TorrentInfo>> GetTorrentInfoByFileAsync(byte[] file, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(file), "file", "torrent.torrent");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/torrentinfo") { Content = content };
        return await TorBoxApiHelper.SendAsync<TorrentInfo>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> EditTorrentAsync(EditTorrentRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "torrents/edittorrent")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }
}
