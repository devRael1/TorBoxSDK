using TorboxSDK.UnitTests.Helpers;
using TorBoxSDK.Main.Stream;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Stream;

namespace TorboxSDK.UnitTests.Main.Stream;

public sealed class StreamClientTests
{
	private const string StreamUrlJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": "https://stream.torbox.app/abc123/stream.m3u8"
        }
        """;

	private const string StreamDataJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": {
                "hls_url": "https://stream.torbox.app/abc123/stream.m3u8",
                "domain": "https://stream.torbox.app",
                "presigned_token": "pre-signed-token-abc",
                "subtitle_index": 0,
                "audio_index": 1,
                "resolution_index": 2,
                "file_token": "file-token-xyz",
                "token": "auth-token-123",
                "is_transcoding": false,
                "needs_transcoding": true,
                "metadata": {
                    "video": {},
                    "audios": [],
                    "subtitles": [],
                    "thumbnail": "thumbnail.png",
                    "chapters": "chapters.vtt"
                },
                "search_metadata": {}
            }
        }
        """;

	[Fact]
	public async Task CreateStreamAsync_WithRequiredParams_SendsCorrectUrl()
	{
		// Arrange
		(StreamClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<StreamClient>(StreamUrlJson);

		// Act
		TorBoxResponse<string> result = await client.CreateStreamAsync(42, 5, "torrent");

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
		string url = handler.LastRequest.RequestUri!.ToString();
		Assert.Contains("stream/createstream", url);
		Assert.Contains("id=42", url);
		Assert.Contains("file_id=5", url);
		Assert.Contains("type=torrent", url);
		Assert.True(result.Success);
	}

	[Fact]
	public async Task CreateStreamAsync_WithAllOptionalParams_IncludesInQueryString()
	{
		// Arrange
		(StreamClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<StreamClient>(StreamUrlJson);

		// Act
		await client.CreateStreamAsync(1, 2, "usenet", new CreateStreamOptions { ChosenSubtitleIndex = 3, ChosenAudioIndex = 1, ChosenResolutionIndex = 720 });

		// Assert
		Assert.NotNull(handler.LastRequest);
		string url = handler.LastRequest.RequestUri!.ToString();
		Assert.Contains("chosen_subtitle_index=3", url);
		Assert.Contains("chosen_audio_index=1", url);
		Assert.Contains("chosen_resolution_index=720", url);
	}

	[Fact]
	public async Task CreateStreamAsync_WithNullOptionalParams_OmitsFromQueryString()
	{
		// Arrange
		(StreamClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<StreamClient>(StreamUrlJson);

		// Act
		await client.CreateStreamAsync(1, 2, "torrent");

		// Assert
		Assert.NotNull(handler.LastRequest);
		string url = handler.LastRequest.RequestUri!.ToString();
		Assert.DoesNotContain("chosen_subtitle_index", url);
		Assert.DoesNotContain("chosen_audio_index", url);
		Assert.DoesNotContain("chosen_resolution_index", url);
	}

	[Fact]
	public async Task CreateStreamAsync_WithEmptyType_ThrowsArgumentException()
	{
		// Arrange
		(StreamClient client, _) = ClientTestBase.CreateClient<StreamClient>(StreamUrlJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.CreateStreamAsync(1, 2, string.Empty));
	}

	[Fact]
	public async Task GetStreamDataAsync_WithRequiredParams_SendsCorrectUrl()
	{
		// Arrange
		(StreamClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

		// Act
		TorBoxResponse<StreamData> result = await client.GetStreamDataAsync("pre-signed-token-abc", "auth-token-123");

		// Assert
		Assert.NotNull(handler.LastRequest);
		Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
		string url = handler.LastRequest.RequestUri!.ToString();
		Assert.Contains("stream/getstreamdata", url);
		Assert.Contains("presigned_token=pre-signed-token-abc", url);
		Assert.Contains("token=auth-token-123", url);
		Assert.True(result.Success);
		Assert.NotNull(result.Data);
		Assert.Equal("https://stream.torbox.app/abc123/stream.m3u8", result.Data!.HlsUrl);
		Assert.Equal("https://stream.torbox.app", result.Data.Domain);
		Assert.Equal("file-token-xyz", result.Data.FileToken);
		Assert.False(result.Data.IsTranscoding);
		Assert.True(result.Data.NeedsTranscoding);
		Assert.NotNull(result.Data.Metadata);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithAllOptionalParams_IncludesInQueryString()
	{
		// Arrange
		(StreamClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

		// Act
		await client.GetStreamDataAsync("presigned", "token", new GetStreamDataOptions { ChosenSubtitleIndex = 0, ChosenAudioIndex = 2, ChosenResolutionIndex = 1080 });

		// Assert
		Assert.NotNull(handler.LastRequest);
		string url = handler.LastRequest.RequestUri!.ToString();
		Assert.Contains("chosen_subtitle_index=0", url);
		Assert.Contains("chosen_audio_index=2", url);
		Assert.Contains("chosen_resolution_index=1080", url);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithNullOptionalParams_OmitsFromQueryString()
	{
		// Arrange
		(StreamClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

		// Act
		await client.GetStreamDataAsync("presigned", "token");

		// Assert
		Assert.NotNull(handler.LastRequest);
		string url = handler.LastRequest.RequestUri!.ToString();
		Assert.DoesNotContain("chosen_subtitle_index", url);
		Assert.DoesNotContain("chosen_audio_index", url);
		Assert.DoesNotContain("chosen_resolution_index", url);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithNullPresignedToken_ThrowsArgumentNullException()
	{
		// Arrange
		(StreamClient client, _) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetStreamDataAsync(null!, "token"));
	}

	[Fact]
	public async Task GetStreamDataAsync_WithEmptyPresignedToken_ThrowsArgumentException()
	{
		// Arrange
		(StreamClient client, _) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.GetStreamDataAsync(string.Empty, "token"));
	}

	[Fact]
	public async Task GetStreamDataAsync_WithEmptyToken_ThrowsArgumentException()
	{
		// Arrange
		(StreamClient client, _) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => client.GetStreamDataAsync("presigned", string.Empty));
	}
}
