using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;

namespace TorBoxSDK.Examples.Main.Notifications;

/// <summary>
/// Demonstrates how to manage notifications: list, clear, and send test notifications.
/// </summary>
public static class NotificationsExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Notifications — Manage Notifications");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Get all notifications.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching notifications...");

            TorBoxResponse<IReadOnlyList<Notification>> notificationsResponse =
                await client.Main.Notifications.GetMyNotificationsAsync(cts.Token);

            if (notificationsResponse.Data is null || notificationsResponse.Data.Count == 0)
            {
                Console.WriteLine("No notifications found.");
            }
            else
            {
                Console.WriteLine($"Found {notificationsResponse.Data.Count} notification(s):");
                Console.WriteLine();

                foreach (Notification notification in notificationsResponse.Data)
                {
                    string readIcon = notification.Read ? "✓" : "•";
                    Console.WriteLine($"  [{readIcon}] {notification.Title}");
                    Console.WriteLine($"      {notification.Message}");
                    Console.WriteLine($"      Date: {notification.CreatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}");
                    Console.WriteLine();
                }
            }

            // ──────────────────────────────────────────────────────
            // Send a test notification (useful for verifying setup).
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Sending test notification...");

            TorBoxResponse testResponse =
                await client.Main.Notifications.SendTestNotificationAsync(cts.Token);

            Console.WriteLine($"  Result: {testResponse.Detail ?? "Test notification sent"}");

            // ──────────────────────────────────────────────────────
            // Clear a specific notification.
            // ──────────────────────────────────────────────────────
            long notificationId = 12345; // Replace with an actual notification ID

            Console.WriteLine($"Clearing notification {notificationId}...");

            TorBoxResponse clearResponse =
                await client.Main.Notifications.ClearNotificationAsync(notificationId, cts.Token);

            Console.WriteLine($"  Result: {clearResponse.Detail ?? "Notification cleared"}");

            // ──────────────────────────────────────────────────────
            // Clear all notifications.
            // This is a destructive action — requires confirmation.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Clear all notifications is a destructive action.");
            Console.Write("Type YES to clear all notifications, or press Enter to skip: ");
            string? clearAllConfirmation = Console.ReadLine();

            if (string.Equals(clearAllConfirmation, "YES", StringComparison.Ordinal))
            {
                Console.WriteLine("Clearing all notifications...");

                TorBoxResponse clearAllResponse =
                    await client.Main.Notifications.ClearAllNotificationsAsync(cts.Token);

                Console.WriteLine($"  Result: {clearAllResponse.Detail ?? "All notifications cleared"}");
            }
            else
            {
                Console.WriteLine("Skipped clearing all notifications.");
            }

            // ──────────────────────────────────────────────────────
            // Get notification RSS feed URL.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching notification RSS feed...");

            TorBoxResponse<string> rssResponse =
                await client.Main.Notifications.GetNotificationRssAsync(cts.Token);

            if (rssResponse.Data is not null)
            {
                Console.WriteLine($"  RSS Feed URL: {rssResponse.Data}");
            }

            // ──────────────────────────────────────────────────────
            // Get Intercom identity verification hash.
            // Used for secure Intercom integration.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching Intercom identity hash...");

            TorBoxResponse<IntercomHash> intercomResponse =
                await client.Main.Notifications.GetIntercomHashAsync(cts.Token);

            if (intercomResponse.Data is not null)
            {
                Console.WriteLine($"  Intercom hash: {intercomResponse.Data.Hash ?? "N/A"}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Notifications example completed.");
    }
}
