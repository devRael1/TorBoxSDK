using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Examples.Main.User;

/// <summary>
/// Demonstrates how to manage custom search engines: add, list, modify, and control.
/// </summary>
public static class SearchEnginesExample
{
	public static async Task RunAsync()
	{
		ExampleHelper.PrintHeader("User — Search Engines Management");

		ITorBoxClient client = ExampleHelper.CreateClient();
		using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

		try
		{
			// ──────────────────────────────────────────────────────
			// List existing search engines.
			// ──────────────────────────────────────────────────────
			Console.WriteLine("Fetching search engines...");

			TorBoxResponse<IReadOnlyList<SearchEngine>> listResponse =
				await client.Main.User.GetSearchEnginesAsync(cancellationToken: cts.Token);

			if (listResponse.Data is not null && listResponse.Data.Count > 0)
			{
				Console.WriteLine($"Found {listResponse.Data.Count} search engine(s):");

				foreach (SearchEngine engine in listResponse.Data)
				{
					string enabledIcon = engine.Enabled ? "✓" : "✗";
					Console.WriteLine($"  [{enabledIcon}] {engine.Name ?? "N/A"}");
				}
			}
			else
			{
				Console.WriteLine("  No custom search engines configured.");
			}

			// ──────────────────────────────────────────────────────
			// Add a new custom search engine (Torznab/Newznab).
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Adding a custom Torznab search engine...");

			string indexerApiKey = Environment.GetEnvironmentVariable("TORBOX_INDEXER_API_KEY")
				?? throw new InvalidOperationException(
					"Set the TORBOX_INDEXER_API_KEY environment variable to your external indexer API key.");

			AddSearchEnginesRequest addRequest = new()
			{
				Type = "torznab",
				Url = "https://my-indexer.example.com/api",  // Replace with actual indexer URL
				Apikey = indexerApiKey,
				DownloadType = "torrent",
			};

			TorBoxResponse addResponse =
				await client.Main.User.AddSearchEnginesAsync(addRequest, cts.Token);

			Console.WriteLine($"  Result: {addResponse.Detail ?? "Search engine added"}");

			// ──────────────────────────────────────────────────────
			// Modify an existing search engine.
			// ──────────────────────────────────────────────────────
			long engineId = 1; // Replace with the actual search engine ID

			Console.WriteLine();
			Console.WriteLine($"Modifying search engine {engineId}...");

			ModifySearchEnginesRequest modifyRequest = new()
			{
				Id = engineId,
				Url = "https://updated-indexer.example.com/api",  // Updated URL
				Apikey = indexerApiKey,                            // Reuse the env-var-sourced API key
			};

			TorBoxResponse modifyResponse =
				await client.Main.User.ModifySearchEnginesAsync(modifyRequest, cts.Token);

			Console.WriteLine($"  Result: {modifyResponse.Detail ?? "Search engine modified"}");

			// ──────────────────────────────────────────────────────
			// Control search engines: enable, disable, or delete.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine($"Disabling search engine {engineId}...");

			ControlSearchEnginesRequest disableRequest = new()
			{
				Operation = "disable",
				Id = engineId,
			};

			TorBoxResponse disableResponse =
				await client.Main.User.ControlSearchEnginesAsync(disableRequest, cts.Token);

			Console.WriteLine($"  Result: {disableResponse.Detail ?? "Search engine disabled"}");

			// Delete a specific search engine.
			// This is a destructive action — requires confirmation.
			Console.WriteLine();
			Console.WriteLine($"Delete search engine {engineId}? This action is irreversible.");
			Console.Write("Type DELETE to confirm, or press Enter to skip: ");
			string? deleteConfirmation = Console.ReadLine();

			if (string.Equals(deleteConfirmation, "DELETE", StringComparison.Ordinal))
			{
				ControlSearchEnginesRequest deleteRequest = new()
				{
					Operation = "delete",
					Id = engineId,
				};

				TorBoxResponse deleteResponse =
					await client.Main.User.ControlSearchEnginesAsync(deleteRequest, cts.Token);

				Console.WriteLine($"  Result: {deleteResponse.Detail ?? "Search engine deleted"}");
			}
			else
			{
				Console.WriteLine("  Delete operation skipped.");
			}

			// ──────────────────────────────────────────────────────
			// Get a specific search engine by ID.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine($"Fetching search engine {engineId}...");

			TorBoxResponse<IReadOnlyList<SearchEngine>> singleResponse =
				await client.Main.User.GetSearchEnginesAsync(id: engineId, cancellationToken: cts.Token);

			if (singleResponse.Data is not null && singleResponse.Data.Count > 0)
			{
				SearchEngine engine = singleResponse.Data[0];
				Console.WriteLine($"  Name: {engine.Name ?? "N/A"}");
				Console.WriteLine($"  Enabled: {engine.Enabled}");
			}
		}
		catch (TorBoxException ex)
		{
			Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
		}

		Console.WriteLine();
		Console.WriteLine("Search engines example completed.");
	}
}
