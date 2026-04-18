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
    ║    3.  Standalone Setup (No DI)                          ║
    ║                                                          ║
    ║  ── Main Client ─────────────────────────────────────    ║
    ║                                                          ║
    ║  Main > Torrents                                         ║
    ║    4.  List Torrents                                     ║
    ║    5.  Create Torrent                                    ║
    ║    6.  Control Torrent (pause/resume/delete)             ║
    ║    7.  Download Torrent                                  ║
    ║    8.  Check Cached                                      ║
    ║    9.  Edit Torrent, Info by File & Magnet-to-File       ║
    ║                                                          ║
    ║  Main > Usenet                                           ║
    ║   10.  List Usenet Downloads                             ║
    ║   11.  Create Usenet Download                            ║
    ║   12.  Usenet Advanced (Cache, Edit, Async Create)       ║
    ║                                                          ║
    ║  Main > Web Downloads                                    ║
    ║   13.  List Web Downloads                                ║
    ║   14.  Create Web Download & Hosters                     ║
    ║   15.  Web Downloads Advanced (Cache, Edit, Async)       ║
    ║                                                          ║
    ║  Main > User                                             ║
    ║   16.  Profile & Account Info                            ║
    ║   17.  Manage Settings                                   ║
    ║   18.  Authentication & Device Auth                      ║
    ║   19.  Search Engines Management                         ║
    ║   20.  Transactions & PDF Export                         ║
    ║                                                          ║
    ║  Main > Notifications                                    ║
    ║   21.  Manage Notifications                              ║
    ║                                                          ║
    ║  Main > RSS                                              ║
    ║   22.  RSS Feeds                                         ║
    ║                                                          ║
    ║  Main > Integrations                                     ║
    ║   23.  Cloud Storage Integration (Google Drive)          ║
    ║   24.  All Cloud Providers (Dropbox, OneDrive, etc.)     ║
    ║   25.  OAuth & Discord Integration                       ║
    ║   26.  Job Management                                    ║
    ║                                                          ║
    ║  Main > Vendors                                          ║
    ║   27.  Vendor Account Management                         ║
    ║                                                          ║
    ║  Main > Queued                                           ║
    ║   28.  Queued Downloads                                  ║
    ║                                                          ║
    ║  Main > Stream                                           ║
    ║   29.  Stream Media Files                                ║
    ║                                                          ║
    ║  Main > General                                          ║
    ║   30.  Service Status & Stats                            ║
    ║   31.  Speedtest Files                                   ║
    ║                                                          ║
    ║  ── Search Client ───────────────────────────────────    ║
    ║                                                          ║
    ║  Search                                                  ║
    ║   32.  Search Torrents                                   ║
    ║   33.  Search Usenet                                     ║
    ║   34.  Search Meta (Movies, TV)                          ║
    ║   35.  Search Tutorials                                  ║
    ║   36.  Download Search Results & Get by ID               ║
    ║                                                          ║
    ║  ── Relay Client ────────────────────────────────────    ║
    ║                                                          ║
    ║  Relay                                                   ║
    ║   37.  Relay Status & Inactivity Check                   ║
    ║                                                          ║
    ║  ── Other ───────────────────────────────────────────    ║
    ║                                                          ║
    ║  Error Handling                                           ║
    ║   38.  Comprehensive Error Handling Patterns             ║
    ║                                                          ║
    ║    0.  Exit                                              ║
    ║                                                          ║
    ╚══════════════════════════════════════════════════════════╝
    """);

while (true)
{
    Console.Write("Select an example (0-38): ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int choice))
    {
        Console.WriteLine("Invalid input. Please enter a number between 0 and 38.");
        continue;
    }

    if (choice == 0)
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    if (choice is < 1 or > 38)
    {
        Console.WriteLine("Invalid choice. Please enter a number between 0 and 38.");
        continue;
    }

    try
    {
        await (choice switch
        {
            // Getting Started
            1 => BasicSetupExample.RunAsync(),
            2 => ConfigurationExample.RunAsync(),
            3 => StandaloneSetupExample.RunAsync(),

            // Main > Torrents
            4 => ListTorrentsExample.RunAsync(),
            5 => CreateTorrentExample.RunAsync(),
            6 => ControlTorrentExample.RunAsync(),
            7 => DownloadTorrentExample.RunAsync(),
            8 => CheckCachedExample.RunAsync(),
            9 => EditTorrentExample.RunAsync(),

            // Main > Usenet
            10 => ListUsenetExample.RunAsync(),
            11 => CreateUsenetExample.RunAsync(),
            12 => UsenetAdvancedExample.RunAsync(),

            // Main > Web Downloads
            13 => ListWebDownloadsExample.RunAsync(),
            14 => CreateWebDownloadExample.RunAsync(),
            15 => WebDownloadsAdvancedExample.RunAsync(),

            // Main > User
            16 => GetProfileExample.RunAsync(),
            17 => ManageSettingsExample.RunAsync(),
            18 => AuthenticationExample.RunAsync(),
            19 => SearchEnginesExample.RunAsync(),
            20 => TransactionsExample.RunAsync(),

            // Main > Notifications
            21 => NotificationsExample.RunAsync(),

            // Main > RSS
            22 => RssFeedsExample.RunAsync(),

            // Main > Integrations
            23 => CloudIntegrationExample.RunAsync(),
            24 => AllCloudProvidersExample.RunAsync(),
            25 => OAuthExample.RunAsync(),
            26 => JobManagementExample.RunAsync(),

            // Main > Vendors
            27 => VendorExample.RunAsync(),

            // Main > Queued
            28 => QueuedDownloadsExample.RunAsync(),

            // Main > Stream
            29 => StreamExample.RunAsync(),

            // Main > General
            30 => GeneralExample.RunAsync(),
            31 => SpeedtestExample.RunAsync(),

            // Search
            32 => SearchTorrentsExample.RunAsync(),
            33 => SearchUsenetExample.RunAsync(),
            34 => SearchMetaExample.RunAsync(),
            35 => SearchTutorialsExample.RunAsync(),
            36 => DownloadSearchResultsExample.RunAsync(),

            // Relay
            37 => RelayExample.RunAsync(),

            // Error Handling
            38 => ErrorHandlingExample.RunAsync(),

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
