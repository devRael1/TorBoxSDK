using System.Net;

namespace TorboxSDK.UnitTests.Helpers;

internal static class ClientTestBase
{
    internal static (TClient client, MockHttpMessageHandler handler) CreateClient<TClient>(
        string json,
        HttpStatusCode statusCode = HttpStatusCode.OK)
        where TClient : class
    {
        var handler = new MockHttpMessageHandler(json, statusCode);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.torbox.app/v1/api/") };
        TClient client = (TClient)Activator.CreateInstance(typeof(TClient), httpClient)!;
        return (client, handler);
    }
}
