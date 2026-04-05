using TorBoxSDK.Examples.ErrorHandling;
using TorBoxSDK.Examples.General;
using TorBoxSDK.Examples.GettingStarted;
using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Examples.Integrations;
using TorBoxSDK.Examples.Notifications;
using TorBoxSDK.Examples.Rss;
using TorBoxSDK.Examples.Search;
using TorBoxSDK.Examples.Streaming;
using TorBoxSDK.Examples.Torrents;
using TorBoxSDK.Examples.Usenet;
using TorBoxSDK.Examples.User;
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
    ║                                                          ║
    ║  Usenet                                                  ║
    ║    8.  List Usenet Downloads                             ║
    ║    9.  Create Usenet Download                            ║
    ║                                                          ║
    ║  Web Downloads                                           ║
    ║   10.  List Web Downloads                                ║
    ║   11.  Create Web Download & Hosters                     ║
    ║                                                          ║
    ║  Search                                                  ║
    ║   12.  Search Torrents                                   ║
    ║   13.  Search Usenet                                     ║
    ║   14.  Search Meta (Movies, TV)                          ║
    ║                                                          ║
    ║  User                                                    ║
    ║   15.  Profile & Account Info                            ║
    ║   16.  Manage Settings                                   ║
    ║                                                          ║
    ║  Notifications                                           ║
    ║   17.  Manage Notifications                              ║
    ║                                                          ║
    ║  RSS                                                     ║
    ║   18.  RSS Feeds                                         ║
    ║                                                          ║
    ║  Integrations                                            ║
    ║   19.  Cloud Storage Integration                         ║
    ║                                                          ║
    ║  Streaming                                               ║
    ║   20.  Stream Media Files                                ║
    ║                                                          ║
    ║  General                                                 ║
    ║   21.  Service Status & Stats                            ║
    ║                                                          ║
    ║  Error Handling                                           ║
    ║   22.  Comprehensive Error Handling Patterns             ║
    ║                                                          ║
    ║    0.  Exit                                              ║
    ║                                                          ║
    ╚══════════════════════════════════════════════════════════╝
    """);

while (true)
{
    Console.Write("Select an example (0-22): ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int choice))
    {
        Console.WriteLine("Invalid input. Please enter a number between 0 and 22.");
        continue;
    }

    if (choice == 0)
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    if (choice is < 1 or > 22)
    {
        Console.WriteLine("Invalid choice. Please enter a number between 0 and 22.");
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
            8 => ListUsenetExample.RunAsync(),
            9 => CreateUsenetExample.RunAsync(),
            10 => ListWebDownloadsExample.RunAsync(),
            11 => CreateWebDownloadExample.RunAsync(),
            12 => SearchTorrentsExample.RunAsync(),
            13 => SearchUsenetExample.RunAsync(),
            14 => SearchMetaExample.RunAsync(),
            15 => GetProfileExample.RunAsync(),
            16 => ManageSettingsExample.RunAsync(),
            17 => NotificationsExample.RunAsync(),
            18 => RssFeedsExample.RunAsync(),
            19 => CloudIntegrationExample.RunAsync(),
            20 => StreamExample.RunAsync(),
            21 => GeneralExample.RunAsync(),
            22 => ErrorHandlingExample.RunAsync(),
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
