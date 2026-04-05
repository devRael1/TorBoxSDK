using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Examples.User;

/// <summary>
/// Demonstrates how to retrieve the authenticated user's profile, subscriptions, and transactions.
/// </summary>
public static class GetProfileExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("User — Profile & Account Info");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Get the user's profile.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching user profile...");

            TorBoxResponse<UserProfile> profileResponse =
                await client.Main.User.GetMeAsync(settings: true, ct: cts.Token);

            if (profileResponse.Data is not null)
            {
                UserProfile profile = profileResponse.Data;

                Console.WriteLine($"  Email: {profile.Email ?? "N/A"}");
                Console.WriteLine($"  Plan: {profile.Plan}");
                Console.WriteLine($"  Subscribed: {profile.IsSubscribed}");
                Console.WriteLine($"  Premium expires: {profile.PremiumExpiresAt?.ToString("yyyy-MM-dd") ?? "N/A"}");
                Console.WriteLine($"  Total downloaded: {ExampleHelper.FormatBytes(profile.TotalDownloaded)}");
                Console.WriteLine($"  Account created: {profile.CreatedAt?.ToString("yyyy-MM-dd") ?? "N/A"}");
                Console.WriteLine($"  Referral code: {profile.UserReferralCode ?? "N/A"}");

                if (profile.Settings is not null)
                {
                    Console.WriteLine($"  Settings loaded: Yes");
                }
            }

            // ──────────────────────────────────────────────────────
            // Get subscriptions.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching subscriptions...");

            TorBoxResponse<IReadOnlyList<Subscription>> subscriptionsResponse =
                await client.Main.User.GetSubscriptionsAsync(cts.Token);

            if (subscriptionsResponse.Data is not null && subscriptionsResponse.Data.Count > 0)
            {
                Console.WriteLine($"  Found {subscriptionsResponse.Data.Count} subscription(s).");
            }
            else
            {
                Console.WriteLine("  No active subscriptions.");
            }

            // ──────────────────────────────────────────────────────
            // Get transaction history.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching transaction history...");

            TorBoxResponse<IReadOnlyList<Transaction>> transactionsResponse =
                await client.Main.User.GetTransactionsAsync(cts.Token);

            if (transactionsResponse.Data is not null && transactionsResponse.Data.Count > 0)
            {
                Console.WriteLine($"  Found {transactionsResponse.Data.Count} transaction(s).");
            }
            else
            {
                Console.WriteLine("  No transactions found.");
            }

            // ──────────────────────────────────────────────────────
            // Get referral data.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching referral data...");

            TorBoxResponse<ReferralData> referralResponse =
                await client.Main.User.GetReferralDataAsync(cts.Token);

            if (referralResponse.Data is not null)
            {
                Console.WriteLine($"  Referral data retrieved successfully.");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Get profile example completed.");
    }
}
