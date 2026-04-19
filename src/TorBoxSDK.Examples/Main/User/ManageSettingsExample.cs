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
			// Edit notification settings.
			// Only include the settings you want to change.
			// ──────────────────────────────────────────────────────
			Console.WriteLine("Updating notification settings...");

			EditSettingsRequest notificationSettings = new()
			{
				EmailNotifications = true,
				WebNotifications = true,
				DiscordNotifications = false,
				TelegramNotifications = false,
			};

			TorBoxResponse notificationResponse =
				await client.Main.User.EditSettingsAsync(notificationSettings, cts.Token);

			Console.WriteLine($"  Notification settings updated: {notificationResponse.Detail ?? "Success"}");

			// ──────────────────────────────────────────────────────
			// Edit download and UI settings.
			// These include recently added properties such as
			// DashboardSort and AppendFilenameToLinks.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Updating download and UI settings...");

			EditSettingsRequest downloadSettings = new()
			{
				AllowZipped = true,
				AppendFilenameToLinks = true,  // Append the filename to generated download links
				DashboardSort = "created_at",  // Sort dashboard items by creation date
				DownloadSpeedInTab = true,     // Show download speed in browser tab title
				ShowTrackerInTorrents = false,
			};

			TorBoxResponse downloadResponse =
				await client.Main.User.EditSettingsAsync(downloadSettings, cts.Token);

			Console.WriteLine($"  Download/UI settings updated: {downloadResponse.Detail ?? "Success"}");

			// ──────────────────────────────────────────────────────
			// Edit web player settings.
			// Configure transcoding, language preferences, and playback.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Updating web player settings...");

			EditSettingsRequest webPlayerSettings = new()
			{
				WebPlayerAlwaysTranscode = false,
				WebPlayerAlwaysSkipIntro = true,
				WebPlayerAudioPreferredLanguage = "en",   // Preferred audio language
				WebPlayerSubtitlePreferredLanguage = "en", // Preferred subtitle language
				WebPlayerEnableScrobbling = true,
			};

			TorBoxResponse webPlayerResponse =
				await client.Main.User.EditSettingsAsync(webPlayerSettings, cts.Token);

			Console.WriteLine($"  Web player settings updated: {webPlayerResponse.Detail ?? "Success"}");

			// ──────────────────────────────────────────────────────
			// Edit WebDAV settings.
			// Configure WebDAV file access behavior.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Updating WebDAV settings...");

			EditSettingsRequest webdavSettings = new()
			{
				WebdavUseLocalFiles = false,
				WebdavUseFolderView = true,
				WebdavFlatten = false,
			};

			TorBoxResponse webdavResponse =
				await client.Main.User.EditSettingsAsync(webdavSettings, cts.Token);

			Console.WriteLine($"  WebDAV settings updated: {webdavResponse.Detail ?? "Success"}");

			// ──────────────────────────────────────────────────────
			// List configured search engines.
			// ──────────────────────────────────────────────────────
			Console.WriteLine();
			Console.WriteLine("Fetching search engines...");

			TorBoxResponse<IReadOnlyList<SearchEngine>> enginesResponse =
				await client.Main.User.GetSearchEnginesAsync(cancellationToken: cts.Token);

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
			AddReferralRequest referralRequest = new()
			{
				Referral = referralCode,
			};

			Console.WriteLine($"Adding referral code: {referralCode}...");

			TorBoxResponse referralResponse =
				await client.Main.User.AddReferralAsync(referralRequest, cts.Token);

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
