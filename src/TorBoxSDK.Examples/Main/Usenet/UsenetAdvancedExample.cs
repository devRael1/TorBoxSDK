using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Usenet;

namespace TorBoxSDK.Examples.Main.Usenet;

/// <summary>
/// Demonstrates advanced Usenet operations: cache checks, editing, and async creation.
/// </summary>
public static class UsenetAdvancedExample
{
	public static async Task RunAsync()
	{
		ExampleHelper.PrintHeader("Usenet — Cache Check, Edit & Async Create");

		ITorBoxClient client = ExampleHelper.CreateClient();
		using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

		try
		{
			// ──────────────────────────────────────────────────────
			// Check if Usenet NZBs are cached (GET method).
			// ──────────────────────────────────────────────────────
			IReadOnlyList<string> hashes = ["EXAMPLE_HASH_1", "EXAMPLE_HASH_2"]; // Replace with actual NZB hashes

			Console.WriteLine($"Checking cache for {hashes.Count} Usenet hash(es) (GET)...");

			TorBoxResponse<CheckUsenetCached> cacheResponse =
				await client.Main.Usenet.CheckCachedAsync(hashes, new CheckCachedOptions { Format = "object", ListFiles = true }, cancellationToken: cts.Token);

			Console.WriteLine($"  Cache check result: {cacheResponse.Detail ?? "Completed"}");

			// ──────────────────────────────────────────────────────
			// Check cache via POST (for large hash lists).
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Checking cache via POST...");

			CheckUsenetCachedRequest cacheRequest = new()
			{
				Hashes = ["EXAMPLE_HASH_1", "EXAMPLE_HASH_2"],
				Format = "object",
				ListFiles = true,
			};

			TorBoxResponse<CheckUsenetCached> cachePostResponse =
				await client.Main.Usenet.CheckCachedByPostAsync(cacheRequest, cts.Token);

			Console.WriteLine($"  POST cache check result: {cachePostResponse.Detail ?? "Completed"}");

			// ──────────────────────────────────────────────────────
			// Edit a Usenet download (rename, add tags).
			// ──────────────────────────────────────────────────────
			long usenetId = 12345; // Replace with your actual Usenet download ID

			Console.WriteLine();
			Console.WriteLine($"Editing Usenet download {usenetId}...");

			EditUsenetDownloadRequest editRequest = new()
			{
				UsenetDownloadId = usenetId,
				Name = "My Renamed Usenet Download",
				Tags = ["nzb", "linux"],
			};

			TorBoxResponse editResponse =
				await client.Main.Usenet.EditUsenetDownloadAsync(editRequest, cts.Token);

			Console.WriteLine($"  Result: {editResponse.Detail ?? "Download updated"}");

			// ──────────────────────────────────────────────────────
			// Create a Usenet download asynchronously.
			// Returns immediately while the server processes it.
			// ──────────────────────────────────────────────────────
			string nzbLink = "https://example.com/file.nzb"; // Replace with actual NZB link

			Console.WriteLine();
			Console.WriteLine("Creating Usenet download (async)...");

			CreateUsenetDownloadRequest asyncRequest = new()
			{
				Link = nzbLink,
				Name = "My Async Usenet Download",
			};

			TorBoxResponse<UsenetDownload> asyncResponse =
				await client.Main.Usenet.AsyncCreateUsenetDownloadAsync(asyncRequest, cts.Token);

			if (asyncResponse.Data is not null)
			{
				Console.WriteLine($"  Async download created - ID: {asyncResponse.Data.Id}");
				Console.WriteLine($"  Name: {asyncResponse.Data.Name ?? "N/A"}");
			}
		}
		catch (TorBoxException ex)
		{
			Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
		}

		Console.WriteLine();
		Console.WriteLine("Usenet advanced example completed.");
	}
}
