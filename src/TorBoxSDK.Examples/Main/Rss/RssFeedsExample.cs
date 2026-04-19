using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Rss;

namespace TorBoxSDK.Examples.Main.Rss;

/// <summary>
/// Demonstrates how to manage RSS feeds: create, list, modify, and control.
/// </summary>
public static class RssFeedsExample
{
	public static async Task RunAsync()
	{
		ExampleHelper.PrintHeader("RSS — Manage Feeds");

		ITorBoxClient client = ExampleHelper.CreateClient();
		using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

		try
		{
			// ──────────────────────────────────────────────────────
			// Add a new RSS feed.
			// ──────────────────────────────────────────────────────
			string feedUrl = "https://example.com/rss/feed.xml"; // Replace with your actual RSS feed URL

			Console.WriteLine($"Adding RSS feed: {feedUrl}...");

			AddRssRequest addRequest = new()
			{
				Url = feedUrl,
				Name = "My Torrent Feed",         // Optional: display name
				ScanInterval = 60,                 // Optional: scan every 60 minutes
				RssType = "torrent",               // Feed type: "torrent" or "usenet"
				RegexFilter = "1080p",             // Optional: only match items containing "1080p"
				RegexFilterExclude = "CAM|HDTS",   // Optional: exclude low-quality releases
			};

			TorBoxResponse addResponse =
				await client.Main.Rss.AddRssAsync(addRequest, cts.Token);

			Console.WriteLine($"  Result: {addResponse.Detail ?? "Feed added"}");

			// ──────────────────────────────────────────────────────
			// List all RSS feeds.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Fetching RSS feeds...");

			TorBoxResponse<IReadOnlyList<RssFeed>> feedsResponse =
				await client.Main.Rss.GetFeedsAsync(cts.Token);

			if (feedsResponse.Data is null || feedsResponse.Data.Count == 0)
			{
				Console.WriteLine("No RSS feeds found.");
				return;
			}

			Console.WriteLine($"Found {feedsResponse.Data.Count} RSS feed(s):");
			Console.WriteLine();

			foreach (RssFeed feed in feedsResponse.Data)
			{
				string activeStatus = feed.Active ? "Active" : "Paused";
				Console.WriteLine($"  [{feed.Id}] {feed.Name ?? "Unnamed Feed"} ({activeStatus})");
				Console.WriteLine($"       URL: {feed.Url}");
				Console.WriteLine($"       Type: {feed.RssType ?? "N/A"}");
				Console.WriteLine($"       Scan interval: {feed.ScanInterval} min");

				if (feed.RegexFilter is not null)
				{
					Console.WriteLine($"       Include filter: {feed.RegexFilter}");
				}

				if (feed.RegexFilterExclude is not null)
				{
					Console.WriteLine($"       Exclude filter: {feed.RegexFilterExclude}");
				}

				Console.WriteLine();
			}

			// ──────────────────────────────────────────────────────
			// Get items from the first feed.
			// ──────────────────────────────────────────────────────
			long rssFeedId = feedsResponse.Data[0].Id; // Use the first feed's ID

			Console.WriteLine($"Fetching items for feed {rssFeedId}...");

			TorBoxResponse<IReadOnlyList<RssFeedItem>> itemsResponse =
				await client.Main.Rss.GetFeedItemsAsync(rssFeedId, cts.Token);

			if (itemsResponse.Data is not null && itemsResponse.Data.Count > 0)
			{
				Console.WriteLine($"  Found {itemsResponse.Data.Count} item(s):");

				int displayCount = Math.Min(5, itemsResponse.Data.Count); // Show first 5

				for (int i = 0; i < displayCount; i++)
				{
					RssFeedItem item = itemsResponse.Data[i];
					Console.WriteLine($"    - {item.Title ?? "Untitled"}");
					Console.WriteLine($"      Published: {item.PublishedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}");
				}

				if (itemsResponse.Data.Count > displayCount)
				{
					Console.WriteLine($"    ... and {itemsResponse.Data.Count - displayCount} more.");
				}
			}
			else
			{
				Console.WriteLine("  No items in this feed yet.");
			}

			// ──────────────────────────────────────────────────────
			// Modify a feed (update scan interval).
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine($"Modifying feed {rssFeedId} (changing scan interval)...");

			ModifyRssRequest modifyRequest = new()
			{
				RssFeedId = rssFeedId,
				ScanInterval = 30, // Change to scan every 30 minutes
			};

			TorBoxResponse modifyResponse =
				await client.Main.Rss.ModifyRssAsync(modifyRequest, cts.Token);

			Console.WriteLine($"  Result: {modifyResponse.Detail ?? "Feed modified"}");

			// ──────────────────────────────────────────────────────
			// Pause a feed.
			// ──────────────────────────────────────────────────────
			Console.WriteLine($"Pausing feed {rssFeedId}...");

			ControlRssRequest controlRequest = new()
			{
				RssFeedId = rssFeedId,
				Operation = ControlOperation.Pause,
			};

			TorBoxResponse controlResponse =
				await client.Main.Rss.ControlRssAsync(controlRequest, cts.Token);

			Console.WriteLine($"  Result: {controlResponse.Detail ?? "Feed paused"}");
		}
		catch (TorBoxException ex)
		{
			Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
		}

		Console.WriteLine();
		Console.WriteLine("RSS feeds example completed.");
	}
}
