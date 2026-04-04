using System.Net;

using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorboxSDK.UnitTests.Helpers;

internal static class ClientTestBase
{
    private static readonly Dictionary<Type, string> BaseAddresses = new()
    {
        [typeof(SearchApiClient)] = "https://search-api.torbox.app/",
        [typeof(RelayApiClient)] = "https://relay.torbox.app/",
    };

    internal static (TClient client, MockHttpMessageHandler handler) CreateClient<TClient>(
        string json,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string? baseAddress = null)
        where TClient : class
    {
        string resolvedBaseAddress = baseAddress
            ?? (BaseAddresses.TryGetValue(typeof(TClient), out string? mapped) ? mapped : "https://api.torbox.app/v1/api/");

        var handler = new MockHttpMessageHandler(json, statusCode);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri(resolvedBaseAddress) };

        TClient client = (TClient?)Activator.CreateInstance(typeof(TClient), httpClient)
            ?? throw new InvalidOperationException($"Failed to create instance of {typeof(TClient).Name}. Ensure it has a public constructor accepting HttpClient.");

        return (client, handler);
    }
}
