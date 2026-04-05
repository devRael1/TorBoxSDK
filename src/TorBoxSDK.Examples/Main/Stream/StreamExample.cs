using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Examples.Main.Stream;

/// <summary>
/// Demonstrates how to create streaming links for media files stored on TorBox.
/// Streaming allows playing video/audio directly without downloading.
/// </summary>
public static class StreamExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("Streaming — Create Stream Link");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Create a stream link for a specific file in a download.
            // This requires:
            //   - id: the download ID
            //   - fileId: the specific file ID within the download
            //   - type: the download type ("torrent", "usenet", "webdownload")
            // ──────────────────────────────────────────────────────
            long downloadId = 12345; // Replace with your actual download ID
            long fileId = 1;         // Replace with the actual file ID from the download's Files list
            string type = "torrent"; // Download type: "torrent", "usenet", or "webdownload"

            Console.WriteLine($"Creating stream for download {downloadId}, file {fileId} (type: {type})...");

            TorBoxResponse<string> streamResponse =
                await client.Main.Stream.CreateStreamAsync(
                    downloadId,
                    fileId,
                    type,
                    ct: cts.Token);

            if (streamResponse.Data is not null)
            {
                Console.WriteLine($"  Stream URL: {streamResponse.Data}");
                Console.WriteLine();
                Console.WriteLine("  You can open this URL in a media player (VLC, mpv, etc.)");
                Console.WriteLine("  or embed it in a web application for playback.");
            }

            // ──────────────────────────────────────────────────────
            // Create a stream with specific subtitle and audio tracks.
            // ──────────────────────────────────────────────────────
            int subtitleIndex = 0; // Replace with the desired subtitle track index
            int audioIndex = 0;   // Replace with the desired audio track index

            Console.WriteLine();
            Console.WriteLine("Creating stream with specific subtitle and audio tracks...");

            TorBoxResponse<string> customStreamResponse =
                await client.Main.Stream.CreateStreamAsync(
                    downloadId,
                    fileId,
                    type,
                    chosenSubtitleIndex: subtitleIndex,
                    chosenAudioIndex: audioIndex,
                    ct: cts.Token);

            if (customStreamResponse.Data is not null)
            {
                Console.WriteLine($"  Custom stream URL: {customStreamResponse.Data}");

                // ──────────────────────────────────────────────────
                // Get detailed stream data using a presigned token.
                // The token is extracted from the stream URL above.
                // This returns metadata about available tracks.
                // ──────────────────────────────────────────────────
                string presignedToken = "token-from-stream-url"; // Extract from the stream URL path or query string
                string authToken = "your-auth-token";            // Your TorBox API auth token

                Console.WriteLine();
                Console.WriteLine("Getting stream data with presigned token...");

                TorBoxResponse<object> streamDataResponse =
                    await client.Main.Stream.GetStreamDataAsync(
                        presignedToken,
                        authToken,
                        chosenSubtitleIndex: subtitleIndex,
                        chosenAudioIndex: audioIndex,
                        ct: cts.Token);

                Console.WriteLine($"  Stream data: {streamDataResponse.Detail ?? "Retrieved"}");
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Streaming example completed.");
    }
}
