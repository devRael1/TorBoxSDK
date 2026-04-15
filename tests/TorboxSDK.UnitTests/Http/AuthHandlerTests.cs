using Microsoft.Extensions.Options;
using TorboxSDK.UnitTests.Helpers;
using TorBoxSDK;
using TorBoxSDK.Http.Handlers;

namespace TorboxSDK.UnitTests.Http;

public sealed class AuthHandlerTests
{
    [Fact]
    public async Task SendAsync_WithApiKey_AddsAuthorizationHeader()
    {
        // Arrange
        IOptions<TorBoxClientOptions> options = Options.Create(new TorBoxClientOptions { ApiKey = "test-api-key-123" });
        var innerHandler = new MockHttpMessageHandler("""{"success":true,"error":null,"detail":"OK"}""");
        var authHandler = new AuthHandler(options) { InnerHandler = innerHandler };
        using var httpClient = new HttpClient(authHandler);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.torbox.app/v1/api/torrents/mylist");

        // Act
        await httpClient.SendAsync(request);

        // Assert
        Assert.NotNull(innerHandler.LastRequest);
        Assert.NotNull(innerHandler.LastRequest.Headers.Authorization);
        Assert.Equal("Bearer", innerHandler.LastRequest.Headers.Authorization.Scheme);
        Assert.Equal("test-api-key-123", innerHandler.LastRequest.Headers.Authorization.Parameter);
    }

    [Fact]
    public async Task SendAsync_WithEmptyApiKey_DoesNotAddAuthorizationHeader()
    {
        // Arrange
        IOptions<TorBoxClientOptions> options = Options.Create(new TorBoxClientOptions { ApiKey = string.Empty });
        var innerHandler = new MockHttpMessageHandler("""{"success":true,"error":null,"detail":"OK"}""");
        var authHandler = new AuthHandler(options) { InnerHandler = innerHandler };
        using var httpClient = new HttpClient(authHandler);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.torbox.app/v1/api/torrents/mylist");

        // Act
        await httpClient.SendAsync(request);

        // Assert
        Assert.NotNull(innerHandler.LastRequest);
        Assert.Null(innerHandler.LastRequest.Headers.Authorization);
    }
}
