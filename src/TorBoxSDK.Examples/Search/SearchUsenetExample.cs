using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;

namespace TorBoxSDK.Examples.Search;

/// <summary>
/// Demonstrates how to search Usenet using the TorBox Search API.
/// </summary>
public static class SearchUsenetExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Search — Search Usenet");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Basic Usenet search.
            // ──────────────────────────────────────────────────────
            string query = "ubuntu iso"; // Replace with your search query

            Console.WriteLine($"Searching Usenet for: \"{query}\"...");

            TorBoxResponse<UsenetSearchResponse> searchResponse =
                await client.Search.SearchUsenetAsync(query, cancellationToken: cts.Token);

            if (searchResponse.Data is null || searchResponse.Data.Nzbs.Count == 0)
            {
                Console.WriteLine("No Usenet results found.");
                return;
            }

            Console.WriteLine($"Found {searchResponse.Data.Nzbs.Count} Usenet result(s):");
            Console.WriteLine();

            foreach (UsenetSearchResult result in searchResponse.Data.Nzbs)
            {
                Console.WriteLine($"  {result.Name}");
                Console.WriteLine($"    Size: {ExampleHelper.FormatBytes(result.Size)}");
                Console.WriteLine($"    Source: {result.Source ?? "N/A"}");
                Console.WriteLine();
            }

            // ──────────────────────────────────────────────────────
            // Advanced Usenet search with season/episode filters.
            // ──────────────────────────────────────────────────────
            string tvQuery = "Breaking Bad"; // Replace with your search query
            int seasonNumber = 1;            // Replace with the season number
            int episodeNumber = 1;           // Replace with the episode number

            Console.WriteLine($"Searching Usenet for: \"{tvQuery}\" S{seasonNumber:D2}E{episodeNumber:D2}...");

            UsenetSearchOptions usenetOptions = new()
            {
                Season = seasonNumber,
                Episode = episodeNumber,
                CheckCache = true,
            };

            TorBoxResponse<UsenetSearchResponse> advancedResponse =
                await client.Search.SearchUsenetAsync(tvQuery, usenetOptions, cts.Token);

            Console.WriteLine($"Advanced results: {advancedResponse.Data?.Nzbs.Count ?? 0}");
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Search Usenet example completed.");
    }
}
