using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK;
using TorBoxSDK.DependencyInjection;

namespace TorBoxSDK.Examples.Helpers;

/// <summary>
/// Provides shared helper methods for all examples, including DI setup and client creation.
/// </summary>
public static class ExampleHelper
{
    private static ServiceProvider? _provider;

    /// <summary>
    /// Creates a configured <see cref="ITorBoxClient"/> using the standard DI pattern.
    /// Reads the API key from the <c>TORBOX_API_KEY</c> environment variable.
    /// </summary>
    /// <remarks>
    /// A single shared <see cref="ServiceProvider"/> is kept for the process lifetime
    /// so that <see cref="HttpClient"/> handlers and sockets are reused across examples.
    /// Call <see cref="DisposeProvider"/> on exit to release all resources.
    /// </remarks>
    /// <returns>A fully configured <see cref="ITorBoxClient"/> instance.</returns>
    public static ITorBoxClient CreateClient()
    {
        if (_provider is null)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddTorBox(options =>
            {
                options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
                    ?? throw new InvalidOperationException(
                        "Set the TORBOX_API_KEY environment variable before running examples.");
            });

            _provider = services.BuildServiceProvider();
        }

        return _provider.GetRequiredService<ITorBoxClient>();
    }

    /// <summary>
    /// Disposes the shared <see cref="ServiceProvider"/> and releases all resources.
    /// Call this once when the application is shutting down.
    /// </summary>
    public static void DisposeProvider()
    {
        _provider?.Dispose();
        _provider = null;
    }

    /// <summary>
    /// Creates a <see cref="CancellationTokenSource"/> with a default 30-second timeout.
    /// </summary>
    /// <param name="timeoutSeconds">Timeout duration in seconds. Defaults to 30.</param>
    /// <returns>A new <see cref="CancellationTokenSource"/> with the specified timeout.</returns>
    public static CancellationTokenSource CreateTimeout(int timeoutSeconds = 30)
        => new(TimeSpan.FromSeconds(timeoutSeconds));

    /// <summary>
    /// Prints a section header to the console for visual clarity.
    /// </summary>
    /// <param name="title">The section title to display.</param>
    public static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();
    }

    /// <summary>
    /// Formats a byte count as a human-readable string (e.g., "1.5 GB").
    /// </summary>
    /// <param name="bytes">The byte count to format.</param>
    /// <returns>A human-readable string representation of the byte count.</returns>
    public static string FormatBytes(long bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB", "TB"];
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < suffixes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {suffixes[order]}";
    }
}
