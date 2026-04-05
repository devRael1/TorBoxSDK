using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.WebDownloads;

namespace TorBoxSDK.Examples.Main.WebDownloads;

/// <summary>
/// Demonstrates advanced web download operations: cache checks, editing, and async creation.
/// </summary>
public static class WebDownloadsAdvancedExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Web Downloads — Cache Check, Edit & Async Create");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Check if web download URLs are cached (GET method).
            // ──────────────────────────────────────────────────────
            IReadOnlyList<string> hashes = ["EXAMPLE_HASH_1", "EXAMPLE_HASH_2"]; // Replace with actual hashes

            Console.WriteLine($"Checking cache for {hashes.Count} web download hash(es) (GET)...");

            TorBoxResponse<object> cacheResponse =
                await client.Main.WebDownloads.CheckCachedAsync(hashes, format: "object", listFiles: true, ct: cts.Token);

            Console.WriteLine($"  Cache check result: {cacheResponse.Detail ?? "Completed"}");

            // ──────────────────────────────────────────────────────
            // Check cache via POST (for large hash lists).
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Checking cache via POST...");

            CheckWebCachedRequest cacheRequest = new()
            {
                Hashes = ["EXAMPLE_HASH_1", "EXAMPLE_HASH_2"],
                Format = "object",
                ListFiles = true,
            };

            TorBoxResponse<object> cachePostResponse =
                await client.Main.WebDownloads.CheckCachedByPostAsync(cacheRequest, cts.Token);

            Console.WriteLine($"  POST cache check result: {cachePostResponse.Detail ?? "Completed"}");

            // ──────────────────────────────────────────────────────
            // Edit a web download (rename, add tags).
            // ──────────────────────────────────────────────────────
            long webDownloadId = 12345; // Replace with your actual web download ID

            Console.WriteLine();
            Console.WriteLine($"Editing web download {webDownloadId}...");

            EditWebDownloadRequest editRequest = new()
            {
                WebdlId = webDownloadId,
                Name = "My Renamed Web Download",
                Tags = ["file", "download"],
            };

            TorBoxResponse editResponse =
                await client.Main.WebDownloads.EditWebDownloadAsync(editRequest, cts.Token);

            Console.WriteLine($"  Result: {editResponse.Detail ?? "Download updated"}");

            // ──────────────────────────────────────────────────────
            // Create a web download asynchronously.
            // Returns immediately while the server processes it.
            // ──────────────────────────────────────────────────────
            string downloadUrl = "https://example.com/largefile.zip"; // Replace with actual URL

            Console.WriteLine();
            Console.WriteLine("Creating web download (async)...");

            CreateWebDownloadRequest asyncRequest = new()
            {
                Link = downloadUrl,
                Name = "My Async Web Download",
            };

            TorBoxResponse<WebDownload> asyncResponse =
                await client.Main.WebDownloads.AsyncCreateWebDownloadAsync(asyncRequest, cts.Token);

            if (asyncResponse.Data is not null)
            {
                Console.WriteLine($"  Async download created - ID: {asyncResponse.Data.Id}");
                Console.WriteLine($"  Name: {asyncResponse.Data.Name ?? "N/A"}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Web downloads advanced example completed.");
    }
}
