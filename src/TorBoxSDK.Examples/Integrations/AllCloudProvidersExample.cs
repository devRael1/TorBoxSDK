using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;

namespace TorBoxSDK.Examples.Integrations;

/// <summary>
/// Demonstrates creating integration jobs for all supported cloud providers:
/// Dropbox, OneDrive, GoFile, 1Fichier, and Pixeldrain.
/// </summary>
public static class AllCloudProvidersExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Integrations — All Cloud Providers");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            long downloadId = 12345;          // Replace with your actual download ID
            string downloadType = "torrent";  // "torrent", "usenet", or "webdownload"

            CreateIntegrationJobRequest jobRequest = new()
            {
                DownloadId = downloadId,
                DownloadType = downloadType,
                Zip = false,
            };

            // ──────────────────────────────────────────────────────
            // Dropbox — transfer files to your Dropbox account.
            // Requires a connected Dropbox OAuth integration.
            // ──────────────────────────────────────────────────────
            Console.WriteLine($"Creating Dropbox job for download {downloadId}...");

            TorBoxResponse<IntegrationJob> dropboxResponse =
                await client.Main.Integrations.CreateDropboxJobAsync(jobRequest, cts.Token);

            if (dropboxResponse.Data is not null)
            {
                Console.WriteLine($"  Dropbox job ID: {dropboxResponse.Data.Id} | Status: {dropboxResponse.Data.Status ?? "N/A"}");
            }

            // ──────────────────────────────────────────────────────
            // OneDrive — transfer files to your OneDrive account.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Creating OneDrive job for download {downloadId}...");

            TorBoxResponse<IntegrationJob> onedriveResponse =
                await client.Main.Integrations.CreateOnedriveJobAsync(jobRequest, cts.Token);

            if (onedriveResponse.Data is not null)
            {
                Console.WriteLine($"  OneDrive job ID: {onedriveResponse.Data.Id} | Status: {onedriveResponse.Data.Status ?? "N/A"}");
            }

            // ──────────────────────────────────────────────────────
            // GoFile — transfer files to GoFile hosting.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Creating GoFile job for download {downloadId}...");

            TorBoxResponse<IntegrationJob> gofileResponse =
                await client.Main.Integrations.CreateGofileJobAsync(jobRequest, cts.Token);

            if (gofileResponse.Data is not null)
            {
                Console.WriteLine($"  GoFile job ID: {gofileResponse.Data.Id} | Status: {gofileResponse.Data.Status ?? "N/A"}");
            }

            // ──────────────────────────────────────────────────────
            // 1Fichier — transfer files to 1Fichier hosting.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Creating 1Fichier job for download {downloadId}...");

            TorBoxResponse<IntegrationJob> oneFichierResponse =
                await client.Main.Integrations.CreateOneFichierJobAsync(jobRequest, cts.Token);

            if (oneFichierResponse.Data is not null)
            {
                Console.WriteLine($"  1Fichier job ID: {oneFichierResponse.Data.Id} | Status: {oneFichierResponse.Data.Status ?? "N/A"}");
            }

            // ──────────────────────────────────────────────────────
            // Pixeldrain — transfer files to Pixeldrain hosting.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Creating Pixeldrain job for download {downloadId}...");

            TorBoxResponse<IntegrationJob> pixeldrainResponse =
                await client.Main.Integrations.CreatePixeldrainJobAsync(jobRequest, cts.Token);

            if (pixeldrainResponse.Data is not null)
            {
                Console.WriteLine($"  Pixeldrain job ID: {pixeldrainResponse.Data.Id} | Status: {pixeldrainResponse.Data.Status ?? "N/A"}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("All cloud providers example completed.");
    }
}
