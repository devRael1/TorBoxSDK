using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Usenet;

namespace TorBoxSDK.Examples.Usenet;

/// <summary>
/// Demonstrates how to list and inspect Usenet downloads.
/// </summary>
public static class ListUsenetExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Usenet — List My Downloads");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Retrieve the full Usenet download list.
            // ──────────────────────────────────────────────────────
            TorBoxResponse<IReadOnlyList<UsenetDownload>> response =
                await client.Main.Usenet.GetMyUsenetListAsync(ct: cts.Token);

            if (response.Data is null || response.Data.Count == 0)
            {
                Console.WriteLine("No Usenet downloads found in your account.");
                return;
            }

            Console.WriteLine($"Found {response.Data.Count} Usenet download(s):");
            Console.WriteLine();

            foreach (UsenetDownload download in response.Data)
            {
                Console.WriteLine($"  [{download.Id}] {download.Name}");
                Console.WriteLine($"       Size: {ExampleHelper.FormatBytes(download.Size)}");
                Console.WriteLine($"       Progress: {download.Progress:P0}");
                Console.WriteLine($"       Status: {download.DownloadState ?? "unknown"}");
                Console.WriteLine($"       Speed: ↓ {ExampleHelper.FormatBytes(download.DownloadSpeed)}/s");
                Console.WriteLine();
            }

            // ──────────────────────────────────────────────────────
            // Retrieve a single Usenet download by ID.
            // ──────────────────────────────────────────────────────
            long downloadId = response.Data[0].Id; // Replace with your actual download ID

            TorBoxResponse<IReadOnlyList<UsenetDownload>> singleResponse =
                await client.Main.Usenet.GetMyUsenetListAsync(id: downloadId, ct: cts.Token);

            if (singleResponse.Data is not null && singleResponse.Data.Count > 0)
            {
                UsenetDownload single = singleResponse.Data[0];
                Console.WriteLine($"Single download details for ID {downloadId}:");
                Console.WriteLine($"  Name: {single.Name}");
                Console.WriteLine($"  Created: {single.CreatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}");
                Console.WriteLine($"  Finished: {single.DownloadFinished}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("List Usenet downloads example completed.");
    }
}
