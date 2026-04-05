using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Vendors;

namespace TorBoxSDK.Examples.Main.Vendors;

/// <summary>
/// Demonstrates the Vendors API: register, manage accounts, and handle users.
/// The Vendors API is designed for third-party service providers who integrate with TorBox.
/// </summary>
public static class VendorExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Vendors — Vendor Account Management");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Register a new vendor account.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Registering a vendor account...");

            RegisterVendorRequest registerRequest = new()
            {
                VendorName = "My Service",
                VendorUrl = "https://my-service.example.com",
            };

            TorBoxResponse<VendorAccount> registerResponse =
                await client.Main.Vendors.RegisterAsync(registerRequest, cts.Token);

            if (registerResponse.Data is not null)
            {
                Console.WriteLine($"  Vendor registered - ID: {registerResponse.Data.Id}");
                Console.WriteLine($"  Name: {registerResponse.Data.VendorName ?? "N/A"}");
                Console.WriteLine("  API Key: [omitted from output for security]");
            }

            // ──────────────────────────────────────────────────────
            // Get your vendor account details.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching vendor account...");

            TorBoxResponse<VendorAccount> accountResponse =
                await client.Main.Vendors.GetAccountAsync(cts.Token);

            if (accountResponse.Data is not null)
            {
                Console.WriteLine($"  Vendor: {accountResponse.Data.VendorName ?? "N/A"}");
                Console.WriteLine($"  URL: {accountResponse.Data.VendorUrl ?? "N/A"}");
                Console.WriteLine($"  Created: {accountResponse.Data.CreatedAt?.ToString("yyyy-MM-dd") ?? "N/A"}");
            }

            // ──────────────────────────────────────────────────────
            // Update vendor account details.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Updating vendor account...");

            UpdateVendorAccountRequest updateRequest = new()
            {
                VendorName = "My Updated Service",
                VendorUrl = "https://updated-service.example.com",
            };

            TorBoxResponse<VendorAccount> updateResponse =
                await client.Main.Vendors.UpdateAccountAsync(updateRequest, cts.Token);

            if (updateResponse.Data is not null)
            {
                Console.WriteLine($"  Updated name: {updateResponse.Data.VendorName ?? "N/A"}");
            }

            // ──────────────────────────────────────────────────────
            // List all vendor-managed accounts.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Listing vendor-managed accounts...");

            TorBoxResponse<IReadOnlyList<VendorAccount>> accountsResponse =
                await client.Main.Vendors.GetAccountsAsync(cts.Token);

            if (accountsResponse.Data is not null && accountsResponse.Data.Count > 0)
            {
                Console.WriteLine($"  Found {accountsResponse.Data.Count} account(s):");

                foreach (VendorAccount account in accountsResponse.Data)
                {
                    Console.WriteLine($"    [{account.Id}] {account.VendorName ?? "N/A"}");
                }
            }
            else
            {
                Console.WriteLine("  No managed accounts found.");
            }

            // ──────────────────────────────────────────────────────
            // Get a specific account by authentication ID.
            // ──────────────────────────────────────────────────────
            string userAuthId = "user-auth-id-123"; // Replace with actual auth ID

            Console.WriteLine();
            Console.WriteLine($"Getting account by auth ID: {userAuthId}...");

            TorBoxResponse<VendorAccount> authResponse =
                await client.Main.Vendors.GetAccountByAuthIdAsync(userAuthId, cts.Token);

            if (authResponse.Data is not null)
            {
                Console.WriteLine($"  Found: {authResponse.Data.VendorName ?? "N/A"}");
            }

            // ──────────────────────────────────────────────────────
            // Register a user under the vendor.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Registering a user under vendor...");

            RegisterVendorUserRequest userRequest = new()
            {
                UserEmail = "user@example.com", // Replace with actual user email
            };

            TorBoxResponse registerUserResponse =
                await client.Main.Vendors.RegisterUserAsync(userRequest, cts.Token);

            Console.WriteLine($"  Result: {registerUserResponse.Detail ?? "User registered"}");

            // ──────────────────────────────────────────────────────
            // Remove a user from the vendor.
            // This is a destructive action — requires confirmation.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Remove a user from vendor...");
            Console.Write("Type 'yes' to confirm user removal, or press Enter to skip: ");
            string? removeConfirmation = Console.ReadLine();

            if (string.Equals(removeConfirmation, "yes", StringComparison.OrdinalIgnoreCase))
            {
                RemoveVendorUserRequest removeRequest = new()
                {
                    UserEmail = "user@example.com", // Replace with actual user email
                };

                TorBoxResponse removeResponse =
                    await client.Main.Vendors.RemoveUserAsync(removeRequest, cts.Token);

                Console.WriteLine($"  Result: {removeResponse.Detail ?? "User removed"}");
            }
            else
            {
                Console.WriteLine("  User removal skipped.");
            }

            // ──────────────────────────────────────────────────────
            // Refresh vendor credentials.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Refreshing vendor credentials...");

            TorBoxResponse<VendorAccount> refreshResponse =
                await client.Main.Vendors.RefreshAsync(cts.Token);

            if (refreshResponse.Data is not null)
            {
                Console.WriteLine("  Refreshed API Key: [omitted from output for security]");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Vendor management example completed.");
    }
}
