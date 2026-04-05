using TorBoxSDK.Main.Rss;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Rss;
using TorboxSDK.UnitTests.Helpers;

namespace TorboxSDK.UnitTests.Main.Rss;

public sealed class RssClientTests
{
    private const string SuccessJson = """
        {
            "success": true,
            "error": null,
            "detail": "OK."
        }
        """;

    private const string FeedsJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "name": "Ubuntu Releases",
                    "url": "https://example.com/rss"
                }
            ]
        }
        """;

    private const string FeedItemsJson = """
        {
            "success": true,
            "error": null,
            "detail": "Found.",
            "data": [
                {
                    "id": 1,
                    "title": "Ubuntu 24.04",
                    "link": "https://example.com/item/1"
                }
            ]
        }
        """;

    // --- AddRssAsync ---

    [Fact]
    public async Task AddRssAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (RssClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<RssClient>(SuccessJson);
        AddRssRequest request = new() { Url = "https://example.com/rss" };

        // Act
        TorBoxResponse result = await client.AddRssAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("rss/addrss", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task AddRssAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (RssClient client, _) = ClientTestBase.CreateClient<RssClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.AddRssAsync(null!));
    }

    // --- ControlRssAsync ---

    [Fact]
    public async Task ControlRssAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (RssClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<RssClient>(SuccessJson);
        ControlRssRequest request = new() { RssFeedId = 1, Operation = ControlOperation.Delete };

        // Act
        TorBoxResponse result = await client.ControlRssAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("rss/controlrss", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ControlRssAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (RssClient client, _) = ClientTestBase.CreateClient<RssClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.ControlRssAsync(null!));
    }

    // --- ModifyRssAsync ---

    [Fact]
    public async Task ModifyRssAsync_WithValidRequest_SendsPostRequest()
    {
        // Arrange
        (RssClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<RssClient>(SuccessJson);
        ModifyRssRequest request = new() { RssFeedId = 1, Name = "Updated Feed" };

        // Act
        TorBoxResponse result = await client.ModifyRssAsync(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("rss/modifyrss", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ModifyRssAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        (RssClient client, _) = ClientTestBase.CreateClient<RssClient>(SuccessJson);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.ModifyRssAsync(null!));
    }

    // --- GetFeedsAsync ---

    [Fact]
    public async Task GetFeedsAsync_SendsGetRequest()
    {
        // Arrange
        (RssClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<RssClient>(FeedsJson);

        // Act
        TorBoxResponse<IReadOnlyList<RssFeed>> result = await client.GetFeedsAsync();

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("rss/getfeeds", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    // --- GetFeedItemsAsync ---

    [Fact]
    public async Task GetFeedItemsAsync_WithFeedId_SendsGetRequest()
    {
        // Arrange
        (RssClient client, MockHttpMessageHandler handler) = ClientTestBase.CreateClient<RssClient>(FeedItemsJson);

        // Act
        TorBoxResponse<IReadOnlyList<RssFeedItem>> result = await client.GetFeedItemsAsync(42);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Contains("rss/getfeeditems", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("rss_feed_id=42", handler.LastRequest.RequestUri!.ToString());
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }
}
