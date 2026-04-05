using TorBoxSDK.Main.Vendors;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Vendors;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Main.Vendors;

public sealed class VendorsClientTests
{
    private const string SuccessJson = """
        {
            "success": true,
            "error": null,
            "detail": "OK."
        }
        """;

    private const string VendorAccountJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {
                "id": 1,
                "vendor_name": "My Vendor",
                "vendor_url": "https://vendor.example.com",
                "api_key": "vendor-key-123",
                "created_at": "2024-01-01T00:00:00Z"
            }
        }
        """;

    private const string VendorAccountsListJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "vendor_name": "My Vendor",
                    "vendor_url": "https://vendor.example.com",
                    "api_key": "vendor-key-123",
                    "created_at": "2024-01-01T00:00:00Z"
                }
            ]
        }
        """;

    // --- RegisterAsync ---

    [Fact]
    public async Task RegisterAsync_WithValidRequest_SendsPostMultipart()
    {
        // Arrange
        (VendorsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);
        RegisterVendorRequest request = new() { VendorName = "My Vendor" };

        // Act
        TorBoxResponse<VendorAccount> result = await client.RegisterAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("vendors/register", handler.LastRequest.RequestUri!.ToString());
        Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task RegisterAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (VendorsClient client, _) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.RegisterAsync(null!));
    }

    [Fact]
    public async Task RegisterAsync_WithEmptyVendorName_ThrowsArgumentException()
    {
        // Arrange
        (VendorsClient client, _) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);
        RegisterVendorRequest request = new() { VendorName = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.RegisterAsync(request));
    }

    // --- GetAccountAsync ---

    [Fact]
    public async Task GetAccountAsync_SendsGetRequest()
    {
        // Arrange
        (VendorsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);

        // Act
        TorBoxResponse<VendorAccount> result = await client.GetAccountAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("vendors/account", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- UpdateAccountAsync ---

    [Fact]
    public async Task UpdateAccountAsync_WithValidRequest_SendsPutMultipart()
    {
        // Arrange
        (VendorsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);
        UpdateVendorAccountRequest request = new() { VendorName = "Updated Vendor" };

        // Act
        TorBoxResponse<VendorAccount> result = await client.UpdateAccountAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Put, handler.LastRequest.Method);
        Assert.Contains("vendors/updateaccount", handler.LastRequest.RequestUri!.ToString());
        Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task UpdateAccountAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (VendorsClient client, _) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.UpdateAccountAsync(null!));
    }

    [Fact]
    public async Task UpdateAccountAsync_WithNoFields_ThrowsArgumentException()
    {
        // Arrange
        (VendorsClient client, _) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);
        UpdateVendorAccountRequest request = new();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateAccountAsync(request));
    }

    // --- GetAccountsAsync ---

    [Fact]
    public async Task GetAccountsAsync_SendsGetRequest()
    {
        // Arrange
        (VendorsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountsListJson);

        // Act
        TorBoxResponse<IReadOnlyList<VendorAccount>> result = await client.GetAccountsAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("vendors/getaccounts", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetAccountByAuthIdAsync ---

    [Fact]
    public async Task GetAccountByAuthIdAsync_WithAuthId_SendsGetRequest()
    {
        // Arrange
        (VendorsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);

        // Act
        await client.GetAccountByAuthIdAsync("auth-123");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("vendors/getaccount", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("user_auth_id=auth-123", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetAccountByAuthIdAsync_WithNullAuthId_ThrowsArgumentNullException()
    {
        // Arrange
        (VendorsClient client, _) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAccountByAuthIdAsync(null!));
    }

    [Fact]
    public async Task GetAccountByAuthIdAsync_WithEmptyAuthId_ThrowsArgumentException()
    {
        // Arrange
        (VendorsClient client, _) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetAccountByAuthIdAsync(""));
    }

    // --- RegisterUserAsync ---

    [Fact]
    public async Task RegisterUserAsync_WithValidRequest_SendsPostMultipart()
    {
        // Arrange
        (VendorsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<VendorsClient>(SuccessJson);
        RegisterVendorUserRequest request = new() { UserEmail = "user@example.com" };

        // Act
        TorBoxResponse result = await client.RegisterUserAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("vendors/registeruser", handler.LastRequest.RequestUri!.ToString());
        Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task RegisterUserAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (VendorsClient client, _) = ClientTestBase.CreateClient<VendorsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.RegisterUserAsync(null!));
    }

    [Fact]
    public async Task RegisterUserAsync_WithEmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        (VendorsClient client, _) = ClientTestBase.CreateClient<VendorsClient>(SuccessJson);
        RegisterVendorUserRequest request = new() { UserEmail = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.RegisterUserAsync(request));
    }

    // --- RemoveUserAsync ---

    [Fact]
    public async Task RemoveUserAsync_WithValidRequest_SendsDeleteRequest()
    {
        // Arrange
        (VendorsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<VendorsClient>(SuccessJson);
        RemoveVendorUserRequest request = new();

        // Act
        TorBoxResponse result = await client.RemoveUserAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Delete, handler.LastRequest.Method);
        Assert.Contains("vendors/removeuser", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task RemoveUserAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (VendorsClient client, _) = ClientTestBase.CreateClient<VendorsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.RemoveUserAsync(null!));
    }

    // --- RefreshAsync ---

    [Fact]
    public async Task RefreshAsync_SendsPatchRequest()
    {
        // Arrange
        (VendorsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<VendorsClient>(VendorAccountJson);

        // Act
        TorBoxResponse<VendorAccount> result = await client.RefreshAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Patch, handler.LastRequest.Method);
        Assert.Contains("vendors/refresh", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }
}
