using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.WebDownloads;

namespace TorBoxSDK.Examples.Main.WebDownloads;

/// <summary>
/// Demonstrates how to create web downloads from direct links and manage supported hosters.
/// </summary>
public static class CreateWebDownloadExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Web Downloads — Create Download & List Hosters");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // List supported hosters.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching supported hosters...");

            TorBoxResponse<IReadOnlyList<Hoster>> hostersResponse =
                await client.Main.WebDownloads.GetHostersAsync(cts.Token);

            if (hostersResponse.Data is not null && hostersResponse.Data.Count > 0)
            {
                Console.WriteLine($"Supported hosters ({hostersResponse.Data.Count}):");

                foreach (Hoster hoster in hostersResponse.Data)
                {
                    string status = hoster.Status ? "✓ Online" : "✗ Offline";
                    string bandwidth = hoster.DailyBandwidthLimit.HasValue
                        ? $" ({ExampleHelper.FormatBytes(hoster.DailyBandwidthUsed ?? 0)}/{ExampleHelper.FormatBytes(hoster.DailyBandwidthLimit.Value)})"
                        : "";

                    Console.WriteLine($"  [{status}] {hoster.Name}{bandwidth}");
                }

                Console.WriteLine();
            }

            // ──────────────────────────────────────────────────────
            // Create a web download from a direct link.
            // ──────────────────────────────────────────────────────
            string fileUrl = "https://example.com/largefile.zip"; // Replace with your actual URL

            Console.WriteLine($"Creating web download from: {fileUrl}...");

            CreateWebDownloadRequest createRequest = new()
            {
                Link = fileUrl,
                Name = "My Web Download", // Optional: override name
            };

            TorBoxResponse<WebDownload> createResponse =
                await client.Main.WebDownloads.CreateWebDownloadAsync(createRequest, cts.Token);

            if (createResponse.Data is not null)
            {
                Console.WriteLine($"  Created download ID: {createResponse.Data.Id}");
                Console.WriteLine($"  Name: {createResponse.Data.Name}");
                Console.WriteLine($"  Size: {ExampleHelper.FormatBytes(createResponse.Data.Size)}");
            }

            // ──────────────────────────────────────────────────────
            // Request a download link for a completed web download.
            // ──────────────────────────────────────────────────────
            long webDownloadId = 12345; // Replace with your actual web download ID

            Console.WriteLine($"Requesting download link for web download {webDownloadId}...");

            RequestWebDownloadOptions downloadOptions = new()
            {
                WebId = webDownloadId,
            };

            TorBoxResponse<string> downloadResponse =
                await client.Main.WebDownloads.RequestDownloadAsync(downloadOptions, cts.Token);

            if (downloadResponse.Data is not null)
            {
                Console.WriteLine($"  Download URL: {downloadResponse.Data}");
            }

            // ──────────────────────────────────────────────────────
            // Control a web download (delete).
            // This is a destructive action — requires confirmation.
            // ──────────────────────────────────────────────────────
            Console.WriteLine($"Delete web download {webDownloadId}? This action is irreversible.");
            Console.Write("Type DELETE to confirm, or press Enter to skip: ");
            string? deleteConfirmation = Console.ReadLine();

            if (string.Equals(deleteConfirmation, "DELETE", StringComparison.Ordinal))
            {
                Console.WriteLine($"Deleting web download {webDownloadId}...");

                ControlWebDownloadRequest controlRequest = new()
                {
                    WebdlId = webDownloadId,
                    Operation = ControlOperation.Delete,
                };

                TorBoxResponse controlResponse =
                    await client.Main.WebDownloads.ControlWebDownloadAsync(controlRequest, cts.Token);

                Console.WriteLine($"  Result: {controlResponse.Detail ?? "Success"}");
            }
            else
            {
                Console.WriteLine("Delete operation skipped.");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Create web download example completed.");
    }
}
