using TorBoxSDK.Models.General;

namespace TorboxSDK.UnitTests.Models.General;

public sealed class ChangelogsRssItemTests
{
    [Fact]
    public void ChangelogsRssItem_Create_WithAllProperties_PopulatesCorrectly()
    {
        // Arrange & Act
        ChangelogsRssItem item = new()
        {
            Title = "v2.0.0 Release",
            Link = "https://torbox.app/changelog/v2",
            Description = "Major update with new features",
            PubDate = "Mon, 14 Apr 2026 10:00:00 GMT",
            ContentEncoded = "<p>Full release notes</p>",
        };

        // Assert
        Assert.Equal("v2.0.0 Release", item.Title);
        Assert.Equal("https://torbox.app/changelog/v2", item.Link);
        Assert.Equal("Major update with new features", item.Description);
        Assert.Equal("Mon, 14 Apr 2026 10:00:00 GMT", item.PubDate);
        Assert.Equal("<p>Full release notes</p>", item.ContentEncoded);
    }

    [Fact]
    public void ChangelogsRssItem_Create_WithDefaults_HasEmptyStrings()
    {
        // Arrange & Act
        ChangelogsRssItem item = new();

        // Assert
        Assert.Equal(string.Empty, item.Title);
        Assert.Equal(string.Empty, item.Link);
        Assert.Equal(string.Empty, item.Description);
        Assert.Equal(string.Empty, item.PubDate);
        Assert.Null(item.ContentEncoded);
    }
}
