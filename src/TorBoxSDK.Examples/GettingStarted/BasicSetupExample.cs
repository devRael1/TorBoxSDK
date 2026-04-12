using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Examples.GettingStarted;

/// <summary>
/// Demonstrates the basic setup of the TorBoxSDK using dependency injection.
/// This is the recommended way to configure and use the SDK.
/// </summary>
public static class BasicSetupExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Getting Started — Basic DI Setup");

        // ──────────────────────────────────────────────────────────
        // Step 1: Configure services using the standard DI pattern.
        //         AddTorBox() registers all API clients, handlers,
        //         and options needed by the SDK.
        // ──────────────────────────────────────────────────────────
        var services = new ServiceCollection();

        services.AddTorBox(options =>
        {
            // Always read API keys from environment variables — never hardcode them.
            options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
                ?? throw new InvalidOperationException(
                    "Set the TORBOX_API_KEY environment variable.");

            // Optional: override default timeouts and base URLs.
            // options.Timeout = TimeSpan.FromSeconds(60);
            // options.MainApiBaseUrl = "https://api.torbox.app/v1/api/";
        });

        // ──────────────────────────────────────────────────────────
        // Step 2: Build the service provider and resolve the client.
        // ──────────────────────────────────────────────────────────
        using ServiceProvider provider = services.BuildServiceProvider();

        ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();

        // ──────────────────────────────────────────────────────────
        // Step 3: Use the client with a CancellationToken.
        //         The client exposes three API families:
        //         - client.Main   (torrents, usenet, user, etc.)
        //         - client.Search (torrent/usenet/meta search)
        //         - client.Relay  (relay status checks)
        // ──────────────────────────────────────────────────────────
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            TorBoxResponse<UserProfile> response =
                await client.Main.User.GetMeAsync(cancellationToken: cts.Token);

            if (response.Data is not null)
            {
                Console.WriteLine($"Authenticated as: {response.Data.Email}");
                Console.WriteLine($"Plan: {response.Data.Plan}");
                Console.WriteLine($"Premium expires: {response.Data.PremiumExpiresAt?.ToString("yyyy-MM-dd") ?? "N/A"}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Basic setup example completed.");
    }
}
