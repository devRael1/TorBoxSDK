using TorboxSDK.UnitTests.Helpers;
using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;

namespace TorboxSDK.UnitTests.Http;

public sealed class TorBoxApiHelperTests
{
	[Fact]
	public void BuildQuery_WithAllValues_ReturnsCorrectString()
	{
		// Arrange & Act
		string result = TorBoxApiHelper.BuildQuery(
			("id", "42"),
			("name", "test"),
			("active", "true"));

		// Assert
		Assert.Equal("?id=42&name=test&active=true", result);
	}

	[Fact]
	public void BuildQuery_WithNullValues_SkipsNulls()
	{
		// Arrange & Act
		string result = TorBoxApiHelper.BuildQuery(
			("id", "42"),
			("name", null),
			("active", "true"));

		// Assert
		Assert.Equal("?id=42&active=true", result);
	}

	[Fact]
	public void BuildQuery_WithNoValues_ReturnsEmpty()
	{
		// Arrange & Act
		string result = TorBoxApiHelper.BuildQuery();

		// Assert
		Assert.Equal(string.Empty, result);
	}

	[Fact]
	public async Task SendAsync_WithSuccessResponse_ReturnsData()
	{
		// Arrange
		string json = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "https://download.torbox.app/file"
            }
            """;
		var handler = new MockHttpMessageHandler(json);
		using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.torbox.app/v1/api/") };
		using var request = new HttpRequestMessage(HttpMethod.Get, "torrents/requestdl?torrent_id=1");

		// Act
		TorBoxResponse<string> result = await TorBoxApiHelper.SendAsync<string>(httpClient, request, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.Success);
		Assert.Equal("https://download.torbox.app/file", result.Data);
	}

	[Fact]
	public async Task SendAsync_WithErrorResponse_ThrowsTorBoxException()
	{
		// Arrange
		string json = """
            {
                "success": false,
                "error": "BAD_TOKEN",
                "detail": "The provided token is invalid.",
                "data": null
            }
            """;
		var handler = new MockHttpMessageHandler(json);
		using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.torbox.app/v1/api/") };
		using var request = new HttpRequestMessage(HttpMethod.Get, "torrents/mylist");

		// Act & Assert
		TorBoxException ex = await Assert.ThrowsAsync<TorBoxException>(
			() => TorBoxApiHelper.SendAsync<string>(httpClient, request, CancellationToken.None));
		Assert.Equal(TorBoxErrorCode.BadToken, ex.ErrorCode);
		Assert.Equal("The provided token is invalid.", ex.Detail);
	}
}
