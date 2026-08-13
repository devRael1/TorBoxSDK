using System.Net;
using System.Text.Json;
using TorBoxSDK.Http;
using TorBoxSDK.Main.Stream;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Stream;
using TorBoxSDK.V2.Testing;

namespace TorBoxSDK.V2.UnitTests.Main.Stream;

public sealed class StreamClientTests
{
	[Fact]
	public async Task CreateStreamAsync_WithNullRequest_ThrowsBeforeSendingARequest()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(SuccessfulCreateStreamJson);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		Func<Task> action = () => client.CreateStreamAsync(null!);

		// Act
		ArgumentNullException exception = await Assert.ThrowsAsync<ArgumentNullException>(action);

		// Assert
		Assert.Equal("request", exception.ParamName);
		Assert.Equal(0, handler.SendCount);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithNullRequest_ThrowsBeforeSendingARequest()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(SuccessfulStreamDataJson);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		Func<Task> action = () => client.GetStreamDataAsync(null!);

		// Act
		ArgumentNullException exception = await Assert.ThrowsAsync<ArgumentNullException>(action);

		// Assert
		Assert.Equal("request", exception.ParamName);
		Assert.Equal(0, handler.SendCount);
	}

	[Fact]
	public async Task CreateStreamAsync_WithWhitespaceType_ThrowsBeforeSendingARequest()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(SuccessfulCreateStreamJson);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		CreateStreamRequest request = new() { DownloadId = 42, Type = " \t" };
		Func<Task> action = () => client.CreateStreamAsync(request);

		// Act
		ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(action);

		// Assert
		Assert.Equal("request", exception.ParamName);
		Assert.Equal(0, handler.SendCount);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithWhitespacePresignedToken_ThrowsBeforeSendingARequest()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(SuccessfulStreamDataJson);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		GetStreamDataRequest request = new() { PresignedToken = " \t", Token = "token" };
		Func<Task> action = () => client.GetStreamDataAsync(request);

		// Act
		ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(action);

		// Assert
		Assert.Equal("request", exception.ParamName);
		Assert.Equal(0, handler.SendCount);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithWhitespaceToken_ThrowsBeforeSendingARequest()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(SuccessfulStreamDataJson);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		GetStreamDataRequest request = new() { PresignedToken = "presigned", Token = " \t" };
		Func<Task> action = () => client.GetStreamDataAsync(request);

		// Act
		ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(action);

		// Assert
		Assert.Equal("request", exception.ParamName);
		Assert.Equal(0, handler.SendCount);
	}

	[Fact]
	public async Task CreateStreamAsync_WithOnlyDownloadId_OmitsAllTorBoxOwnedDefaults()
	{
		// Arrange
		HttpMethod? observedMethod = null;
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.FromResponse(
			(request, cancellationToken) =>
			{
				observedMethod = request.Method;
				return CreateJsonResponse(HttpStatusCode.OK, SuccessfulCreateStreamJson);
			});
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		CreateStreamRequest request = new() { DownloadId = 42 };

		// Act
		TorBoxResponse<string> response = await client.CreateStreamAsync(request);

		// Assert
		Assert.True(response.Success);
		Assert.Equal(HttpMethod.Get, observedMethod);
		Assert.NotNull(handler.LastRequestUri);
		Assert.EndsWith("stream/createstream?id=42", handler.LastRequestUri.AbsoluteUri, StringComparison.Ordinal);
		Assert.Empty(handler.LastAuthorizationHeaderValues);
	}

	[Fact]
	public async Task CreateStreamAsync_WithExplicitOptions_UsesInvariantEncodedQueryValues()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(SuccessfulCreateStreamJson);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		CreateStreamRequest request = new()
		{
			DownloadId = 42,
			FileId = -7,
			Type = "usenet & test",
			ChosenSubtitleIndex = 0,
			ChosenAudioIndex = 2,
			ChosenResolutionIndex = 1080,
			ScrobblingEnabled = false,
		};

		// Act
		TorBoxResponse<string> response = await client.CreateStreamAsync(request);

		// Assert
		Assert.True(response.Success);
		Assert.NotNull(handler.LastRequestUri);
		Assert.EndsWith(
			"stream/createstream?id=42&file_id=-7&type=usenet%20%26%20test&chosen_subtitle_index=0&chosen_audio_index=2&chosen_resolution_index=1080&scrobbling_enabled=false",
			handler.LastRequestUri.AbsoluteUri,
			StringComparison.Ordinal);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithTokensAndOptions_UsesNamedEncodedQueryValues()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(SuccessfulStreamDataJson);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		GetStreamDataRequest request = new()
		{
			PresignedToken = "pre+/ ?&",
			Token = "token+/ ?&",
			ChosenSubtitleIndex = 0,
			ChosenAudioIndex = 2,
			ChosenResolutionIndex = 1080,
		};

		// Act
		TorBoxResponse<StreamData> response = await client.GetStreamDataAsync(request);

		// Assert
		Assert.True(response.Success);
		Assert.NotNull(handler.LastRequestUri);
		Assert.EndsWith(
			"stream/getstreamdata?presigned_token=pre%2B%2F%20%3F%26&token=token%2B%2F%20%3F%26&chosen_subtitle_index=0&chosen_audio_index=2&chosen_resolution_index=1080",
			handler.LastRequestUri.AbsoluteUri,
			StringComparison.Ordinal);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithNestedJson_ReturnsTypedDataAndRawJsonElements()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(SuccessfulStreamDataJson);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		GetStreamDataRequest request = new() { PresignedToken = "presigned", Token = "token" };

		// Act
		TorBoxResponse<StreamData> response = await client.GetStreamDataAsync(request);

		// Assert
		Assert.True(response.Success);
		Assert.NotNull(response.Data);
		Assert.Equal("https://stream.example.test/playlist.m3u8", response.Data.HlsUrl);
		Assert.NotNull(response.Data.Metadata);
		Assert.Single(response.Data.Metadata.Audios);
		Assert.Equal("English", response.Data.Metadata.Audios[0].LanguageFull);
		Assert.Single(response.Data.Metadata.Subtitles);
		Assert.Equal("French", response.Data.Metadata.Subtitles[0].LanguageFull);
		Assert.Equal(JsonValueKind.Object, response.Data.Metadata.Video?.ValueKind);
		Assert.Equal(JsonValueKind.Array, response.Data.SearchMetadata?.ValueKind);
	}

	[Fact]
	public async Task CreateStreamAsync_WithStructuredFailure_ReturnsFailureEnvelope()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(FailureJson, HttpStatusCode.Unauthorized);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		CreateStreamRequest request = new() { DownloadId = 42 };

		// Act
		TorBoxResponse<string> response = await client.CreateStreamAsync(request);

		// Assert
		Assert.False(response.Success);
		Assert.Equal("BAD_TOKEN", response.Error);
		Assert.Equal("Invalid token.", response.Detail);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithStructuredFailure_ReturnsFailureEnvelope()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(FailureJson, HttpStatusCode.Unauthorized);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		GetStreamDataRequest request = new() { PresignedToken = "presigned", Token = "token" };

		// Act
		TorBoxResponse<StreamData> response = await client.GetStreamDataAsync(request);

		// Assert
		Assert.False(response.Success);
		Assert.Equal("BAD_TOKEN", response.Error);
		Assert.Equal("Invalid token.", response.Detail);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task GetStreamDataAsync_WithCancellationToken_ForwardsItToTheHandler()
	{
		// Arrange
		RecordingHttpMessageHandler handler = CreateJsonHandler(SuccessfulStreamDataJson);
		using HttpClient httpClient = CreateHttpClient(handler);
		StreamClient client = new(httpClient, new TorBoxApiTransport());
		using CancellationTokenSource source = new();
		GetStreamDataRequest request = new() { PresignedToken = "presigned", Token = "token" };
		source.Cancel();

		// Act
		OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
			() => client.GetStreamDataAsync(request, source.Token));

		// Assert
		Assert.Equal(source.Token, exception.CancellationToken);
		Assert.True(handler.LastCancellationToken.IsCancellationRequested);
	}

	private const string SuccessfulCreateStreamJson = """
		{
		  "success": true,
		  "error": null,
		  "detail": "Created.",
		  "data": "https://stream.example.test/playlist.m3u8"
		}
		""";

	private const string SuccessfulStreamDataJson = """
		{
		  "success": true,
		  "error": null,
		  "detail": "Found.",
		  "data": {
		    "hls_url": "https://stream.example.test/playlist.m3u8",
		    "domain": "stream.example.test",
		    "presigned_token": "presigned",
		    "subtitle_index": 3,
		    "audio_index": 1,
		    "resolution_index": 0,
		    "file_token": "file-token",
		    "token": "token",
		    "is_transcoding": false,
		    "needs_transcoding": true,
		    "metadata": {
		      "video": { "codec": "h264" },
		      "audios": [
		        {
		          "index": 1,
		          "language": "en",
		          "language_full": "English",
		          "codec": "aac",
		          "codec_type": "audio",
		          "default": true,
		          "sample_rate": "48000",
		          "channels": 2,
		          "channel_layout": "stereo",
		          "title": "Main"
		        }
		      ],
		      "subtitles": [
		        {
		          "index": 3,
		          "language": "fr",
		          "language_full": "French",
		          "title": "French"
		        }
		      ],
		      "thumbnail": "thumbnail.jpg",
		      "chapters": "chapters.vtt"
		    },
		    "search_metadata": [ { "title": "Example" } ]
		  }
		}
		""";

	private const string FailureJson = """
		{
		  "success": false,
		  "error": "BAD_TOKEN",
		  "detail": "Invalid token.",
		  "data": null
		}
		""";

	private static HttpClient CreateHttpClient(HttpMessageHandler handler) => new(handler)
	{
		BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
	};

	private static RecordingHttpMessageHandler CreateJsonHandler(string json, HttpStatusCode statusCode = HttpStatusCode.OK) =>
		RecordingHttpMessageHandler.Json(statusCode, json);

	private static HttpResponseMessage CreateJsonResponse(HttpStatusCode statusCode, string json) => new(statusCode)
	{
		Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
	};
}
