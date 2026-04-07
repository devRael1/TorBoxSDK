using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Examples.Main.Torrents;

/// <summary>
/// Demonstrates how to list and inspect torrents using the Torrents client.
/// </summary>
public static class ListTorrentsExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Torrents — List My Torrents");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Retrieve the full torrent list.
            // ──────────────────────────────────────────────────────
            TorBoxResponse<IReadOnlyList<Torrent>> response =
                await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cts.Token);

            if (response.Data is null || response.Data.Count == 0)
            {
                Console.WriteLine("No torrents found in your account.");
                return;
            }

            Console.WriteLine($"Found {response.Data.Count} torrent(s):");
            Console.WriteLine();

            foreach (Torrent torrent in response.Data)
            {
                Console.WriteLine($"  [{torrent.Id}] {torrent.Name}");
                Console.WriteLine($"       Size: {ExampleHelper.FormatBytes(torrent.Size)}");
                Console.WriteLine($"       Progress: {torrent.Progress:P0}");
                Console.WriteLine($"       Status: {torrent.DownloadState ?? "unknown"}");
                Console.WriteLine($"       Seeds: {torrent.Seeds} | Peers: {torrent.Peers}");
                Console.WriteLine($"       Speed: ↓ {ExampleHelper.FormatBytes(torrent.DownloadSpeed)}/s | ↑ {ExampleHelper.FormatBytes(torrent.UploadSpeed)}/s");

                if (torrent.Files.Count > 0)
                {
                    Console.WriteLine($"       Files: {torrent.Files.Count}");
                }

                Console.WriteLine();
            }

            // ──────────────────────────────────────────────────────
            // Retrieve a single torrent by ID (using the first one).
            // ──────────────────────────────────────────────────────
            long torrentId = response.Data[0].Id; // Replace with your actual torrent ID
            TorBoxResponse<IReadOnlyList<Torrent>> singleResponse =
                await client.Main.Torrents.GetMyTorrentListAsync(new GetMyListOptions { Id = torrentId }, cancellationToken: cts.Token);

            if (singleResponse.Data is not null && singleResponse.Data.Count > 0)
            {
                Torrent single = singleResponse.Data[0];
                Console.WriteLine($"Single torrent details for ID {torrentId}:");
                Console.WriteLine($"  Name: {single.Name}");
                Console.WriteLine($"  Hash: {single.Hash ?? "N/A"}");
                Console.WriteLine($"  Created: {single.CreatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}");
                Console.WriteLine($"  Active: {single.Active}");
            }

            // ──────────────────────────────────────────────────────
            // Pagination: retrieve torrents with offset and limit.
            // ──────────────────────────────────────────────────────
            int offset = 0; // Start from the first item
            int limit = 10; // Retrieve 10 items at a time

            TorBoxResponse<IReadOnlyList<Torrent>> pagedResponse =
                await client.Main.Torrents.GetMyTorrentListAsync(
                    new GetMyListOptions { Offset = offset, Limit = limit },
                    cancellationToken: cts.Token);

            Console.WriteLine($"Paginated response: {pagedResponse.Data?.Count ?? 0} torrent(s) returned.");
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("List torrents example completed.");
    }
}
