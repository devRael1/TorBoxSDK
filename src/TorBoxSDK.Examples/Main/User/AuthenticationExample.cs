using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Examples.Main.User;

/// <summary>
/// Demonstrates authentication flows: refresh tokens, device authorization, and account management.
/// </summary>
public static class AuthenticationExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("User — Authentication & Account");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Refresh an expired session token.
            // ──────────────────────────────────────────────────────
            string sessionToken = "your-session-token"; // Replace with actual session token

            Console.WriteLine("Refreshing session token...");

            RefreshTokenRequest refreshRequest = new()
            {
                SessionToken = sessionToken,
            };

            TorBoxResponse<string> refreshResponse =
                await client.Main.User.RefreshTokenAsync(refreshRequest, cts.Token);

            Console.WriteLine($"  Result: {refreshResponse.Detail ?? "Token refreshed"}");
            Console.WriteLine($"  New token: {refreshResponse.Data ?? "N/A"}");

            // ──────────────────────────────────────────────────────
            // Get account confirmation status.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Checking account confirmation status...");

            TorBoxResponse<Confirmation> confirmResponse =
                await client.Main.User.GetConfirmationAsync(cts.Token);

            Console.WriteLine($"  Confirmation: {confirmResponse.Detail ?? "Confirmed"}");

            // ──────────────────────────────────────────────────────
            // Start device authorization flow.
            // This is used for TV/IoT devices that can't use browser auth.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Starting device authorization flow...");

            TorBoxResponse<DeviceCode> deviceCodeResponse =
                await client.Main.User.StartDeviceAuthAsync(app: "my-app", cancellationToken: cts.Token);

            if (deviceCodeResponse.Data is not null)
            {
                DeviceCode deviceCode = deviceCodeResponse.Data;
                Console.WriteLine($"  Code: {deviceCode.UserCode ?? "N/A"}");
                Console.WriteLine($"  Verification URL: {deviceCode.VerificationUrl ?? "N/A"}");
                Console.WriteLine($"  Friendly URL: {deviceCode.FriendlyVerificationUrl ?? "N/A"}");
                Console.WriteLine($"  Expires at: {deviceCode.ExpiresAt?.ToString("o") ?? "N/A"}");
                Console.WriteLine($"  Poll interval: {deviceCode.Interval} seconds");

                // ──────────────────────────────────────────────────
                // Exchange device code for a token.
                // In practice, you would poll this endpoint at the
                // specified interval until the user authorizes.
                // ──────────────────────────────────────────────────
                if (deviceCode.DeviceCodeValue is not null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Exchanging device code for token...");

                    DeviceTokenRequest tokenRequest = new()
                    {
                        DeviceCode = deviceCode.DeviceCodeValue,
                    };

                    TorBoxResponse<DeviceToken> tokenResponse =
                        await client.Main.User.GetDeviceTokenAsync(tokenRequest, cts.Token);

                    Console.WriteLine($"  Result: {tokenResponse.Detail ?? "Token received"}");
                }
            }

            // ──────────────────────────────────────────────────────
            // Delete account (DANGEROUS — requires confirmation).
            // Uncomment only if you truly want to delete the account.
            // ──────────────────────────────────────────────────────
            // Console.WriteLine();
            // Console.WriteLine("Deleting account...");
            //
            // DeleteAccountRequest deleteRequest = new()
            // {
            //     SessionToken = "your-session-token",
            //     ConfirmationCode = 123456,
            // };
            //
            // TorBoxResponse deleteResponse =
            //     await client.Main.User.DeleteMeAsync(deleteRequest, cts.Token);
            //
            // Console.WriteLine($"  Result: {deleteResponse.Detail ?? "Account deleted"}");
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Authentication example completed.");
    }
}
