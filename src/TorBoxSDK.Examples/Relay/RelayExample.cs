using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Relay;

namespace TorBoxSDK.Examples.Relay;

/// <summary>
/// Demonstrates the Relay API: check relay status and verify torrent inactivity.
/// The Relay API is used for relay server management and monitoring.
/// </summary>
public static class RelayExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Relay — Status & Inactivity Check");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Get relay server status.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Checking relay server status...");

            TorBoxResponse<RelayStatus> statusResponse =
                await client.Relay.GetStatusAsync(cts.Token);

            if (statusResponse.Data is not null)
            {
                Console.WriteLine($"  Relay status: {statusResponse.Data.Status ?? "N/A"}");
            }
            else
            {
                Console.WriteLine($"  Status: {statusResponse.Detail ?? "Retrieved"}");
            }

            // ──────────────────────────────────────────────────────
            // Check if a torrent is inactive on the relay.
            // This is used by relay servers to determine if a torrent
            // should continue to be seeded.
            // ──────────────────────────────────────────────────────
            string authId = "user-auth-id"; // Replace with actual user auth ID
            long torrentId = 12345;         // Replace with actual torrent ID

            Console.WriteLine();
            Console.WriteLine($"Checking inactivity for torrent {torrentId} (auth: {authId})...");

            TorBoxResponse<InactiveCheckResult> inactiveResponse =
                await client.Relay.CheckForInactiveAsync(authId, torrentId, cts.Token);

            if (inactiveResponse.Data is not null)
            {
                InactiveCheckResult result = inactiveResponse.Data;
                Console.WriteLine($"  Is inactive: {result.IsInactive}");
                Console.WriteLine($"  Status: {result.Status ?? "N/A"}");
                Console.WriteLine($"  Last active: {result.LastActive?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Relay example completed.");
    }
}
