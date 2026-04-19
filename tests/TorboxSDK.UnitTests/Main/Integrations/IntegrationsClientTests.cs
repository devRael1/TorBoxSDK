using TorboxSDK.UnitTests.Helpers;
using TorBoxSDK.Main.Integrations;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Integrations;

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

	private const string OAuthDictJson = """
        {
            "success": true,
            "error": null,
            "detail": "OAuth integrations retrieved successfully.",
            "data": {
                "trakt": false,
                "mal": false,
                "anilist": false,
                "simkl": false
            }
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
	public async Task GetOAuthMeAsync_WithNoParameters_SendsGetRequest()
	{
		// Arrange
		(IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(OAuthDictJson);

		// Act
		await client.GetOAuthMeAsync();

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
		Assert.Contains("integration/oauth/me", handler.LastRequest.RequestUri!.ToString());
	}

	// --- Create*JobAsync (all 6 providers) ---

	[Fact]
	public async Task CreateGoogleDriveJobAsync_WithValidRequest_SendsPostRequest()
	{
		// Arrange
		(IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobJson);
		CreateIntegrationJobRequest request = new() { DownloadId = 1, DownloadType = "torrent" };

		// Act
		TorBoxResponse<IntegrationJob> result = await client.CreateGoogleDriveJobAsync(request);

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("integration/googledrive", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
	}

	[Fact]
	public async Task CreateDropboxJobAsync_WithValidRequest_SendsPostRequest()
	{
		// Arrange
		(IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobJson);
		CreateIntegrationJobRequest request = new() { DownloadId = 1, DownloadType = "torrent" };

		// Act
		TorBoxResponse<IntegrationJob> result = await client.CreateDropboxJobAsync(request);

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("integration/dropbox", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
	}

	[Fact]
	public async Task CreateOnedriveJobAsync_WithValidRequest_SendsPostRequest()
	{
		// Arrange
		(IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobJson);
		CreateIntegrationJobRequest request = new() { DownloadId = 1, DownloadType = "torrent" };

		// Act
		TorBoxResponse<IntegrationJob> result = await client.CreateOnedriveJobAsync(request);

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("integration/onedrive", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
	}

	[Fact]
	public async Task CreateGofileJobAsync_WithValidRequest_SendsPostRequest()
	{
		// Arrange
		(IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobJson);
		CreateIntegrationJobRequest request = new() { DownloadId = 1, DownloadType = "torrent" };

		// Act
		TorBoxResponse<IntegrationJob> result = await client.CreateGofileJobAsync(request);

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("integration/gofile", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
	}

	[Fact]
	public async Task CreateOneFichierJobAsync_WithValidRequest_SendsPostRequest()
	{
		// Arrange
		(IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobJson);
		CreateIntegrationJobRequest request = new() { DownloadId = 1, DownloadType = "torrent" };

		// Act
		TorBoxResponse<IntegrationJob> result = await client.CreateOneFichierJobAsync(request);

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("integration/1fichier", handler.LastRequest.RequestUri!.ToString());
		Assert.True(result.Success);
	}

	[Fact]
	public async Task CreatePixeldrainJobAsync_WithValidRequest_SendsPostRequest()
	{
		// Arrange
		(IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(JobJson);
		CreateIntegrationJobRequest request = new() { DownloadId = 1, DownloadType = "torrent" };

		// Act
		TorBoxResponse<IntegrationJob> result = await client.CreatePixeldrainJobAsync(request);

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("integration/pixeldrain", handler.LastRequest.RequestUri!.ToString());
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
	public async Task GetJobsAsync_WithNoParameters_SendsGetRequest()
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

	[Fact]
	public async Task OAuthRedirectAsync_WithEmptyProvider_ThrowsArgumentException()
	{
		// Arrange
		(IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(StringDataJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.OAuthRedirectAsync(""));
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

	[Fact]
	public async Task OAuthCallbackAsync_WithEmptyProvider_ThrowsArgumentException()
	{
		// Arrange
		(IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(ObjectJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.OAuthCallbackAsync(""));
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

	[Fact]
	public async Task OAuthSuccessAsync_WithEmptyProvider_ThrowsArgumentException()
	{
		// Arrange
		(IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(ObjectJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.OAuthSuccessAsync(""));
	}

	// --- OAuthRegisterAsync ---

	[Fact]
	public async Task OAuthRegisterAsync_WithValidRequest_SendsPostRequest()
	{
		// Arrange
		(IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);
		OAuthRegisterRequest request = new() { Provider = "googledrive" };

		// Act
		await client.OAuthRegisterAsync(request);

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("integration/oauth/googledrive/register", handler.LastRequest.RequestUri!.ToString());
	}

	[Fact]
	public async Task OAuthRegisterAsync_WithEmptyProvider_ThrowsArgumentException()
	{
		// Arrange
		(IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);
		OAuthRegisterRequest request = new() { Provider = "" };

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.OAuthRegisterAsync(request));
	}

	[Fact]
	public async Task OAuthRegisterAsync_WithNullRequest_ThrowsArgumentNullException()
	{
		// Arrange
		(IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => client.OAuthRegisterAsync(null!));
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

	[Fact]
	public async Task OAuthUnregisterAsync_WithEmptyProvider_ThrowsArgumentException()
	{
		// Arrange
		(IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(SuccessJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.OAuthUnregisterAsync(""));
	}

	// --- GetLinkedDiscordRolesAsync ---

	[Fact]
	public async Task GetLinkedDiscordRolesAsync_WithValidRequest_SendsPostRequest()
	{
		// Arrange
		(IntegrationsClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<IntegrationsClient>(ObjectJson);
		LinkedRolesRequest request = new() { DiscordToken = "test-token" };

		// Act
		await client.GetLinkedDiscordRolesAsync(request);

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
		Assert.Contains("integration/oauth/discord/linked_roles", handler.LastRequest.RequestUri!.ToString());
	}

	[Fact]
	public async Task GetLinkedDiscordRolesAsync_WithNullRequest_ThrowsArgumentNullException()
	{
		// Arrange
		(IntegrationsClient client, _) = ClientTestBase.CreateClient<IntegrationsClient>(ObjectJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetLinkedDiscordRolesAsync(null!));
	}
}
