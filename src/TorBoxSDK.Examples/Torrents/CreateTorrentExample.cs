using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Examples.Torrents;

/// <summary>
/// Demonstrates how to create torrents from magnet links and .torrent files.
/// </summary>
public static class CreateTorrentExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Torrents — Create Torrent");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Option 1: Create a torrent from a magnet link.
            // ──────────────────────────────────────────────────────
            string magnetLink = "magnet:?xt=urn:btih:EXAMPLE_HASH"; // Replace with your actual magnet link

            CreateTorrentRequest magnetRequest = new CreateTorrentRequest()
            {
                Magnet = magnetLink,
                Name = "My Custom Torrent Name", // Optional: override the torrent name
                Seed = SeedPreference.Auto, // Optional: seeding behavior
                AllowZip = true, // Optional: allow zipping files
            };

            TorBoxResponse<Torrent> magnetResponse =
                await client.Main.Torrents.CreateTorrentAsync(magnetRequest, cts.Token);

            if (magnetResponse.Data is not null)
            {
                Console.WriteLine($"Torrent created from magnet link:");
                Console.WriteLine($"  ID: {magnetResponse.Data.Id}");
                Console.WriteLine($"  Name: {magnetResponse.Data.Name}");
                Console.WriteLine($"  Hash: {magnetResponse.Data.Hash ?? "pending"}");
            }

            // ──────────────────────────────────────────────────────
            // Option 2: Create a torrent from a .torrent file.
            // ──────────────────────────────────────────────────────
            string torrentFilePath = "/path/to/your/file.torrent"; // Replace with your actual file path

            if (File.Exists(torrentFilePath))
            {
                byte[] torrentFileBytes = await File.ReadAllBytesAsync(torrentFilePath, cts.Token);

                CreateTorrentRequest fileRequest = new()
                {
                    File = torrentFileBytes,
                    Name = "Torrent from file",
                };

                TorBoxResponse<Torrent> fileResponse =
                    await client.Main.Torrents.CreateTorrentAsync(fileRequest, cts.Token);

                if (fileResponse.Data is not null)
                {
                    Console.WriteLine($"Torrent created from file:");
                    Console.WriteLine($"  ID: {fileResponse.Data.Id}");
                    Console.WriteLine($"  Name: {fileResponse.Data.Name}");
                }
            }
            else
            {
                Console.WriteLine($"Skipping file-based creation: '{torrentFilePath}' not found.");
            }

            // ──────────────────────────────────────────────────────
            // Option 3: Create a torrent asynchronously.
            //           This returns immediately and processes in the background.
            // ──────────────────────────────────────────────────────
            CreateTorrentRequest asyncRequest = new()
            {
                Magnet = magnetLink,
                AsQueued = true, // Add to queue instead of starting immediately
            };

            TorBoxResponse<Torrent> asyncResponse =
                await client.Main.Torrents.AsyncCreateTorrentAsync(asyncRequest, cts.Token);

            if (asyncResponse.Data is not null)
            {
                Console.WriteLine($"Torrent queued asynchronously:");
                Console.WriteLine($"  ID: {asyncResponse.Data.Id}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Create torrent example completed.");
    }
}
