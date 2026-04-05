using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.WebDownloads;

namespace TorBoxSDK.Examples.WebDownloads;

/// <summary>
/// Demonstrates how to list and inspect web downloads (direct link downloads).
/// </summary>
public static class ListWebDownloadsExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Web Downloads — List My Downloads");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Retrieve the full web download list.
            // ──────────────────────────────────────────────────────
            TorBoxResponse<IReadOnlyList<WebDownload>> response =
                await client.Main.WebDownloads.GetMyWebDownloadListAsync(ct: cts.Token);

            if (response.Data is null || response.Data.Count == 0)
            {
                Console.WriteLine("No web downloads found in your account.");
                return;
            }

            Console.WriteLine($"Found {response.Data.Count} web download(s):");
            Console.WriteLine();

            foreach (WebDownload download in response.Data)
            {
                Console.WriteLine($"  [{download.Id}] {download.Name}");
                Console.WriteLine($"       Size: {ExampleHelper.FormatBytes(download.Size)}");
                Console.WriteLine($"       Progress: {download.Progress:P0}");
                Console.WriteLine($"       Status: {download.DownloadState ?? "unknown"}");
                Console.WriteLine($"       Speed: ↓ {ExampleHelper.FormatBytes(download.DownloadSpeed)}/s");

                if (download.Error is not null)
                {
                    Console.WriteLine($"       Error: {download.Error}");
                }

                Console.WriteLine();
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("List web downloads example completed.");
    }
}
