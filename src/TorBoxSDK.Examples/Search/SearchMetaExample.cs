using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;

namespace TorBoxSDK.Examples.Search;

/// <summary>
/// Demonstrates how to use the meta search API to search across multiple sources.
/// </summary>
public static class SearchMetaExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Search — Meta Search (Movies, TV, etc.)");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Basic meta search.
            // ──────────────────────────────────────────────────────
            string query = "Inception"; // Replace with your search query

            Console.WriteLine($"Meta searching for: \"{query}\"...");

            TorBoxResponse<IReadOnlyList<MetaSearchResult>> searchResponse =
                await client.Search.SearchMetaAsync(query, ct: cts.Token);

            if (searchResponse.Data is null || searchResponse.Data.Count == 0)
            {
                Console.WriteLine("No meta results found.");
                return;
            }

            Console.WriteLine($"Found {searchResponse.Data.Count} meta result(s):");
            Console.WriteLine();

            foreach (MetaSearchResult result in searchResponse.Data)
            {
                Console.WriteLine($"  {result}");
                Console.WriteLine();
            }

            // ──────────────────────────────────────────────────────
            // Meta search with type filter.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Searching with type filter...");

            MetaSearchOptions metaOptions = new()
            {
                Type = "movie", // Filter by content type (e.g., "movie", "tv")
            };

            TorBoxResponse<IReadOnlyList<MetaSearchResult>> filteredResponse =
                await client.Search.SearchMetaAsync(query, metaOptions, cts.Token);

            Console.WriteLine($"Filtered meta results: {filteredResponse.Data?.Count ?? 0}");

            // ──────────────────────────────────────────────────────
            // Torznab and Newznab search (for indexer integration).
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Using Torznab search...");

            TorBoxResponse<string> torznabResponse =
                await client.Search.SearchTorznabAsync(query, ct: cts.Token);

            if (torznabResponse.Data is not null)
            {
                Console.WriteLine($"  Torznab XML response length: {torznabResponse.Data.Length} chars");
            }

            Console.WriteLine("Using Newznab search...");

            TorBoxResponse<string> newznabResponse =
                await client.Search.SearchNewznabAsync(query, ct: cts.Token);

            if (newznabResponse.Data is not null)
            {
                Console.WriteLine($"  Newznab XML response length: {newznabResponse.Data.Length} chars");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Meta search example completed.");
    }
}
