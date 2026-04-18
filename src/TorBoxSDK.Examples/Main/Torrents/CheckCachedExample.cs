using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Examples.Main.Torrents;

/// <summary>
/// Demonstrates how to check if torrent hashes are already cached on TorBox servers.
/// Cached torrents are available for instant download without waiting.
/// </summary>
public static class CheckCachedExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Torrents — Check Cached");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Option 1: Check cache status via GET (list of hashes).
            // ──────────────────────────────────────────────────────
            IReadOnlyList<string> hashes =
            [
                "EXAMPLE_HASH_1", // Replace with actual torrent info hashes
                "EXAMPLE_HASH_2",
                "EXAMPLE_HASH_3",
            ];

            Console.WriteLine($"Checking cache for {hashes.Count} hash(es) via GET...");

            TorBoxResponse<CheckCached> cacheResponse =
                await client.Main.Torrents.CheckCachedAsync(hashes, cancellationToken: cts.Token);

            Console.WriteLine($"  Response: {cacheResponse.Detail ?? "Check completed"}");

            // ──────────────────────────────────────────────────────
            // Option 2: Check cache status via POST (for large batches).
            //           Recommended when checking more than 10 hashes.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Checking cache via POST (for large batches)...");

            CheckCachedRequest postRequest = new()
            {
                Hashes = hashes,
                Format = "object",   // Response format
                ListFiles = true,    // Include file listings in response
            };

            TorBoxResponse<CheckCached> postResponse =
                await client.Main.Torrents.CheckCachedByPostAsync(postRequest, cts.Token);

            Console.WriteLine($"  Response: {postResponse.Detail ?? "Check completed"}");

            // ──────────────────────────────────────────────────────
            // Get detailed torrent info from a hash.
            // ──────────────────────────────────────────────────────
            string torrentHash = "EXAMPLE_HASH"; // Replace with an actual torrent info hash

            Console.WriteLine($"Getting torrent info for hash: {torrentHash}...");

            TorBoxResponse<TorrentInfo> infoResponse =
                await client.Main.Torrents.GetTorrentInfoAsync(torrentHash, cancellationToken: cts.Token);

            if (infoResponse.Data is not null)
            {
                Console.WriteLine($"  Torrent name: {infoResponse.Data.Name}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Check cached example completed.");
    }
}
