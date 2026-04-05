using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Usenet;

namespace TorBoxSDK.Examples.Main.Usenet;

/// <summary>
/// Demonstrates how to create and manage Usenet downloads.
/// </summary>
public static class CreateUsenetExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Usenet — Create Download");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Option 1: Create a Usenet download from an NZB link.
            // ──────────────────────────────────────────────────────
            string nzbLink = "https://example.com/file.nzb"; // Replace with your actual NZB link

            Console.WriteLine("Creating Usenet download from NZB link...");

            CreateUsenetDownloadRequest linkRequest = new()
            {
                Link = nzbLink,
                Name = "My Usenet Download", // Optional: override name
            };

            TorBoxResponse<UsenetDownload> linkResponse =
                await client.Main.Usenet.CreateUsenetDownloadAsync(linkRequest, cts.Token);

            if (linkResponse.Data is not null)
            {
                Console.WriteLine($"  Created download ID: {linkResponse.Data.Id}");
                Console.WriteLine($"  Name: {linkResponse.Data.Name}");
            }

            // ──────────────────────────────────────────────────────
            // Option 2: Create a Usenet download from an NZB file.
            // ──────────────────────────────────────────────────────
            string nzbFilePath = "/path/to/your/file.nzb"; // Replace with your actual file path

            if (File.Exists(nzbFilePath))
            {
                byte[] nzbFileBytes = await File.ReadAllBytesAsync(nzbFilePath, cts.Token);

                CreateUsenetDownloadRequest fileRequest = new()
                {
                    File = nzbFileBytes,
                    Name = "Usenet from NZB file",
                    Password = "archive_password", // Optional: archive password if needed
                };

                TorBoxResponse<UsenetDownload> fileResponse =
                    await client.Main.Usenet.CreateUsenetDownloadAsync(fileRequest, cts.Token);

                if (fileResponse.Data is not null)
                {
                    Console.WriteLine($"  Created from file, ID: {fileResponse.Data.Id}");
                }
            }
            else
            {
                Console.WriteLine($"  Skipping file-based creation: '{nzbFilePath}' not found.");
            }

            // ──────────────────────────────────────────────────────
            // Request a download link for a completed Usenet download.
            // ──────────────────────────────────────────────────────
            long usenetId = 12345; // Replace with your actual Usenet download ID

            Console.WriteLine($"Requesting download link for Usenet ID {usenetId}...");

            RequestUsenetDownloadOptions downloadOptions = new()
            {
                UsenetId = usenetId,
            };

            TorBoxResponse<string> downloadResponse =
                await client.Main.Usenet.RequestDownloadAsync(downloadOptions, cts.Token);

            if (downloadResponse.Data is not null)
            {
                Console.WriteLine($"  Download URL: {downloadResponse.Data}");
            }

            // ──────────────────────────────────────────────────────
            // Control a Usenet download (pause, resume, delete).
            // ──────────────────────────────────────────────────────
            Console.WriteLine($"Pausing Usenet download {usenetId}...");

            ControlUsenetDownloadRequest controlRequest = new()
            {
                UsenetId = usenetId,
                Operation = ControlOperation.Pause,
            };

            TorBoxResponse controlResponse =
                await client.Main.Usenet.ControlUsenetDownloadAsync(controlRequest, cts.Token);

            Console.WriteLine($"  Result: {controlResponse.Detail ?? "Success"}");
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Create Usenet download example completed.");
    }
}
