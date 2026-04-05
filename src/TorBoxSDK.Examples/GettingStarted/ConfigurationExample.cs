using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TorBoxSDK;
using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Examples.GettingStarted;

/// <summary>
/// Demonstrates configuring the TorBoxSDK from an appsettings.json file
/// using the <see cref="IConfiguration"/> binding overload.
/// </summary>
/// <remarks>
/// Expected appsettings.json format:
/// <code>
/// {
///   "TorBox": {
///     "ApiKey": "your-api-key-here",
///     "MainApiBaseUrl": "https://api.torbox.app/v1/api/",
///     "SearchApiBaseUrl": "https://search-api.torbox.app/",
///     "RelayApiBaseUrl": "https://relay.torbox.app/",
///     "Timeout": "00:00:30"
///   }
/// }
/// </code>
/// </remarks>
public static class ConfigurationExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Getting Started — Configuration from appsettings.json");

        // ──────────────────────────────────────────────────────────
        // Step 1: Build configuration from appsettings.json.
        //         In a real application, this is typically done
        //         by the host builder (e.g., WebApplication.CreateBuilder).
        // ──────────────────────────────────────────────────────────
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        // ──────────────────────────────────────────────────────────
        // Step 2: Register TorBox using the IConfiguration overload.
        //         This reads settings from the "TorBox" section.
        // ──────────────────────────────────────────────────────────
        ServiceCollection services = new();

        // Check if we have a TorBox section; if not, fall back to env var.
        IConfigurationSection torBoxSection = configuration.GetSection("TorBox");

        if (torBoxSection.Exists())
        {
            Console.WriteLine("Using configuration from appsettings.json...");
            services.AddTorBox(configuration);
        }
        else
        {
            Console.WriteLine("No appsettings.json found, falling back to environment variable...");
            services.AddTorBox(options =>
            {
                options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
                    ?? throw new InvalidOperationException(
                        "Set the TORBOX_API_KEY environment variable.");
            });
        }

        // ──────────────────────────────────────────────────────────
        // Step 3: Use the configured client.
        // ──────────────────────────────────────────────────────────
        using ServiceProvider provider = services.BuildServiceProvider();
        ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            TorBoxResponse<UserProfile> response = await client.Main.User.GetMeAsync(ct: cts.Token);

            if (response.Data is not null)
            {
                Console.WriteLine($"Successfully connected as: {response.Data.Email}");
                Console.WriteLine($"Account created: {response.Data.CreatedAt?.ToString("yyyy-MM-dd") ?? "N/A"}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Configuration example completed.");
    }
}
