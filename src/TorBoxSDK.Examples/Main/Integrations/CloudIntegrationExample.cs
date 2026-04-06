using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;

namespace TorBoxSDK.Examples.Main.Integrations;

/// <summary>
/// Demonstrates how to use cloud storage integrations (Google Drive, Dropbox, OneDrive, etc.)
/// to automatically transfer completed downloads to your preferred cloud provider.
/// </summary>
public static class CloudIntegrationExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Integrations — Cloud Storage");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // List connected OAuth integrations.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching connected integrations...");

            TorBoxResponse<IReadOnlyDictionary<string, bool>> oauthResponse =
                await client.Main.Integrations.GetOAuthMeAsync(cts.Token);

            if (oauthResponse.Data is not null && oauthResponse.Data.Count > 0)
            {
                Console.WriteLine($"Connected integrations ({oauthResponse.Data.Count}):");

                foreach (KeyValuePair<string, bool> entry in oauthResponse.Data)
                {
                    string status = entry.Value ? "connected" : "not connected";
                    Console.WriteLine($"  - {entry.Key}: {status}");
                }

                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No OAuth integrations found.");
                Console.WriteLine("  Connect via: client.Main.Integrations.OAuthRedirectAsync(provider)");
                Console.WriteLine();
            }

            // ──────────────────────────────────────────────────────
            // Create an integration job (e.g., send to Google Drive).
            // Requires a connected Google Drive OAuth integration.
            // ──────────────────────────────────────────────────────
            long downloadId = 12345; // Replace with your actual download ID

            Console.WriteLine($"Creating Google Drive integration job for download {downloadId}...");

            CreateIntegrationJobRequest jobRequest = new()
            {
                DownloadId = downloadId,
                DownloadType = "torrent",   // Download type: "torrent", "usenet", or "webdownload"
                Zip = false,                // Optional: zip files before transfer
            };

            TorBoxResponse<IntegrationJob> jobResponse =
                await client.Main.Integrations.CreateGoogleDriveJobAsync(jobRequest, cts.Token);

            if (jobResponse.Data is not null)
            {
                Console.WriteLine($"  Job created - ID: {jobResponse.Data.Id}");
                Console.WriteLine($"  Status: {jobResponse.Data.Status ?? "N/A"}");
                Console.WriteLine($"  Progress: {jobResponse.Data.Progress:P0}");
            }

            // ──────────────────────────────────────────────────────
            // Other cloud providers follow the same pattern:
            //   - CreateDropboxJobAsync(request)
            //   - CreateOnedriveJobAsync(request)
            //   - CreateGofileJobAsync(request)
            //   - CreateOneFichierJobAsync(request)
            //   - CreatePixeldrainJobAsync(request)
            // ──────────────────────────────────────────────────────

            // ──────────────────────────────────────────────────────
            // List all integration jobs.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching all integration jobs...");

            TorBoxResponse<IReadOnlyList<IntegrationJob>> jobsResponse =
                await client.Main.Integrations.GetJobsAsync(cts.Token);

            if (jobsResponse.Data is not null && jobsResponse.Data.Count > 0)
            {
                Console.WriteLine($"Found {jobsResponse.Data.Count} integration job(s):");

                foreach (IntegrationJob job in jobsResponse.Data)
                {
                    Console.WriteLine($"  [{job.Id}] Type: {job.JobType ?? "N/A"} | Status: {job.Status ?? "N/A"} | Progress: {job.Progress:P0}");
                }
            }
            else
            {
                Console.WriteLine("  No integration jobs found.");
            }

            // ──────────────────────────────────────────────────────
            // Get a specific job's details.
            // ──────────────────────────────────────────────────────
            long jobId = 12345; // Replace with an actual job ID

            Console.WriteLine();
            Console.WriteLine($"Getting details for job {jobId}...");

            TorBoxResponse<IntegrationJob> jobDetailResponse =
                await client.Main.Integrations.GetJobAsync(jobId, cts.Token);

            if (jobDetailResponse.Data is not null)
            {
                IntegrationJob detail = jobDetailResponse.Data;
                Console.WriteLine($"  Job type: {detail.JobType ?? "N/A"}");
                Console.WriteLine($"  Status: {detail.Status ?? "N/A"}");
                Console.WriteLine($"  Progress: {detail.Progress:P0}");
                Console.WriteLine($"  Created: {detail.CreatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Cloud integration example completed.");
    }
}
