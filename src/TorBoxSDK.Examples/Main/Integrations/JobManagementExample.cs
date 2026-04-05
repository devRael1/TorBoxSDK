using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;

namespace TorBoxSDK.Examples.Main.Integrations;

/// <summary>
/// Demonstrates integration job management: list, get details, search by hash, and delete.
/// </summary>
public static class JobManagementExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Integrations — Job Management");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // List all integration jobs.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching all integration jobs...");

            TorBoxResponse<IReadOnlyList<IntegrationJob>> jobsResponse =
                await client.Main.Integrations.GetJobsAsync(cts.Token);

            if (jobsResponse.Data is not null && jobsResponse.Data.Count > 0)
            {
                Console.WriteLine($"Found {jobsResponse.Data.Count} job(s):");

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
            long jobId = 12345; // Replace with actual job ID

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
                Console.WriteLine($"  Detail: {detail.Detail ?? "N/A"}");
                Console.WriteLine($"  Created: {detail.CreatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}");
            }

            // ──────────────────────────────────────────────────────
            // Search for jobs by download hash.
            // Useful for finding all cloud transfers related to a download.
            // ──────────────────────────────────────────────────────
            string downloadHash = "EXAMPLE_HASH"; // Replace with actual download hash

            Console.WriteLine();
            Console.WriteLine($"Searching jobs for hash: {downloadHash}...");

            TorBoxResponse<IReadOnlyList<IntegrationJob>> hashJobsResponse =
                await client.Main.Integrations.GetJobsByHashAsync(downloadHash, cts.Token);

            if (hashJobsResponse.Data is not null && hashJobsResponse.Data.Count > 0)
            {
                Console.WriteLine($"  Found {hashJobsResponse.Data.Count} job(s) for this hash:");

                foreach (IntegrationJob job in hashJobsResponse.Data)
                {
                    Console.WriteLine($"    [{job.Id}] {job.JobType ?? "N/A"} — {job.Status ?? "N/A"}");
                }
            }
            else
            {
                Console.WriteLine("  No jobs found for this hash.");
            }

            // ──────────────────────────────────────────────────────
            // Delete an integration job.
            // This is a destructive action — requires confirmation.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Ready to delete job {jobId}.");
            Console.Write("Type DELETE to confirm deletion, or press Enter to skip: ");

            string? deleteConfirmation = Console.ReadLine();

            if (string.Equals(deleteConfirmation, "DELETE", StringComparison.Ordinal))
            {
                TorBoxResponse deleteResponse =
                    await client.Main.Integrations.DeleteJobAsync(jobId, cts.Token);

                Console.WriteLine($"  Result: {deleteResponse.Detail ?? "Job deleted"}");
            }
            else
            {
                Console.WriteLine("  Deletion skipped.");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Job management example completed.");
    }
}
