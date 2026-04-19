using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Examples.GettingStarted;

/// <summary>
/// Demonstrates standalone TorBoxClient usage without dependency injection.
/// Shows all three constructor patterns and IDisposable usage.
/// </summary>
public static class StandaloneSetupExample
{
	public static async Task RunAsync()
	{
		ExampleHelper.PrintHeader("Getting Started — Standalone Setup (No DI)");

		// Always read API keys from environment variables — never hardcode them.
		string apiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
			?? throw new InvalidOperationException(
				"Set the TORBOX_API_KEY environment variable.");

		// ──────────────────────────────────────────────────────────
		// Pattern 1: Simple constructor with API key only.
		//            Uses default options (30s timeout, standard URLs).
		// ──────────────────────────────────────────────────────────
		Console.WriteLine("── Pattern 1: Simple API key constructor ──");

		using (TorBoxClient client = new(apiKey))
		{
			using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

			try
			{
				TorBoxResponse<UserProfile> response =
					await client.Main.User.GetMeAsync(cancellationToken: cts.Token);

				if (response.Data is not null)
				{
					Console.WriteLine($"  Authenticated as: {response.Data.Email}");
					Console.WriteLine($"  Plan: {response.Data.Plan}");
				}
			}
			catch (TorBoxException ex)
			{
				Console.Error.WriteLine($"  API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
			}
		}

		Console.WriteLine();

		// ──────────────────────────────────────────────────────────
		// Pattern 2: Options object for full control.
		// ──────────────────────────────────────────────────────────
		Console.WriteLine("── Pattern 2: Options object constructor ──");

		using (TorBoxClient client = new(new TorBoxClientOptions
		{
			ApiKey = apiKey,
			Timeout = TimeSpan.FromSeconds(60)
		}))
		{
			using CancellationTokenSource cts = ExampleHelper.CreateTimeout(timeoutSeconds: 60);

			try
			{
				TorBoxResponse<UserProfile> response =
					await client.Main.User.GetMeAsync(cancellationToken: cts.Token);

				if (response.Data is not null)
				{
					Console.WriteLine($"  Authenticated as: {response.Data.Email}");
				}
			}
			catch (TorBoxException ex)
			{
				Console.Error.WriteLine($"  API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
			}
		}

		Console.WriteLine();

		// ──────────────────────────────────────────────────────────
		// Pattern 3: Configuration delegate (builder pattern).
		// ──────────────────────────────────────────────────────────
		Console.WriteLine("── Pattern 3: Configuration delegate ──");

		using (TorBoxClient client = new(options =>
		{
			options.ApiKey = apiKey;
			options.Timeout = TimeSpan.FromSeconds(45);
		}))
		{
			using CancellationTokenSource cts = ExampleHelper.CreateTimeout(timeoutSeconds: 45);

			try
			{
				TorBoxResponse<UserProfile> response =
					await client.Main.User.GetMeAsync(cancellationToken: cts.Token);

				if (response.Data is not null)
				{
					Console.WriteLine($"  Authenticated as: {response.Data.Email}");
				}
			}
			catch (TorBoxException ex)
			{
				Console.Error.WriteLine($"  API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
			}
		}

		Console.WriteLine();
		Console.WriteLine("Standalone setup example completed.");
		Console.WriteLine("Note: TorBoxClient implements IDisposable — always use 'using' statements.");
	}
}
