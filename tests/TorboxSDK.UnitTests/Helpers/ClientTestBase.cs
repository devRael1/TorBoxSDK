using System.Net;
using System.Reflection;

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

        ConstructorInfo ctor = typeof(TClient)
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .FirstOrDefault(c =>
            {
                ParameterInfo[] ps = c.GetParameters();
                return ps.Length > 0 && ps[0].ParameterType == typeof(HttpClient);
            })
            ?? throw new InvalidOperationException($"No constructor accepting HttpClient found on {typeof(TClient).Name}.");

        ParameterInfo[] parameters = ctor.GetParameters();
        object[] args = new object[parameters.Length];
        args[0] = httpClient;
        for (int i = 1; i < parameters.Length; i++)
        {
            args[i] = parameters[i].ParameterType == typeof(string)
                ? string.Empty
                : Activator.CreateInstance(parameters[i].ParameterType)!;
        }

        TClient client = (TClient?)ctor.Invoke(args)
            ?? throw new InvalidOperationException($"Failed to create instance of {typeof(TClient).Name}.");

        return (client, handler);
    }
}
