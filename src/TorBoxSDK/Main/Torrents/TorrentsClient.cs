using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Main.Torrents;

/// <summary>
/// Default implementation of <see cref="ITorrentsClient"/> for managing
/// torrents through the TorBox Main API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TorrentsClient"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client configured for the Main API.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class TorrentsClient(HttpClient httpClient) : ITorrentsClient
{
    private readonly HttpClient _httpClient = Guard.ThrowIfNull(httpClient);

    /// <inheritdoc />
    public async Task<TorBoxResponse<Torrent>> CreateTorrentAsync(CreateTorrentRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        if (request.Magnet is null && request.File is null)
        {
            throw new ArgumentException("Either Magnet or File must be provided.", nameof(request));
        }

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

        if (request.AddOnlyIfCached is not null)
        {
            content.Add(new StringContent(request.AddOnlyIfCached.Value.ToString().ToLowerInvariant()), "add_only_if_cached");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/createtorrent") { Content = content };
        return await TorBoxApiHelper.SendAsync<Torrent>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ControlTorrentAsync(ControlTorrentRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/controltorrent")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> RequestDownloadAsync(RequestDownloadOptions options, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(options);

        string query = TorBoxApiHelper.BuildQuery(
            ("torrent_id", options.TorrentId.ToString()),
            ("file_id", options.FileId?.ToString()),
            ("zip_link", options.ZipLink?.ToString().ToLowerInvariant()),
            ("token", options.Token),
            ("user_ip", options.UserIp),
            ("redirect", options.Redirect?.ToString().ToLowerInvariant()),
            ("append_name", options.AppendName?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/requestdl{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Torrent>>> GetMyTorrentListAsync(GetMyListOptions? options = null, CancellationToken cancellationToken = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("bypass_cache", options?.BypassCache?.ToString().ToLowerInvariant()),
            ("id", options?.Id?.ToString()),
            ("offset", options?.Offset?.ToString()),
            ("limit", options?.Limit?.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/mylist{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Torrent>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> CheckCachedAsync(CheckCachedOptions options, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(options);
        Guard.ThrowIfNull(options.Hashes, $"{nameof(options)}.{nameof(CheckCachedOptions.Hashes)}");

        string hashParam = string.Join(",", options.Hashes);
        string query = TorBoxApiHelper.BuildQuery(
            ("hash", hashParam),
            ("format", options.Format),
            ("list_files", options.ListFiles?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/checkcached{query}");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> CheckCachedByPostAsync(CheckCachedRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/checkcached")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> ExportDataAsync(ExportDataOptions options, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(options);

        string query = TorBoxApiHelper.BuildQuery(
            ("torrent_id", options.TorrentId.ToString()),
            ("type", options.ExportType));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/exportdata{query}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<TorrentInfo>> GetTorrentInfoAsync(GetTorrentInfoOptions options, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(options);
        Guard.ThrowIfNullOrEmpty(options.Hash, nameof(options.Hash));

        string query = TorBoxApiHelper.BuildQuery(
            ("hash", options.Hash),
            ("timeout", options.Timeout?.ToString()),
            ("use_cache_lookup", options.UseCacheLookup?.ToString().ToLowerInvariant()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"torrents/torrentinfo{query}");
        return await TorBoxApiHelper.SendAsync<TorrentInfo>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<TorrentInfo>> GetTorrentInfoByFileAsync(TorrentInfoRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        if (request.File is null && request.Magnet is null && request.Hash is null)
        {
            throw new ArgumentException("At least one of File, Magnet, or Hash must be provided.", nameof(request));
        }

        MultipartFormDataContent content = [];

        if (request.File is not null)
        {
            string fileName = request.FileName ?? "torrent.torrent";
            content.Add(new ByteArrayContent(request.File), "file", fileName);
        }

        if (request.Magnet is not null)
        {
            content.Add(new StringContent(request.Magnet), "magnet");
        }

        if (request.Hash is not null)
        {
            content.Add(new StringContent(request.Hash), "hash");
        }

        if (request.Timeout is not null)
        {
            content.Add(new StringContent(request.Timeout.Value.ToString()), "timeout");
        }

        if (request.UseCacheLookup is not null)
        {
            content.Add(new StringContent(request.UseCacheLookup.Value.ToString().ToLowerInvariant()), "use_cache_lookup");
        }

        if (request.PeersOnly is not null)
        {
            content.Add(new StringContent(request.PeersOnly.Value.ToString().ToLowerInvariant()), "peers_only");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/torrentinfo") { Content = content };
        return await TorBoxApiHelper.SendAsync<TorrentInfo>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> EditTorrentAsync(EditTorrentRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "torrents/edittorrent")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<Torrent>> AsyncCreateTorrentAsync(CreateTorrentRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        if (request.Magnet is null && request.File is null)
        {
            throw new ArgumentException("Either Magnet or File must be provided.", nameof(request));
        }

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

        if (request.AddOnlyIfCached is not null)
        {
            content.Add(new StringContent(request.AddOnlyIfCached.Value.ToString().ToLowerInvariant()), "add_only_if_cached");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/asynccreatetorrent") { Content = content };
        return await TorBoxApiHelper.SendAsync<Torrent>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> MagnetToFileAsync(MagnetToFileRequest request, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "torrents/magnettofile")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
