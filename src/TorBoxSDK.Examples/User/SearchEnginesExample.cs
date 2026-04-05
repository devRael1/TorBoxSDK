using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Examples.User;

/// <summary>
/// Demonstrates how to manage custom search engines: add, list, modify, and control.
/// </summary>
public static class SearchEnginesExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("User — Search Engines Management");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // List existing search engines.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching search engines...");

            TorBoxResponse<IReadOnlyList<SearchEngine>> listResponse =
                await client.Main.User.GetSearchEnginesAsync(ct: cts.Token);

            if (listResponse.Data is not null && listResponse.Data.Count > 0)
            {
                Console.WriteLine($"Found {listResponse.Data.Count} search engine(s):");

                foreach (SearchEngine engine in listResponse.Data)
                {
                    string enabledIcon = engine.Enabled ? "✓" : "✗";
                    Console.WriteLine($"  [{enabledIcon}] {engine.Name ?? "N/A"}");
                }
            }
            else
            {
                Console.WriteLine("  No custom search engines configured.");
            }

            // ──────────────────────────────────────────────────────
            // Add a new custom search engine (Torznab/Newznab).
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Adding a custom Torznab search engine...");

            AddSearchEnginesRequest addRequest = new()
            {
                Type = "torznab",
                Url = "https://my-indexer.example.com/api",  // Replace with actual indexer URL
                Apikey = "your-indexer-api-key",             // Replace with actual API key
                DownloadType = "torrent",
            };

            TorBoxResponse addResponse =
                await client.Main.User.AddSearchEnginesAsync(addRequest, cts.Token);

            Console.WriteLine($"  Result: {addResponse.Detail ?? "Search engine added"}");

            // ──────────────────────────────────────────────────────
            // Modify an existing search engine.
            // ──────────────────────────────────────────────────────
            long engineId = 1; // Replace with the actual search engine ID

            Console.WriteLine();
            Console.WriteLine($"Modifying search engine {engineId}...");

            ModifySearchEnginesRequest modifyRequest = new()
            {
                Id = engineId,
                Url = "https://updated-indexer.example.com/api",  // Updated URL
                Apikey = "updated-api-key",                       // Updated API key
            };

            TorBoxResponse modifyResponse =
                await client.Main.User.ModifySearchEnginesAsync(modifyRequest, cts.Token);

            Console.WriteLine($"  Result: {modifyResponse.Detail ?? "Search engine modified"}");

            // ──────────────────────────────────────────────────────
            // Control search engines: enable, disable, or delete.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Disabling search engine {engineId}...");

            ControlSearchEnginesRequest disableRequest = new()
            {
                Operation = "disable",
                Id = engineId,
            };

            TorBoxResponse disableResponse =
                await client.Main.User.ControlSearchEnginesAsync(disableRequest, cts.Token);

            Console.WriteLine($"  Result: {disableResponse.Detail ?? "Search engine disabled"}");

            // Delete a specific search engine
            Console.WriteLine();
            Console.WriteLine($"Deleting search engine {engineId}...");

            ControlSearchEnginesRequest deleteRequest = new()
            {
                Operation = "delete",
                Id = engineId,
            };

            TorBoxResponse deleteResponse =
                await client.Main.User.ControlSearchEnginesAsync(deleteRequest, cts.Token);

            Console.WriteLine($"  Result: {deleteResponse.Detail ?? "Search engine deleted"}");

            // ──────────────────────────────────────────────────────
            // Get a specific search engine by ID.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Fetching search engine {engineId}...");

            TorBoxResponse<IReadOnlyList<SearchEngine>> singleResponse =
                await client.Main.User.GetSearchEnginesAsync(id: engineId, ct: cts.Token);

            if (singleResponse.Data is not null && singleResponse.Data.Count > 0)
            {
                SearchEngine engine = singleResponse.Data[0];
                Console.WriteLine($"  Name: {engine.Name ?? "N/A"}");
                Console.WriteLine($"  Enabled: {engine.Enabled}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Search engines example completed.");
    }
}
