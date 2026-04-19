using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;

namespace TorBoxSDK.Examples.Search;

/// <summary>
/// Demonstrates how to access search API tutorials for torrents, Usenet, and meta search.
/// Tutorials provide guidance on available search options and syntax.
/// </summary>
public static class SearchTutorialsExample
{
	public static async Task RunAsync()
	{
		ExampleHelper.PrintHeader("Search — API Tutorials");

		ITorBoxClient client = ExampleHelper.CreateClient();
		using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

		try
		{
			// ──────────────────────────────────────────────────────
			// Get the torrent search tutorial.
			// Explains available search parameters and syntax.
			// ──────────────────────────────────────────────────────
			Console.WriteLine("Fetching torrent search tutorial...");

			TorBoxResponse<string> torrentTutorial =
				await client.Search.GetTorrentSearchTutorialAsync(cts.Token);

			if (torrentTutorial.Data is not null)
			{
				Console.WriteLine($"  Tutorial length: {torrentTutorial.Data.Length} chars");
				Console.WriteLine($"  Preview: {torrentTutorial.Data[..Math.Min(200, torrentTutorial.Data.Length)]}...");
			}

			// ──────────────────────────────────────────────────────
			// Get the Usenet search tutorial.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Fetching Usenet search tutorial...");

			TorBoxResponse<string> usenetTutorial =
				await client.Search.GetUsenetSearchTutorialAsync(cts.Token);

			if (usenetTutorial.Data is not null)
			{
				Console.WriteLine($"  Tutorial length: {usenetTutorial.Data.Length} chars");
				Console.WriteLine($"  Preview: {usenetTutorial.Data[..Math.Min(200, usenetTutorial.Data.Length)]}...");
			}

			// ──────────────────────────────────────────────────────
			// Get the meta search tutorial.
			// Meta search covers movies, TV shows, and other media.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Fetching meta search tutorial...");

			TorBoxResponse<string> metaTutorial =
				await client.Search.GetMetaSearchTutorialAsync(cts.Token);

			if (metaTutorial.Data is not null)
			{
				Console.WriteLine($"  Tutorial length: {metaTutorial.Data.Length} chars");
				Console.WriteLine($"  Preview: {metaTutorial.Data[..Math.Min(200, metaTutorial.Data.Length)]}...");
			}
		}
		catch (TorBoxException ex)
		{
			Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
		}

		Console.WriteLine();
		Console.WriteLine("Search tutorials example completed.");
	}
}
