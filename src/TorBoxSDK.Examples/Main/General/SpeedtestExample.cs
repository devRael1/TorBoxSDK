using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;

namespace TorBoxSDK.Examples.Main.General;

/// <summary>
/// Demonstrates how to use the speedtest files endpoint to measure download speeds.
/// </summary>
public static class SpeedtestExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("General — Speedtest Files");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Get speedtest files with default settings.
            // Returns URLs to test files for measuring download speed.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Getting speedtest files (default settings)...");

            TorBoxResponse<object> defaultResponse =
                await client.Main.General.GetSpeedtestFilesAsync(cancellationToken: cts.Token);

            Console.WriteLine($"  Result: {defaultResponse.Detail ?? "Speedtest files retrieved"}");

            // ──────────────────────────────────────────────────────
            // Get speedtest files with custom options.
            // You can specify region and test parameters.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Getting speedtest files (custom settings)...");

            SpeedtestOptions options = new()
            {
                Region = "us-east",   // Preferred server region
                TestLength = 5,       // Test duration in seconds
            };

            TorBoxResponse<object> customResponse =
                await client.Main.General.GetSpeedtestFilesAsync(options, cts.Token);

            Console.WriteLine($"  Result: {customResponse.Detail ?? "Custom speedtest files retrieved"}");

            // ──────────────────────────────────────────────────────
            // Get speedtest files with specific user IP.
            // Useful for testing from a specific network location.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Getting speedtest files (with user IP)...");

            SpeedtestOptions ipOptions = new()
            {
                UserIp = "203.0.113.1", // Documentation IP (RFC 5737) — replace with your actual public IP
            };

            TorBoxResponse<object> ipResponse =
                await client.Main.General.GetSpeedtestFilesAsync(ipOptions, cts.Token);

            Console.WriteLine($"  Result: {ipResponse.Detail ?? "IP-specific speedtest files retrieved"}");
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Speedtest example completed.");
    }
}
