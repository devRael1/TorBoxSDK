using TorboxSDK.UnitTests.Helpers;
using TorBoxSDK.Main.User;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorboxSDK.UnitTests.Main.User;

public sealed class UserClientTests
{
    private const string SuccessJson = """
        {
            "success": true,
            "error": null,
            "detail": "OK."
        }
        """;

    private const string ObjectJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {}
        }
        """;

    private const string UserProfileJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {
                "id": 1,
                "email": "user@example.com",
                "plan": 2,
                "total_downloaded": 1073741824,
                "created_at": "2024-01-01T00:00:00Z",
                "updated_at": "2024-06-01T00:00:00Z",
                "is_subscribed": true,
                "auth_id": "auth-123"
            }
        }
        """;

    private const string StringDataJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": "https://example.com/pdf"
        }
        """;

    private const string ListJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": []
        }
        """;

    private const string DeviceCodeJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {
                "device_code": "device-abc",
                "code": "ABCD-1234",
                "verification_url": "https://torbox.app/verify",
                "friendly_verification_url": "https://tor.box/link",
                "expires_at": "2026-04-06T14:58:34Z",
                "interval": 5
            }
        }
        """;

    private const string ReferralJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {
                "referred_accounts": 5,
                "referral_code": "abc-123",
                "purchases_referred": 2
            }
        }
        """;

    private const string SearchEnginesJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "type": "torznab",
                    "url": "https://example.com/api"
                }
            ]
        }
        """;

    // --- RefreshTokenAsync ---

    [Fact]
    public async Task RefreshTokenAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(ObjectJson);
        RefreshTokenRequest request = new();

        // Act
        await client.RefreshTokenAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("user/refreshtoken", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task RefreshTokenAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(ObjectJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.RefreshTokenAsync(null!));
    }

    // --- GetConfirmationAsync ---

    [Fact]
    public async Task GetConfirmationAsync_WithNoParameters_SendsGetRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(ObjectJson);

        // Act
        await client.GetConfirmationAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("user/getconfirmation", handler.LastRequest.RequestUri!.ToString());
    }

    // --- GetMeAsync ---

    [Fact]
    public async Task GetMeAsync_WithValidResponse_ReturnsUserProfile()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(UserProfileJson);

        // Act
        TorBoxResponse<UserProfile> result = await client.GetMeAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("user@example.com", result.Data.Email);
    }

    [Fact]
    public async Task GetMeAsync_WithSettings_IncludesInQueryString()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(UserProfileJson);

        // Act
        await client.GetMeAsync(settings: true);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("settings=true", url);
    }

    [Fact]
    public async Task GetMeAsync_WithNullSettings_OmitsFromQueryString()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(UserProfileJson);

        // Act
        await client.GetMeAsync(settings: null);

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.DoesNotContain("settings", url);
    }

    // --- AddReferralAsync ---

    [Fact]
    public async Task AddReferralAsync_WithCode_SendsPostRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(SuccessJson);

        // Act
        await client.AddReferralAsync("REF-CODE-123");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("user/addreferral", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("referral=REF-CODE-123", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task AddReferralAsync_WithNullCode_ThrowsArgumentNullException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.AddReferralAsync(null!));
    }

    [Fact]
    public async Task AddReferralAsync_WithEmptyCode_ThrowsArgumentException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.AddReferralAsync(""));
    }

    // --- StartDeviceAuthAsync ---

    [Fact]
    public async Task StartDeviceAuthAsync_WithNoApp_SendsGetRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(DeviceCodeJson);

        // Act
        TorBoxResponse<DeviceCode> result = await client.StartDeviceAuthAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("user/auth/device/start", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task StartDeviceAuthAsync_WithApp_IncludesInQueryString()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(DeviceCodeJson);

        // Act
        await client.StartDeviceAuthAsync(app: "my-app");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Contains("app=my-app", handler.LastRequest.RequestUri!.ToString());
    }

    // --- GetDeviceTokenAsync ---

    [Fact]
    public async Task GetDeviceTokenAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(ObjectJson);
        DeviceTokenRequest request = new();

        // Act
        await client.GetDeviceTokenAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("user/auth/device/token", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetDeviceTokenAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(ObjectJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetDeviceTokenAsync(null!));
    }

    // --- DeleteMeAsync ---

    [Fact]
    public async Task DeleteMeAsync_WithValidRequest_SendsDeleteRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(SuccessJson);
        DeleteAccountRequest request = new();

        // Act
        await client.DeleteMeAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Delete, handler.LastRequest.Method);
        Assert.Contains("user/deleteme", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task DeleteMeAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.DeleteMeAsync(null!));
    }

    // --- GetReferralDataAsync ---

    [Fact]
    public async Task GetReferralDataAsync_WithNoParameters_SendsGetRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(ReferralJson);

        // Act
        TorBoxResponse<ReferralData> result = await client.GetReferralDataAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("user/referraldata", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetSubscriptionsAsync ---

    [Fact]
    public async Task GetSubscriptionsAsync_WithNoParameters_SendsGetRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(ListJson);

        // Act
        TorBoxResponse<IReadOnlyList<Subscription>> result = await client.GetSubscriptionsAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("user/subscriptions", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetTransactionsAsync ---

    [Fact]
    public async Task GetTransactionsAsync_WithNoParameters_SendsGetRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(ListJson);

        // Act
        TorBoxResponse<IReadOnlyList<Transaction>> result = await client.GetTransactionsAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("user/transactions", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetTransactionPdfAsync ---

    [Fact]
    public async Task GetTransactionPdfAsync_WithTransactionId_SendsGetRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(StringDataJson);

        // Act
        await client.GetTransactionPdfAsync("42");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("user/transaction/pdf", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("transaction_id=42", handler.LastRequest.RequestUri!.ToString());
    }

    // --- AddSearchEnginesAsync ---

    [Fact]
    public async Task AddSearchEnginesAsync_WithValidRequest_SendsPutRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(SuccessJson);
        AddSearchEnginesRequest request = new() { Type = "torznab", Url = "https://example.com/api" };

        // Act
        await client.AddSearchEnginesAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Put, handler.LastRequest.Method);
        Assert.Contains("user/settings/addsearchengines", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task AddSearchEnginesAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.AddSearchEnginesAsync(null!));
    }

    [Fact]
    public async Task AddSearchEnginesAsync_WithEmptyType_ThrowsArgumentException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(SuccessJson);
        AddSearchEnginesRequest request = new() { Type = "", Url = "https://example.com" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.AddSearchEnginesAsync(request));
    }

    // --- GetSearchEnginesAsync ---

    [Fact]
    public async Task GetSearchEnginesAsync_WithNoId_SendsGetRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(SearchEnginesJson);

        // Act
        TorBoxResponse<IReadOnlyList<SearchEngine>> result = await client.GetSearchEnginesAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("user/settings/searchengines", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetSearchEnginesAsync_WithId_IncludesInQueryString()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(SearchEnginesJson);

        // Act
        await client.GetSearchEnginesAsync(id: 42);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Contains("id=42", handler.LastRequest.RequestUri!.ToString());
    }

    // --- ModifySearchEnginesAsync ---

    [Fact]
    public async Task ModifySearchEnginesAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(SuccessJson);
        ModifySearchEnginesRequest request = new() { Id = 1 };

        // Act
        await client.ModifySearchEnginesAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("user/settings/modifysearchengines", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task ModifySearchEnginesAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.ModifySearchEnginesAsync(null!));
    }

    // --- ControlSearchEnginesAsync ---

    [Fact]
    public async Task ControlSearchEnginesAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(SuccessJson);
        ControlSearchEnginesRequest request = new() { Operation = "delete" };

        // Act
        await client.ControlSearchEnginesAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("user/settings/controlsearchengines", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task ControlSearchEnginesAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.ControlSearchEnginesAsync(null!));
    }

    [Fact]
    public async Task ControlSearchEnginesAsync_WithEmptyOperation_ThrowsArgumentException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(SuccessJson);
        ControlSearchEnginesRequest request = new() { Operation = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.ControlSearchEnginesAsync(request));
    }

    // --- EditSettingsAsync ---

    [Fact]
    public async Task EditSettingsAsync_WithValidRequest_SendsPutRequest()
    {
        // Arrange
        (UserClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<UserClient>(SuccessJson);
        EditSettingsRequest request = new();

        // Act
        await client.EditSettingsAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Put, handler.LastRequest.Method);
        Assert.Contains("user/settings/editsettings", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task EditSettingsAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (UserClient client, _) = ClientTestBase.CreateClient<UserClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.EditSettingsAsync(null!));
    }
}
