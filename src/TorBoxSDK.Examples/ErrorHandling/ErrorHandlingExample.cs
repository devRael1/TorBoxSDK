using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Torrents;

namespace TorBoxSDK.Examples.ErrorHandling;

/// <summary>
/// Demonstrates comprehensive error handling patterns when using the TorBoxSDK.
/// Shows how to handle API errors, network errors, timeouts, and cancellations.
/// </summary>
public static class ErrorHandlingExample
{
	public static async Task RunAsync()
	{
		ExampleHelper.PrintHeader("Error Handling — Comprehensive Patterns");

		ITorBoxClient client = ExampleHelper.CreateClient();

		// ──────────────────────────────────────────────────────
		// Pattern 1: Handle TorBoxException (API-level errors).
		//            The TorBox API returns structured errors with
		//            error codes and detail messages.
		// ──────────────────────────────────────────────────────
		Console.WriteLine("Pattern 1: TorBoxException handling");

		using CancellationTokenSource cts1 = ExampleHelper.CreateTimeout();

		try
		{
			TorBoxResponse<IReadOnlyList<Torrent>> response =
				await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cts1.Token);

			Console.WriteLine($"  Success: {response.Data?.Count ?? 0} torrents retrieved.");
		}
		catch (TorBoxException ex)
		{
			// TorBoxException provides structured error details from the API.
			Console.Error.WriteLine($"  TorBox API error:");
			Console.Error.WriteLine($"    Error code: {ex.ErrorCode}");
			Console.Error.WriteLine($"    Detail: {ex.Detail ?? "No additional detail"}");
			Console.Error.WriteLine($"    Message: {ex.Message}");

			// Handle specific error codes differently.
			switch (ex.ErrorCode)
			{
				case TorBoxErrorCode.BadToken:
				case TorBoxErrorCode.NoAuth:
					Console.Error.WriteLine("    → Your API key is invalid or missing.");
					break;
				case TorBoxErrorCode.TooManyRequests:
					Console.Error.WriteLine("    → Rate limited. Please wait before retrying.");
					break;
				case TorBoxErrorCode.PlanRestrictedFeature:
					Console.Error.WriteLine("    → This feature requires a higher plan.");
					break;
				case TorBoxErrorCode.ItemNotFound:
					Console.Error.WriteLine("    → The requested resource was not found.");
					break;
				default:
					Console.Error.WriteLine($"    → Unhandled error code: {ex.ErrorCode}");
					break;
			}
		}

		// ──────────────────────────────────────────────────────
		// Pattern 2: Handle network/transport errors.
		//            These occur when the HTTP request itself fails
		//            (DNS resolution, connection refused, etc.).
		// ──────────────────────────────────────────────────────
		Console.WriteLine();
		Console.WriteLine("Pattern 2: Network error handling");

		using CancellationTokenSource cts2 = ExampleHelper.CreateTimeout();

		try
		{
			TorBoxResponse<IReadOnlyList<Torrent>> response =
				await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cts2.Token);

			Console.WriteLine($"  Success: retrieved {response.Data?.Count ?? 0} torrents.");
		}
		catch (TorBoxException ex)
		{
			Console.Error.WriteLine($"  API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
		}
		catch (HttpRequestException ex)
		{
			// Network-level errors (DNS, connection, TLS, etc.).
			Console.Error.WriteLine($"  Network error: {ex.Message}");
			Console.Error.WriteLine("  → Check your internet connection and firewall settings.");
		}

		// ──────────────────────────────────────────────────────
		// Pattern 3: Handle timeouts and cancellations.
		//            Always use a CancellationToken to avoid
		//            hanging requests.
		// ──────────────────────────────────────────────────────
		Console.WriteLine();
		Console.WriteLine("Pattern 3: Timeout and cancellation handling");

		// Short timeout to demonstrate timeout behavior.
		using CancellationTokenSource shortCts = new(TimeSpan.FromMilliseconds(1));

		try
		{
			// This will likely time out with the very short timeout.
			TorBoxResponse<IReadOnlyList<Torrent>> response =
				await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: shortCts.Token);

			Console.WriteLine("  (Request completed before timeout)");
		}
		catch (TorBoxException ex)
		{
			Console.Error.WriteLine($"  API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
		}
		catch (TaskCanceledException) when (shortCts.IsCancellationRequested)
		{
			Console.Error.WriteLine("  Request timed out.");
			Console.Error.WriteLine("  → Consider increasing the timeout duration.");
		}
		catch (OperationCanceledException)
		{
			Console.Error.WriteLine("  Request was canceled.");
		}

		// ──────────────────────────────────────────────────────
		// Pattern 4: Full error handling template.
		//            Combine all patterns for production code.
		// ──────────────────────────────────────────────────────
		Console.WriteLine();
		Console.WriteLine("Pattern 4: Complete error handling template");

		using CancellationTokenSource cts4 = ExampleHelper.CreateTimeout();

		try
		{
			TorBoxResponse<IReadOnlyList<Torrent>> response =
				await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cts4.Token);

			if (response.Data is not null)
			{
				Console.WriteLine($"  Success: {response.Data.Count} torrents.");
			}
		}
		catch (TorBoxException ex)
		{
			Console.Error.WriteLine($"  API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
		}
		catch (HttpRequestException ex)
		{
			Console.Error.WriteLine($"  Network error: {ex.Message}");
		}
		catch (TaskCanceledException) when (cts4.IsCancellationRequested)
		{
			Console.Error.WriteLine("  Request timed out or was canceled.");
		}

		Console.WriteLine();
		Console.WriteLine("Error handling example completed.");
	}
}
