using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;

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

            TorBoxResponse<IReadOnlyList<DailyStats>> stats30DayResponse =
                await client.Main.General.Get30DayStatsAsync(cts.Token);

            if (stats30DayResponse.Data is not null)
            {
                Console.WriteLine($"  30-day stats: {stats30DayResponse.Data.Count} snapshots");
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
                    Console.WriteLine($"  [{changelog.Id}] {changelog.Name}");
                    if (changelog.Link is not null)
                        Console.WriteLine($"      Link: {changelog.Link}");
                    if (changelog.CreatedAt is not null)
                        Console.WriteLine($"      Date: {changelog.CreatedAt:yyyy-MM-dd}");
                }
            }

            // ──────────────────────────────────────────────────────
            // Get changelogs as RSS feed.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching changelogs RSS...");

            TorBoxResponse<RssFeed> rssResponse =
                await client.Main.General.GetChangelogsRssAsync(cts.Token);

            if (rssResponse.Data?.Channel is not null)
            {
                RssChannel channel = rssResponse.Data.Channel;
                Console.WriteLine($"  Feed title: {channel.Title}");
                Console.WriteLine($"  Items: {channel.Items.Count}");

                int displayCount = Math.Min(3, channel.Items.Count);
                for (int i = 0; i < displayCount; i++)
                {
                    RssItem item = channel.Items[i];
                    Console.WriteLine($"    - {item.Title} ({item.PubDate})");
                }
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
