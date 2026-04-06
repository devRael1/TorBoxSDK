using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Queued;

namespace TorBoxSDK.Examples.Main.Queued;

/// <summary>
/// Demonstrates how to manage queued downloads: list, filter, and control operations.
/// Queued downloads are items waiting to be processed by the server.
/// </summary>
public static class QueuedDownloadsExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Queued — Manage Queued Downloads");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // List all queued downloads.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching all queued downloads...");

            TorBoxResponse<IReadOnlyList<QueuedDownload>> queuedResponse =
                await client.Main.Queued.GetQueuedAsync(cancellationToken: cts.Token);

            if (queuedResponse.Data is not null && queuedResponse.Data.Count > 0)
            {
                Console.WriteLine($"Found {queuedResponse.Data.Count} queued download(s):");
                Console.WriteLine();

                foreach (QueuedDownload item in queuedResponse.Data)
                {
                    Console.WriteLine($"  [{item.Id}] {item.Name ?? "N/A"}");
                    Console.WriteLine($"       Type: {item.DownloadType ?? "N/A"} | Size: {ExampleHelper.FormatBytes(item.Size)}");
                    Console.WriteLine($"       Status: {item.Status ?? "N/A"} | Created: {item.CreatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("  No queued downloads found.");
            }

            // ──────────────────────────────────────────────────────
            // List queued downloads with pagination and type filter.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching queued torrent downloads (page 1, limit 10)...");

            TorBoxResponse<IReadOnlyList<QueuedDownload>> filteredResponse =
                await client.Main.Queued.GetQueuedAsync(
                    offset: 0,
                    limit: 10,
                    type: "torrent",
                    cancellationToken: cts.Token);

            Console.WriteLine($"  Queued torrents: {filteredResponse.Data?.Count ?? 0}");

            // ──────────────────────────────────────────────────────
            // Get a specific queued download by ID.
            // ──────────────────────────────────────────────────────
            long queuedId = 12345; // Replace with actual queued download ID

            Console.WriteLine();
            Console.WriteLine($"Fetching queued download {queuedId}...");

            TorBoxResponse<IReadOnlyList<QueuedDownload>> singleResponse =
                await client.Main.Queued.GetQueuedAsync(id: queuedId, cancellationToken: cts.Token);

            if (singleResponse.Data is not null && singleResponse.Data.Count > 0)
            {
                QueuedDownload item = singleResponse.Data[0];
                Console.WriteLine($"  Name: {item.Name ?? "N/A"}");
                Console.WriteLine($"  Hash: {item.Hash ?? "N/A"}");
            }

            // ──────────────────────────────────────────────────────
            // Control queued downloads: delete a specific item.
            // This is a destructive action — requires confirmation.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Type DELETE to confirm deleting this queued download, or press Enter to skip:");
            Console.Write("> ");
            string? deleteConfirmation = Console.ReadLine();

            if (string.Equals(deleteConfirmation, "DELETE", StringComparison.Ordinal))
            {
                ControlQueuedRequest deleteRequest = new()
                {
                    QueuedId = queuedId,
                    Operation = ControlOperation.Delete,
                };

                TorBoxResponse deleteResponse =
                    await client.Main.Queued.ControlQueuedAsync(deleteRequest, cts.Token);

                Console.WriteLine($"  Result: {deleteResponse.Detail ?? "Queued download deleted"}");
            }
            else
            {
                Console.WriteLine("  Skipped deleting the queued download.");
            }

            // ──────────────────────────────────────────────────────
            // Control queued downloads: delete ALL queued items.
            // Use with caution — this is irreversible!
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Type DELETE ALL to confirm deleting all queued downloads, or press Enter to skip:");
            Console.Write("> ");
            string? deleteAllConfirmation = Console.ReadLine();

            if (string.Equals(deleteAllConfirmation, "DELETE ALL", StringComparison.Ordinal))
            {
                ControlQueuedRequest deleteAllRequest = new()
                {
                    All = true,
                    Operation = ControlOperation.Delete,
                };

                TorBoxResponse deleteAllResponse =
                    await client.Main.Queued.ControlQueuedAsync(deleteAllRequest, cts.Token);

                Console.WriteLine($"  Result: {deleteAllResponse.Detail ?? "All queued downloads deleted"}");
            }
            else
            {
                Console.WriteLine("  Skipped deleting all queued downloads.");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Queued downloads example completed.");
    }
}
