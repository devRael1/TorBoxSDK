using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;

namespace TorBoxSDK.Examples.Search;

/// <summary>
/// Demonstrates how to search for torrents using the TorBox Search API.
/// </summary>
public static class SearchTorrentsExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Search — Search Torrents");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Basic torrent search.
            // ──────────────────────────────────────────────────────
            string query = "ubuntu 24.04"; // Replace with your search query

            Console.WriteLine($"Searching for torrents: \"{query}\"...");

            TorBoxResponse<IReadOnlyList<TorrentSearchResult>> searchResponse =
                await client.Search.SearchTorrentsAsync(query, ct: cts.Token);

            if (searchResponse.Data is null || searchResponse.Data.Count == 0)
            {
                Console.WriteLine("No results found.");
                return;
            }

            Console.WriteLine($"Found {searchResponse.Data.Count} result(s):");
            Console.WriteLine();

            foreach (TorrentSearchResult result in searchResponse.Data)
            {
                Console.WriteLine($"  {result.Name}");
                Console.WriteLine($"    Size: {ExampleHelper.FormatBytes(result.Size)}");
                Console.WriteLine($"    Seeds: {result.Seeders} | Leechers: {result.Leechers}");
                Console.WriteLine($"    Source: {result.Source ?? "N/A"}");
                Console.WriteLine($"    Hash: {result.Hash ?? "N/A"}");
                Console.WriteLine();
            }

            // ──────────────────────────────────────────────────────
            // Advanced search with options.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Searching with advanced options (cached only, with metadata)...");

            TorrentSearchOptions searchOptions = new()
            {
                Metadata = true,       // Include metadata in results
                CheckCache = true,     // Check if results are cached on TorBox
                CachedOnly = true,     // Only return cached results
            };

            TorBoxResponse<IReadOnlyList<TorrentSearchResult>> advancedResponse =
                await client.Search.SearchTorrentsAsync(query, searchOptions, cts.Token);

            Console.WriteLine($"Cached results: {advancedResponse.Data?.Count ?? 0}");

            // ──────────────────────────────────────────────────────
            // Search for TV show episodes.
            // ──────────────────────────────────────────────────────
            string tvQuery = "Game of Thrones"; // Replace with your TV show name
            int seasonNumber = 1;               // Replace with the season number
            int episodeNumber = 1;              // Replace with the episode number

            Console.WriteLine($"Searching for: \"{tvQuery}\" S{seasonNumber:D2}E{episodeNumber:D2}...");

            TorrentSearchOptions tvOptions = new()
            {
                Season = seasonNumber,
                Episode = episodeNumber,
                Metadata = true,
            };

            TorBoxResponse<IReadOnlyList<TorrentSearchResult>> tvResponse =
                await client.Search.SearchTorrentsAsync(tvQuery, tvOptions, cts.Token);

            Console.WriteLine($"TV results: {tvResponse.Data?.Count ?? 0}");

            // ──────────────────────────────────────────────────────
            // Get a specific torrent by ID.
            // ──────────────────────────────────────────────────────
            if (searchResponse.Data.Count > 0 && searchResponse.Data[0].Hash is { } torrentId)
            {
                Console.WriteLine($"Getting torrent details for: {torrentId}...");

                TorBoxResponse<TorrentSearchResult> detailResponse =
                    await client.Search.GetTorrentByIdAsync(torrentId, ct: cts.Token);

                if (detailResponse.Data is not null)
                {
                    Console.WriteLine($"  Name: {detailResponse.Data.Name}");
                    Console.WriteLine($"  Magnet: {detailResponse.Data.Magnet?[..Math.Min(80, detailResponse.Data.Magnet.Length)]}...");
                }
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Search torrents example completed.");
    }
}
