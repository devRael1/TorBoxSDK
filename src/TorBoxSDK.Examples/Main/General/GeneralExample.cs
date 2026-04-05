using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;

namespace TorBoxSDK.Examples.Main.General;

/// <summary>
/// Demonstrates how to use the General API: service status, stats, changelogs, and speed tests.
/// </summary>
public static class GeneralExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("General — Service Status & Stats");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Check if the TorBox API is online.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Checking API status...");

            TorBoxResponse<object> statusResponse =
                await client.Main.General.GetUpStatusAsync(cts.Token);

            Console.WriteLine($"  API is up: {statusResponse.Detail ?? "Online"}");

            // ──────────────────────────────────────────────────────
            // Get current stats.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching current stats...");

            TorBoxResponse<Stats> statsResponse =
                await client.Main.General.GetStatsAsync(cts.Token);

            if (statsResponse.Data is not null)
            {
                Console.WriteLine($"  Current stats: {statsResponse.Data}");
            }

            // ──────────────────────────────────────────────────────
            // Get 30-day stats.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching 30-day stats...");

            TorBoxResponse<Stats> stats30DayResponse =
                await client.Main.General.Get30DayStatsAsync(cts.Token);

            if (stats30DayResponse.Data is not null)
            {
                Console.WriteLine($"  30-day stats: {stats30DayResponse.Data}");
            }

            // ──────────────────────────────────────────────────────
            // Get changelogs in JSON format.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching changelogs...");

            TorBoxResponse<IReadOnlyList<Changelog>> changelogsResponse =
                await client.Main.General.GetChangelogsJsonAsync(cts.Token);

            if (changelogsResponse.Data is not null && changelogsResponse.Data.Count > 0)
            {
                int displayCount = Math.Min(3, changelogsResponse.Data.Count); // Show latest 3

                Console.WriteLine($"Latest changelogs ({changelogsResponse.Data.Count} total):");

                for (int i = 0; i < displayCount; i++)
                {
                    Changelog changelog = changelogsResponse.Data[i];
                    Console.WriteLine($"  - {changelog}");
                }
            }

            // ──────────────────────────────────────────────────────
            // Get changelogs as RSS feed.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching changelogs RSS...");

            TorBoxResponse<string> rssResponse =
                await client.Main.General.GetChangelogsRssAsync(cts.Token);

            if (rssResponse.Data is not null)
            {
                Console.WriteLine($"  RSS feed length: {rssResponse.Data.Length} chars");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("General example completed.");
    }
}
