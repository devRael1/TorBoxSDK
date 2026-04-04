using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Relay;
using TorBoxSDK.Relay;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Relay;

public sealed class RelayApiClientTests
{
    private const string StatusJson = """
        {
            "success": true,
            "error": null,
            "detail": "OK.",
            "data": {
                "status": "operational"
            }
        }
        """;

    private const string InactiveCheckJson = """
        {
            "success": true,
            "error": null,
            "detail": "OK.",
            "data": {
                "status": "active",
                "is_inactive": false,
                "last_active": "2024-01-15T12:00:00Z"
            }
        }
        """;

    [Fact]
    public async Task GetStatusAsync_WithValidResponse_ReturnsStatus()
    {
        // Arrange
        var (client, handler) = ClientTestBase.CreateClient<RelayApiClient>(StatusJson);

        // Act
        TorBoxResponse<RelayStatus> result = await client.GetStatusAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("operational", result.Data.Status);
    }

    [Fact]
    public async Task CheckForInactiveAsync_WithValidParams_SendsCorrectUrl()
    {
        // Arrange
        var (client, handler) = ClientTestBase.CreateClient<RelayApiClient>(InactiveCheckJson);

        // Act
        TorBoxResponse<InactiveCheckResult> result = await client.CheckForInactiveAsync("auth-id-123", 42);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("v1/inactivecheck/torrent/auth-id-123/42", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CheckForInactiveAsync_WithNullAuthId_ThrowsArgumentException()
    {
        // Arrange
        var (client, _) = ClientTestBase.CreateClient<RelayApiClient>(InactiveCheckJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.CheckForInactiveAsync(null!, 42));
    }
}
