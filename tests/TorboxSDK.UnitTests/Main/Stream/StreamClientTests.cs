using TorBoxSDK.Main.Stream;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Stream;
using TorboxSDK.UnitTests.Helpers;

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
                "url": "https://stream.torbox.app/play/abc123",
                "subtitles": [],
                "audio_tracks": []
            }
        }
        """;

    [Fact]
    public async Task CreateStreamAsync_WithRequiredParams_SendsCorrectUrl()
    {
        // Arrange
        (StreamClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<StreamClient>(StreamUrlJson);

        // Act
        TorBoxResponse<string> result = await client.CreateStreamAsync(new CreateStreamOptions { Id = 42, FileId = 5, Type = "torrent" });

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
        await client.CreateStreamAsync(new CreateStreamOptions { Id = 1, FileId = 2, Type = "usenet", ChosenSubtitleIndex = 3, ChosenAudioIndex = 1, ChosenResolutionIndex = 720 });

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
        await client.CreateStreamAsync(new CreateStreamOptions { Id = 1, FileId = 2, Type = "torrent" });

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.DoesNotContain("chosen_subtitle_index", url);
        Assert.DoesNotContain("chosen_audio_index", url);
        Assert.DoesNotContain("chosen_resolution_index", url);
    }

    [Fact]
    public async Task CreateStreamAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        (StreamClient client, _) = ClientTestBase.CreateClient<StreamClient>(StreamUrlJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CreateStreamAsync(null!));
    }

    [Fact]
    public async Task CreateStreamAsync_WithEmptyType_ThrowsArgumentException()
    {
        // Arrange
        (StreamClient client, _) = ClientTestBase.CreateClient<StreamClient>(StreamUrlJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.CreateStreamAsync(new CreateStreamOptions { Id = 1, FileId = 2, Type = string.Empty }));
    }

    [Fact]
    public async Task GetStreamDataAsync_WithRequiredParams_SendsCorrectUrl()
    {
        // Arrange
        (StreamClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

        // Act
        TorBoxResponse<object> result = await client.GetStreamDataAsync(new GetStreamDataOptions { PresignedToken = "pre-signed-token-abc", Token = "auth-token-123" });

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("stream/getstreamdata", url);
        Assert.Contains("presigned_token=pre-signed-token-abc", url);
        Assert.Contains("token=auth-token-123", url);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetStreamDataAsync_WithAllOptionalParams_IncludesInQueryString()
    {
        // Arrange
        (StreamClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

        // Act
        await client.GetStreamDataAsync(new GetStreamDataOptions { PresignedToken = "presigned", Token = "token", ChosenSubtitleIndex = 0, ChosenAudioIndex = 2, ChosenResolutionIndex = 1080 });

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
        await client.GetStreamDataAsync(new GetStreamDataOptions { PresignedToken = "presigned", Token = "token" });

        // Assert
        Assert.NotNull(handler.LastRequest);
        string url = handler.LastRequest.RequestUri!.ToString();
        Assert.DoesNotContain("chosen_subtitle_index", url);
        Assert.DoesNotContain("chosen_audio_index", url);
        Assert.DoesNotContain("chosen_resolution_index", url);
    }

    [Fact]
    public async Task GetStreamDataAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        (StreamClient client, _) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetStreamDataAsync(null!));
    }

    [Fact]
    public async Task GetStreamDataAsync_WithEmptyPresignedToken_ThrowsArgumentException()
    {
        // Arrange
        (StreamClient client, _) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetStreamDataAsync(new GetStreamDataOptions { PresignedToken = string.Empty, Token = "token" }));
    }

    [Fact]
    public async Task GetStreamDataAsync_WithEmptyToken_ThrowsArgumentException()
    {
        // Arrange
        (StreamClient client, _) = ClientTestBase.CreateClient<StreamClient>(StreamDataJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetStreamDataAsync(new GetStreamDataOptions { PresignedToken = "presigned", Token = string.Empty }));
    }
}
