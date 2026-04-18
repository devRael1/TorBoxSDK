using TorboxSDK.UnitTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Relay;
using TorBoxSDK.Relay;

namespace TorboxSDK.UnitTests.Relay;

public sealed class RelayApiClientTests
{
    private const string StatusJson = """
        {
            "status": "TorBox Relay Satellite is running.",
            "data": {
                "current_online": 0,
                "current_workers": 5
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
        (RelayApiClient client, MockHttpMessageHandler _) = ClientTestBase.CreateClient<RelayApiClient>(StatusJson);

        // Act
        TorBoxResponse<RelayStatus> result = await client.GetStatusAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("TorBox Relay Satellite is running.", result.Data.Status);
        Assert.NotNull(result.Data.Data);
        Assert.Equal(0, result.Data.Data.CurrentOnline);
        Assert.Equal(5, result.Data.Data.CurrentWorkers);
    }

    [Fact]
    public async Task CheckForInactiveAsync_WithValidParams_SendsCorrectUrl()
    {
        // Arrange
        (RelayApiClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<RelayApiClient>(InactiveCheckJson);

        // Act
        TorBoxResponse<InactiveCheckResult> result = await client.CheckForInactiveAsync("auth-id-123", 42);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("v1/inactivecheck/torrent/auth-id-123/42", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CheckForInactiveAsync_WithNullAuthId_ThrowsArgumentNullException()
    {
        // Arrange
        (RelayApiClient client, _) = ClientTestBase.CreateClient<RelayApiClient>(InactiveCheckJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CheckForInactiveAsync(null!, 1));
    }

    [Fact]
    public async Task CheckForInactiveAsync_WithEmptyAuthId_ThrowsArgumentException()
    {
        // Arrange
        (RelayApiClient client, _) = ClientTestBase.CreateClient<RelayApiClient>(InactiveCheckJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.CheckForInactiveAsync(string.Empty, 1));
    }
}
