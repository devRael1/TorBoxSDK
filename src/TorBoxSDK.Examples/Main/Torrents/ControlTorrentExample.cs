using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Examples.Main.Torrents;

/// <summary>
/// Demonstrates how to control torrents: pause, resume, recheck, and delete.
/// </summary>
public static class ControlTorrentExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Torrents — Control Operations");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            long torrentId = 12345; // Replace with your actual torrent ID

            // ──────────────────────────────────────────────────────
            // Pause a specific torrent.
            // ──────────────────────────────────────────────────────
            Console.WriteLine($"Pausing torrent {torrentId}...");

            ControlTorrentRequest pauseRequest = new()
            {
                TorrentId = torrentId,
                Operation = ControlOperation.Pause,
            };

            TorBoxResponse pauseResponse =
                await client.Main.Torrents.ControlTorrentAsync(pauseRequest, cts.Token);

            Console.WriteLine($"  Result: {pauseResponse.Detail ?? "Success"}");

            // ──────────────────────────────────────────────────────
            // Resume a specific torrent.
            // ──────────────────────────────────────────────────────
            Console.WriteLine($"Resuming torrent {torrentId}...");

            ControlTorrentRequest resumeRequest = new()
            {
                TorrentId = torrentId,
                Operation = ControlOperation.Resume,
            };

            TorBoxResponse resumeResponse =
                await client.Main.Torrents.ControlTorrentAsync(resumeRequest, cts.Token);

            Console.WriteLine($"  Result: {resumeResponse.Detail ?? "Success"}");

            // ──────────────────────────────────────────────────────
            // Recheck a specific torrent.
            // ──────────────────────────────────────────────────────
            Console.WriteLine($"Rechecking torrent {torrentId}...");

            ControlTorrentRequest recheckRequest = new()
            {
                TorrentId = torrentId,
                Operation = ControlOperation.Recheck,
            };

            TorBoxResponse recheckResponse =
                await client.Main.Torrents.ControlTorrentAsync(recheckRequest, cts.Token);

            Console.WriteLine($"  Result: {recheckResponse.Detail ?? "Success"}");

            // ──────────────────────────────────────────────────────
            // Delete a specific torrent.
            // This is a destructive action — requires confirmation.
            // ──────────────────────────────────────────────────────
            Console.WriteLine($"Delete torrent {torrentId}? This action is irreversible.");
            Console.Write("Type DELETE to confirm, or press Enter to skip: ");
            string? deleteConfirmation = Console.ReadLine();

            if (string.Equals(deleteConfirmation, "DELETE", StringComparison.Ordinal))
            {
                Console.WriteLine($"Deleting torrent {torrentId}...");

                ControlTorrentRequest deleteRequest = new()
                {
                    TorrentId = torrentId,
                    Operation = ControlOperation.Delete,
                };

                TorBoxResponse deleteResponse =
                    await client.Main.Torrents.ControlTorrentAsync(deleteRequest, cts.Token);

                Console.WriteLine($"  Result: {deleteResponse.Detail ?? "Success"}");
            }
            else
            {
                Console.WriteLine("Delete operation skipped.");
            }

            // ──────────────────────────────────────────────────────
            // Bulk operation: pause ALL torrents.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Pausing ALL torrents...");

            ControlTorrentRequest pauseAllRequest = new()
            {
                All = true,
                Operation = ControlOperation.Pause,
            };

            TorBoxResponse pauseAllResponse =
                await client.Main.Torrents.ControlTorrentAsync(pauseAllRequest, cts.Token);

            Console.WriteLine($"  Result: {pauseAllResponse.Detail ?? "All torrents paused"}");
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Control torrent example completed.");
    }
}
