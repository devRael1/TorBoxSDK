using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Examples.Main.Torrents;

/// <summary>
/// Demonstrates how to request download links for completed torrents.
/// </summary>
public static class DownloadTorrentExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Torrents — Request Download Link");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            long torrentId = 12345; // Replace with your actual torrent ID

            // ──────────────────────────────────────────────────────
            // Option 1: Get a download link for the entire torrent.
            // ──────────────────────────────────────────────────────
            Console.WriteLine($"Requesting download link for torrent {torrentId}...");

            TorBoxResponse<string> downloadResponse =
                await client.Main.Torrents.RequestDownloadAsync(torrentId, cancellationToken: cts.Token);

            if (downloadResponse.Data is not null)
            {
                Console.WriteLine($"  Download URL: {downloadResponse.Data}");
            }

            // ──────────────────────────────────────────────────────
            // Option 2: Get a download link for a specific file
            //           within the torrent.
            // ──────────────────────────────────────────────────────
            long fileId = 1; // Replace with the actual file ID from the torrent's Files list

            RequestDownloadOptions fileDownloadOptions = new()
            {
                FileId = fileId,
            };

            TorBoxResponse<string> fileDownloadResponse =
                await client.Main.Torrents.RequestDownloadAsync(torrentId, fileDownloadOptions, cts.Token);

            if (fileDownloadResponse.Data is not null)
            {
                Console.WriteLine($"  File download URL: {fileDownloadResponse.Data}");
            }

            // ──────────────────────────────────────────────────────
            // Option 3: Get a zipped download link.
            // ──────────────────────────────────────────────────────
            RequestDownloadOptions zipOptions = new()
            {
                ZipLink = true,
            };

            TorBoxResponse<string> zipResponse =
                await client.Main.Torrents.RequestDownloadAsync(torrentId, zipOptions, cts.Token);

            if (zipResponse.Data is not null)
            {
                Console.WriteLine($"  Zip download URL: {zipResponse.Data}");
            }

            // ──────────────────────────────────────────────────────
            // Export torrent data (e.g., magnet link).
            // ──────────────────────────────────────────────────────
            Console.WriteLine($"Exporting data for torrent {torrentId}...");

            TorBoxResponse<string> exportResponse =
                await client.Main.Torrents.ExportDataAsync(torrentId, cancellationToken: cts.Token);

            if (exportResponse.Data is not null)
            {
                Console.WriteLine($"  Exported data: {exportResponse.Data}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Download torrent example completed.");
    }
}
