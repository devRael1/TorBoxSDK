using TorBoxSDK.Examples.ErrorHandling;
using TorBoxSDK.Examples.GettingStarted;
using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Examples.Main.General;
using TorBoxSDK.Examples.Main.Integrations;
using TorBoxSDK.Examples.Main.Notifications;
using TorBoxSDK.Examples.Main.Queued;
using TorBoxSDK.Examples.Main.Rss;
using TorBoxSDK.Examples.Main.Stream;
using TorBoxSDK.Examples.Main.Torrents;
using TorBoxSDK.Examples.Main.Usenet;
using TorBoxSDK.Examples.Main.User;
using TorBoxSDK.Examples.Main.Vendors;
using TorBoxSDK.Examples.Main.WebDownloads;
using TorBoxSDK.Examples.Relay;
using TorBoxSDK.Examples.Search;

Console.WriteLine("""
    ╔══════════════════════════════════════════════════════════╗
    ║              TorBoxSDK — Examples Runner                ║
    ╠══════════════════════════════════════════════════════════╣
    ║                                                          ║
    ║  Getting Started                                         ║
    ║    1.  Basic DI Setup                                    ║
    ║    2.  Configuration from appsettings.json               ║
    ║                                                          ║
    ║  ── Main Client ─────────────────────────────────────    ║
    ║                                                          ║
    ║  Main > Torrents                                         ║
    ║    3.  List Torrents                                     ║
    ║    4.  Create Torrent                                    ║
    ║    5.  Control Torrent (pause/resume/delete)             ║
    ║    6.  Download Torrent                                  ║
    ║    7.  Check Cached                                      ║
    ║    8.  Edit Torrent, Info by File & Magnet-to-File       ║
    ║                                                          ║
    ║  Main > Usenet                                           ║
    ║    9.  List Usenet Downloads                             ║
    ║   10.  Create Usenet Download                            ║
    ║   11.  Usenet Advanced (Cache, Edit, Async Create)       ║
    ║                                                          ║
    ║  Main > Web Downloads                                    ║
    ║   12.  List Web Downloads                                ║
    ║   13.  Create Web Download & Hosters                     ║
    ║   14.  Web Downloads Advanced (Cache, Edit, Async)       ║
    ║                                                          ║
    ║  Main > User                                             ║
    ║   15.  Profile & Account Info                            ║
    ║   16.  Manage Settings                                   ║
    ║   17.  Authentication & Device Auth                      ║
    ║   18.  Search Engines Management                         ║
    ║   19.  Transactions & PDF Export                         ║
    ║                                                          ║
    ║  Main > Notifications                                    ║
    ║   20.  Manage Notifications                              ║
    ║                                                          ║
    ║  Main > RSS                                              ║
    ║   21.  RSS Feeds                                         ║
    ║                                                          ║
    ║  Main > Integrations                                     ║
    ║   22.  Cloud Storage Integration (Google Drive)          ║
    ║   23.  All Cloud Providers (Dropbox, OneDrive, etc.)     ║
    ║   24.  OAuth & Discord Integration                       ║
    ║   25.  Job Management                                    ║
    ║                                                          ║
    ║  Main > Vendors                                          ║
    ║   26.  Vendor Account Management                         ║
    ║                                                          ║
    ║  Main > Queued                                           ║
    ║   27.  Queued Downloads                                  ║
    ║                                                          ║
    ║  Main > Stream                                           ║
    ║   28.  Stream Media Files                                ║
    ║                                                          ║
    ║  Main > General                                          ║
    ║   29.  Service Status & Stats                            ║
    ║   30.  Speedtest Files                                   ║
    ║                                                          ║
    ║  ── Search Client ───────────────────────────────────    ║
    ║                                                          ║
    ║  Search                                                  ║
    ║   31.  Search Torrents                                   ║
    ║   32.  Search Usenet                                     ║
    ║   33.  Search Meta (Movies, TV)                          ║
    ║   34.  Search Tutorials                                  ║
    ║   35.  Download Search Results & Get by ID               ║
    ║                                                          ║
    ║  ── Relay Client ────────────────────────────────────    ║
    ║                                                          ║
    ║  Relay                                                   ║
    ║   36.  Relay Status & Inactivity Check                   ║
    ║                                                          ║
    ║  ── Other ───────────────────────────────────────────    ║
    ║                                                          ║
    ║  Error Handling                                           ║
    ║   37.  Comprehensive Error Handling Patterns             ║
    ║                                                          ║
    ║    0.  Exit                                              ║
    ║                                                          ║
    ╚══════════════════════════════════════════════════════════╝
    """);

while (true)
{
    Console.Write("Select an example (0-37): ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int choice))
    {
        Console.WriteLine("Invalid input. Please enter a number between 0 and 37.");
        continue;
    }

    if (choice == 0)
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    if (choice is < 1 or > 37)
    {
        Console.WriteLine("Invalid choice. Please enter a number between 0 and 37.");
        continue;
    }

    try
    {
        await (choice switch
        {
            // Getting Started
            1 => BasicSetupExample.RunAsync(),
            2 => ConfigurationExample.RunAsync(),

            // Main > Torrents
            3 => ListTorrentsExample.RunAsync(),
            4 => CreateTorrentExample.RunAsync(),
            5 => ControlTorrentExample.RunAsync(),
            6 => DownloadTorrentExample.RunAsync(),
            7 => CheckCachedExample.RunAsync(),
            8 => EditTorrentExample.RunAsync(),

            // Main > Usenet
            9 => ListUsenetExample.RunAsync(),
            10 => CreateUsenetExample.RunAsync(),
            11 => UsenetAdvancedExample.RunAsync(),

            // Main > Web Downloads
            12 => ListWebDownloadsExample.RunAsync(),
            13 => CreateWebDownloadExample.RunAsync(),
            14 => WebDownloadsAdvancedExample.RunAsync(),

            // Main > User
            15 => GetProfileExample.RunAsync(),
            16 => ManageSettingsExample.RunAsync(),
            17 => AuthenticationExample.RunAsync(),
            18 => SearchEnginesExample.RunAsync(),
            19 => TransactionsExample.RunAsync(),

            // Main > Notifications
            20 => NotificationsExample.RunAsync(),

            // Main > RSS
            21 => RssFeedsExample.RunAsync(),

            // Main > Integrations
            22 => CloudIntegrationExample.RunAsync(),
            23 => AllCloudProvidersExample.RunAsync(),
            24 => OAuthExample.RunAsync(),
            25 => JobManagementExample.RunAsync(),

            // Main > Vendors
            26 => VendorExample.RunAsync(),

            // Main > Queued
            27 => QueuedDownloadsExample.RunAsync(),

            // Main > Stream
            28 => StreamExample.RunAsync(),

            // Main > General
            29 => GeneralExample.RunAsync(),
            30 => SpeedtestExample.RunAsync(),

            // Search
            31 => SearchTorrentsExample.RunAsync(),
            32 => SearchUsenetExample.RunAsync(),
            33 => SearchMetaExample.RunAsync(),
            34 => SearchTutorialsExample.RunAsync(),
            35 => DownloadSearchResultsExample.RunAsync(),

            // Relay
            36 => RelayExample.RunAsync(),

            // Error Handling
            37 => ErrorHandlingExample.RunAsync(),

            _ => Task.CompletedTask,
        });
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Unexpected error: {ex.Message}");
    }

    Console.WriteLine();
    Console.WriteLine("Press Enter to return to the menu...");
    Console.ReadLine();
}

ExampleHelper.DisposeProvider();
