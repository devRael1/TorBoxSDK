using TorBoxSDK.Main.Integrations;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Main.Integrations;

public sealed class IntegrationsClientTests
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

    private const string StringDataJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": "https://example.com/oauth/redirect"
        }
        """;

    private const string OAuthListJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "provider": "googledrive",
                    "status": "active"
                }
            ]
        }
        """;

    private const string JobJson = """
        {
            "success": true,
            "error": null,
            "detail": "Created.",
            "data": {
                "id": 1,
                "auth_id": "auth-123",
                "job_type": "googledrive",
                "status": "pending",
                "progress": 0
            }
        }
        """;

    private const string JobsListJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "auth_id": "auth-123",
                    "job_type": "googledrive",
                    "status": "completed",
                    "progress": 100
                }
            ]
        }
        """;

    // --- GetOAuthMeAsync ---

    [Fact]
    public async Task GetOAuthMeAsync_SendsGetRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(OAuthListJson);

        // Act
        await client.GetOAuthMeAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("integration/oauth/me", handler.LastRequest.RequestUri!.ToString());
    }

    // --- Create*JobAsync (all 6 providers) ---

    [Theory]
    [InlineData("googledrive", "integration/googledrive")]
    [InlineData("dropbox", "integration/dropbox")]
    [InlineData("onedrive", "integration/onedrive")]
    [InlineData("gofile", "integration/gofile")]
    [InlineData("1fichier", "integration/1fichier")]
    [InlineData("pixeldrain", "integration/pixeldrain")]
    public async Task CreateJobAsync_ForProvider_SendsPostRequest(string provider, string expectedUrl)
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobJson);
        CreateIntegrationJobRequest request = new() { DownloadId = 1, DownloadType = "torrent" };

        // Act
        TorBoxResponse<IntegrationJob> result = provider switch
        {
            "googledrive" => await client.CreateGoogleDriveJobAsync(request),
            "dropbox" => await client.CreateDropboxJobAsync(request),
            "onedrive" => await client.CreateOnedriveJobAsync(request),
            "gofile" => await client.CreateGofileJobAsync(request),
            "1fichier" => await client.CreateOneFichierJobAsync(request),
            "pixeldrain" => await client.CreatePixeldrainJobAsync(request),
            _ => throw new ArgumentException($"Unknown provider: {provider}"),
        };

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains(expectedUrl, handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task CreateGoogleDriveJobAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(JobJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CreateGoogleDriveJobAsync(null!));
    }

    // --- GetJobAsync ---

    [Fact]
    public async Task GetJobAsync_WithJobId_SendsGetRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobJson);

        // Act
        TorBoxResponse<IntegrationJob> result = await client.GetJobAsync(42);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("integration/job/42", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- DeleteJobAsync ---

    [Fact]
    public async Task DeleteJobAsync_WithJobId_SendsDeleteRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);

        // Act
        TorBoxResponse result = await client.DeleteJobAsync(42);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Delete, handler.LastRequest.Method);
        Assert.Contains("integration/job/42", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetJobsAsync ---

    [Fact]
    public async Task GetJobsAsync_SendsGetRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobsListJson);

        // Act
        TorBoxResponse<IReadOnlyList<IntegrationJob>> result = await client.GetJobsAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("integration/jobs", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    // --- GetJobsByHashAsync ---

    [Fact]
    public async Task GetJobsByHashAsync_WithHash_SendsGetRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobsListJson);

        // Act
        await client.GetJobsByHashAsync("abc123");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("integration/jobs/abc123", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetJobsByHashAsync_WithNullHash_ThrowsArgumentNullException()
    {
        // Arrange
        (IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(JobsListJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetJobsByHashAsync(null!));
    }

    [Fact]
    public async Task GetJobsByHashAsync_WithEmptyHash_ThrowsArgumentException()
    {
        // Arrange
        (IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(JobsListJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetJobsByHashAsync(""));
    }

    // --- OAuthRedirectAsync ---

    [Fact]
    public async Task OAuthRedirectAsync_WithProvider_SendsGetRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(StringDataJson);

        // Act
        await client.OAuthRedirectAsync("googledrive");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("integration/oauth/googledrive", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task OAuthRedirectAsync_WithNullProvider_ThrowsArgumentNullException()
    {
        // Arrange
        (IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(StringDataJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.OAuthRedirectAsync(null!));
    }

    // --- OAuthCallbackAsync ---

    [Fact]
    public async Task OAuthCallbackAsync_WithProvider_SendsGetRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(ObjectJson);

        // Act
        await client.OAuthCallbackAsync("googledrive");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("integration/oauth/googledrive/callback", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task OAuthCallbackAsync_WithNullProvider_ThrowsArgumentNullException()
    {
        // Arrange
        (IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(ObjectJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.OAuthCallbackAsync(null!));
    }

    // --- OAuthSuccessAsync ---

    [Fact]
    public async Task OAuthSuccessAsync_WithProvider_SendsGetRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(ObjectJson);

        // Act
        await client.OAuthSuccessAsync("googledrive");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("integration/oauth/googledrive/success", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task OAuthSuccessAsync_WithNullProvider_ThrowsArgumentNullException()
    {
        // Arrange
        (IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(ObjectJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.OAuthSuccessAsync(null!));
    }

    // --- OAuthRegisterAsync ---

    [Fact]
    public async Task OAuthRegisterAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);
        OAuthRegisterRequest request = new();

        // Act
        await client.OAuthRegisterAsync("googledrive", request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("integration/oauth/googledrive/register", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task OAuthRegisterAsync_WithNullProvider_ThrowsArgumentNullException()
    {
        // Arrange
        (IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);
        OAuthRegisterRequest request = new();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.OAuthRegisterAsync(null!, request));
    }

    [Fact]
    public async Task OAuthRegisterAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.OAuthRegisterAsync("googledrive", null!));
    }

    // --- OAuthUnregisterAsync ---

    [Fact]
    public async Task OAuthUnregisterAsync_WithProvider_SendsDeleteRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);

        // Act
        await client.OAuthUnregisterAsync("googledrive");

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Delete, handler.LastRequest.Method);
        Assert.Contains("integration/oauth/googledrive/unregister", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task OAuthUnregisterAsync_WithNullProvider_ThrowsArgumentNullException()
    {
        // Arrange
        (IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.OAuthUnregisterAsync(null!));
    }

    // --- GetLinkedDiscordRolesAsync ---

    [Fact]
    public async Task GetLinkedDiscordRolesAsync_SendsPostRequest()
    {
        // Arrange
        (IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(ObjectJson);

        // Act
        await client.GetLinkedDiscordRolesAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("integration/oauth/discord/linked_roles", handler.LastRequest.RequestUri!.ToString());
    }
}
