using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Examples.Main.Torrents;

/// <summary>
/// Demonstrates how to edit torrents, get torrent info from files, and convert magnets to files.
/// </summary>
public static class EditTorrentExample
{
	public static async Task RunAsync()
	{
		ExampleHelper.PrintHeader("Torrents — Edit, Info by File & Magnet-to-File");

		ITorBoxClient client = ExampleHelper.CreateClient();
		using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

		try
		{
			// ──────────────────────────────────────────────────────
			// Edit torrent properties (rename, add tags).
			// ──────────────────────────────────────────────────────
			long torrentId = 12345; // Replace with your actual torrent ID

			Console.WriteLine($"Editing torrent {torrentId}...");

			EditTorrentRequest editRequest = new()
			{
				TorrentId = torrentId,
				Name = "My Renamed Torrent",
				Tags = ["linux", "iso", "2024"],
			};

			TorBoxResponse editResponse =
				await client.Main.Torrents.EditTorrentAsync(editRequest, cts.Token);

			Console.WriteLine($"  Result: {editResponse.Detail ?? "Torrent updated"}");

			// ──────────────────────────────────────────────────────
			// Get torrent info by providing a magnet link via POST.
			// This is an alternative to GetTorrentInfoAsync for more
			// complex requests or when you have a .torrent file.
			// ──────────────────────────────────────────────────────
			string magnetLink = "magnet:?xt=urn:btih:EXAMPLE_HASH"; // Replace with actual magnet

			Console.WriteLine();
			Console.WriteLine("Getting torrent info from magnet via POST...");

			TorrentInfoRequest infoRequest = new()
			{
				Magnet = magnetLink,
				Timeout = 10,
				UseCacheLookup = true,
			};

			TorBoxResponse<TorrentInfo> infoResponse =
				await client.Main.Torrents.GetTorrentInfoByFileAsync(infoRequest, cts.Token);

			if (infoResponse.Data is not null)
			{
				Console.WriteLine($"  Name: {infoResponse.Data.Name}");
				Console.WriteLine($"  Size: {ExampleHelper.FormatBytes(infoResponse.Data.Size)}");
			}

			// ──────────────────────────────────────────────────────
			// Convert a magnet link to a .torrent file.
			// Returns the file as a download URL or base64 data.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Converting magnet to .torrent file...");

			MagnetToFileRequest magnetToFileRequest = new()
			{
				Magnet = magnetLink,
			};

			TorBoxResponse<string> fileResponse =
				await client.Main.Torrents.MagnetToFileAsync(magnetToFileRequest, cts.Token);

			if (fileResponse.Data is not null)
			{
				Console.WriteLine($"  File data length: {fileResponse.Data.Length} chars");
			}
		}
		catch (TorBoxException ex)
		{
			Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
		}

		Console.WriteLine();
		Console.WriteLine("Edit torrent example completed.");
	}
}
