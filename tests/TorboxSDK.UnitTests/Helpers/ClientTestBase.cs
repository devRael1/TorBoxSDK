using System.Net;

using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorboxSDK.UnitTests.Helpers;

internal static class ClientTestBase
{
    private static readonly Dictionary<Type, string> _baseAddresses = new()
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
            ?? (_baseAddresses.TryGetValue(typeof(TClient), out string? mapped) ? mapped : "https://api.torbox.app/v1/api/");

        MockHttpMessageHandler handler = new(json, statusCode);
        HttpClient httpClient = new(handler) { BaseAddress = new Uri(resolvedBaseAddress) };

        TClient client = (TClient?)Activator.CreateInstance(typeof(TClient), httpClient)
            ?? throw new InvalidOperationException($"Failed to create instance of {typeof(TClient).Name}. Ensure it has an internal constructor accepting HttpClient.");

        return (client, handler);
    }
}
