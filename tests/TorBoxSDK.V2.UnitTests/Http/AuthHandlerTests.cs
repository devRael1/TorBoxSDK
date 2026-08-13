using System.Net;
using TorBoxSDK.Http.Handlers;
using TorBoxSDK.V2.Testing;

namespace TorBoxSDK.V2.UnitTests.Http;

public sealed class AuthHandlerTests
{
	[Fact]
	public async Task SendAsync_WithApiKey_AddsExactlyOneBearerAuthorizationHeader()
	{
		// Arrange
		RecordingHttpMessageHandler innerHandler = RecordingHttpMessageHandler.Json(
			HttpStatusCode.OK,
			"""
			{
			  "success": true,
			  "error": null,
			  "detail": "Found."
			}
			""");
		using AuthHandler authHandler = new("test-api-key");
		authHandler.InnerHandler = innerHandler;
		using HttpClient client = new(authHandler, disposeHandler: false);
		using HttpRequestMessage request = new(HttpMethod.Get, "https://api.torbox.app/v1/api/user/me");
		request.Headers.TryAddWithoutValidation("Authorization", "Bearer stale-key");

		// Act
		using HttpResponseMessage response = await client.SendAsync(request, CancellationToken.None);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Equal(["Bearer test-api-key"], innerHandler.LastAuthorizationHeaderValues);
		Assert.Equal(1, innerHandler.SendCount);
	}

	[Fact]
	public async Task SendAsync_WithBlankApiKey_RejectsBeforeSendingAndDoesNotLeakTheValue()
	{
		// Arrange
		const string BlankApiKey = "\u2003";
		RecordingHttpMessageHandler innerHandler = RecordingHttpMessageHandler.Json(HttpStatusCode.OK, "{\"success\":true}");
		using AuthHandler authHandler = new(BlankApiKey);
		authHandler.InnerHandler = innerHandler;
		using HttpClient client = new(authHandler, disposeHandler: false);
		using HttpRequestMessage request = new(HttpMethod.Get, "https://api.torbox.app/v1/api/user/me");

		// Act
		InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
			() => client.SendAsync(request, CancellationToken.None));

		// Assert
		Assert.Equal(0, innerHandler.SendCount);
		Assert.DoesNotContain(BlankApiKey, exception.Message, StringComparison.Ordinal);
	}
}
