using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Examples.Main.User;

/// <summary>
/// Demonstrates how to manage user settings and search engines.
/// </summary>
public static class ManageSettingsExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("User — Manage Settings");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Edit user settings.
            // Only include the settings you want to change.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Updating user settings...");

            EditSettingsRequest settingsRequest = new()
            {
                EnableNotifications = true,
                EmailNotifications = true,
                WebNotifications = true,
                SaveMagnetHistory = true,
            };

            TorBoxResponse settingsResponse =
                await client.Main.User.EditSettingsAsync(settingsRequest, cts.Token);

            Console.WriteLine($"  Settings updated: {settingsResponse.Detail ?? "Success"}");

            // ──────────────────────────────────────────────────────
            // List configured search engines.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching search engines...");

            TorBoxResponse<IReadOnlyList<SearchEngine>> enginesResponse =
                await client.Main.User.GetSearchEnginesAsync(ct: cts.Token);

            if (enginesResponse.Data is not null && enginesResponse.Data.Count > 0)
            {
                Console.WriteLine($"  Found {enginesResponse.Data.Count} search engine(s).");
            }
            else
            {
                Console.WriteLine("  No custom search engines configured.");
            }

            // ──────────────────────────────────────────────────────
            // Add a referral code.
            // ──────────────────────────────────────────────────────
            string referralCode = "EXAMPLE_REFERRAL"; // Replace with an actual referral code

            Console.WriteLine($"Adding referral code: {referralCode}...");

            TorBoxResponse referralResponse =
                await client.Main.User.AddReferralAsync(referralCode, cts.Token);

            Console.WriteLine($"  Result: {referralResponse.Detail ?? "Success"}");
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Manage settings example completed.");
    }
}
