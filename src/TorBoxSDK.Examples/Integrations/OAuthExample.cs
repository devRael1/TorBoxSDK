using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;

namespace TorBoxSDK.Examples.Integrations;

/// <summary>
/// Demonstrates OAuth integration management: redirect, register, unregister,
/// and linked Discord roles.
/// </summary>
public static class OAuthExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Integrations — OAuth & Discord");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // List connected OAuth integrations.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching connected OAuth integrations...");

            TorBoxResponse<IReadOnlyList<OAuthIntegration>> oauthResponse =
                await client.Main.Integrations.GetOAuthMeAsync(cts.Token);

            if (oauthResponse.Data is not null && oauthResponse.Data.Count > 0)
            {
                Console.WriteLine($"  Connected: {oauthResponse.Data.Count} integration(s)");

                foreach (OAuthIntegration integration in oauthResponse.Data)
                {
                    Console.WriteLine($"    - {integration}");
                }
            }
            else
            {
                Console.WriteLine("  No integrations connected.");
            }

            // ──────────────────────────────────────────────────────
            // Get OAuth redirect URL for a provider.
            // User must visit this URL to authorize the integration.
            // Supported: "google", "dropbox", "onedrive", "discord"
            // ──────────────────────────────────────────────────────
            string provider = "google"; // Replace with desired provider

            Console.WriteLine();
            Console.WriteLine($"Getting OAuth redirect URL for '{provider}'...");

            TorBoxResponse<string> redirectResponse =
                await client.Main.Integrations.OAuthRedirectAsync(provider, cts.Token);

            if (redirectResponse.Data is not null)
            {
                Console.WriteLine($"  Redirect URL: {redirectResponse.Data}");
                Console.WriteLine("  Open this URL in a browser to authorize the integration.");
            }

            // ──────────────────────────────────────────────────────
            // Handle OAuth callback (typically done by the redirect
            // handler after the user authorizes).
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Handling OAuth callback for '{provider}'...");

            TorBoxResponse<object> callbackResponse =
                await client.Main.Integrations.OAuthCallbackAsync(provider, cts.Token);

            Console.WriteLine($"  Callback result: {callbackResponse.Detail ?? "Processed"}");

            // ──────────────────────────────────────────────────────
            // Handle OAuth success confirmation.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Handling OAuth success for '{provider}'...");

            TorBoxResponse<object> successResponse =
                await client.Main.Integrations.OAuthSuccessAsync(provider, cts.Token);

            Console.WriteLine($"  Success result: {successResponse.Detail ?? "Confirmed"}");

            // ──────────────────────────────────────────────────────
            // Register an OAuth integration with authorization code.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Registering OAuth for '{provider}'...");

            OAuthRegisterRequest registerRequest = new()
            {
                Provider = provider,
                Code = "authorization-code",                   // Replace with actual OAuth code
                RedirectUri = "https://your-app.com/callback", // Replace with your redirect URI
            };

            TorBoxResponse registerResponse =
                await client.Main.Integrations.OAuthRegisterAsync(provider, registerRequest, cts.Token);

            Console.WriteLine($"  Result: {registerResponse.Detail ?? "Registered"}");

            // ──────────────────────────────────────────────────────
            // Unregister an OAuth integration.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine($"Unregistering OAuth for '{provider}'...");

            TorBoxResponse unregisterResponse =
                await client.Main.Integrations.OAuthUnregisterAsync(provider, cts.Token);

            Console.WriteLine($"  Result: {unregisterResponse.Detail ?? "Unregistered"}");

            // ──────────────────────────────────────────────────────
            // Get linked Discord roles.
            // Shows roles linked to the TorBox account via Discord.
            // ──────────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("Fetching linked Discord roles...");

            TorBoxResponse<object> discordResponse =
                await client.Main.Integrations.GetLinkedDiscordRolesAsync(cts.Token);

            Console.WriteLine($"  Discord roles: {discordResponse.Detail ?? "Retrieved"}");
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("OAuth integration example completed.");
    }
}
