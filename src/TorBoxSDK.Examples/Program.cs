using TorBoxSDK.Examples.ErrorHandling;
using TorBoxSDK.Examples.General;
using TorBoxSDK.Examples.GettingStarted;
using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Examples.Integrations;
using TorBoxSDK.Examples.Notifications;
using TorBoxSDK.Examples.Queued;
using TorBoxSDK.Examples.Relay;
using TorBoxSDK.Examples.Rss;
using TorBoxSDK.Examples.Search;
using TorBoxSDK.Examples.Streaming;
using TorBoxSDK.Examples.Torrents;
using TorBoxSDK.Examples.Usenet;
using TorBoxSDK.Examples.User;
using TorBoxSDK.Examples.Vendors;
using TorBoxSDK.Examples.WebDownloads;

Console.WriteLine("""
    ╔══════════════════════════════════════════════════════════╗
    ║              TorBoxSDK — Examples Runner                ║
    ╠══════════════════════════════════════════════════════════╣
    ║                                                          ║
    ║  Getting Started                                         ║
    ║    1.  Basic DI Setup                                    ║
    ║    2.  Configuration from appsettings.json               ║
    ║                                                          ║
    ║  Torrents                                                ║
    ║    3.  List Torrents                                     ║
    ║    4.  Create Torrent                                    ║
    ║    5.  Control Torrent (pause/resume/delete)             ║
    ║    6.  Download Torrent                                  ║
    ║    7.  Check Cached                                      ║
    ║    8.  Edit Torrent, Info by File & Magnet-to-File       ║
    ║                                                          ║
    ║  Usenet                                                  ║
    ║    9.  List Usenet Downloads                             ║
    ║   10.  Create Usenet Download                            ║
    ║   11.  Usenet Advanced (Cache, Edit, Async Create)       ║
    ║                                                          ║
    ║  Web Downloads                                           ║
    ║   12.  List Web Downloads                                ║
    ║   13.  Create Web Download & Hosters                     ║
    ║   14.  Web Downloads Advanced (Cache, Edit, Async)       ║
    ║                                                          ║
    ║  Search                                                  ║
    ║   15.  Search Torrents                                   ║
    ║   16.  Search Usenet                                     ║
    ║   17.  Search Meta (Movies, TV)                          ║
    ║   18.  Search Tutorials                                  ║
    ║   19.  Download Search Results & Get by ID               ║
    ║                                                          ║
    ║  User                                                    ║
    ║   20.  Profile & Account Info                            ║
    ║   21.  Manage Settings                                   ║
    ║   22.  Authentication & Device Auth                      ║
    ║   23.  Search Engines Management                         ║
    ║   24.  Transactions & PDF Export                         ║
    ║                                                          ║
    ║  Notifications                                           ║
    ║   25.  Manage Notifications                              ║
    ║                                                          ║
    ║  RSS                                                     ║
    ║   26.  RSS Feeds                                         ║
    ║                                                          ║
    ║  Integrations                                            ║
    ║   27.  Cloud Storage Integration (Google Drive)          ║
    ║   28.  All Cloud Providers (Dropbox, OneDrive, etc.)     ║
    ║   29.  OAuth & Discord Integration                       ║
    ║   30.  Job Management                                    ║
    ║                                                          ║
    ║  Vendors                                                 ║
    ║   31.  Vendor Account Management                         ║
    ║                                                          ║
    ║  Queued                                                  ║
    ║   32.  Queued Downloads                                  ║
    ║                                                          ║
    ║  Streaming                                               ║
    ║   33.  Stream Media Files                                ║
    ║                                                          ║
    ║  Relay                                                   ║
    ║   34.  Relay Status & Inactivity Check                   ║
    ║                                                          ║
    ║  General                                                 ║
    ║   35.  Service Status & Stats                            ║
    ║   36.  Speedtest Files                                   ║
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
            1 => BasicSetupExample.RunAsync(),
            2 => ConfigurationExample.RunAsync(),
            3 => ListTorrentsExample.RunAsync(),
            4 => CreateTorrentExample.RunAsync(),
            5 => ControlTorrentExample.RunAsync(),
            6 => DownloadTorrentExample.RunAsync(),
            7 => CheckCachedExample.RunAsync(),
            8 => EditTorrentExample.RunAsync(),
            9 => ListUsenetExample.RunAsync(),
            10 => CreateUsenetExample.RunAsync(),
            11 => UsenetAdvancedExample.RunAsync(),
            12 => ListWebDownloadsExample.RunAsync(),
            13 => CreateWebDownloadExample.RunAsync(),
            14 => WebDownloadsAdvancedExample.RunAsync(),
            15 => SearchTorrentsExample.RunAsync(),
            16 => SearchUsenetExample.RunAsync(),
            17 => SearchMetaExample.RunAsync(),
            18 => SearchTutorialsExample.RunAsync(),
            19 => DownloadSearchResultsExample.RunAsync(),
            20 => GetProfileExample.RunAsync(),
            21 => ManageSettingsExample.RunAsync(),
            22 => AuthenticationExample.RunAsync(),
            23 => SearchEnginesExample.RunAsync(),
            24 => TransactionsExample.RunAsync(),
            25 => NotificationsExample.RunAsync(),
            26 => RssFeedsExample.RunAsync(),
            27 => CloudIntegrationExample.RunAsync(),
            28 => AllCloudProvidersExample.RunAsync(),
            29 => OAuthExample.RunAsync(),
            30 => JobManagementExample.RunAsync(),
            31 => VendorExample.RunAsync(),
            32 => QueuedDownloadsExample.RunAsync(),
            33 => StreamExample.RunAsync(),
            34 => RelayExample.RunAsync(),
            35 => GeneralExample.RunAsync(),
            36 => SpeedtestExample.RunAsync(),
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
