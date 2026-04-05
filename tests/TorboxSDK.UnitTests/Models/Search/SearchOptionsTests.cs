using System.Text.Json;

using TorBoxSDK.Http;
using TorBoxSDK.Models.Search;

namespace TorboxSDK.UnitTests.Models.Search;

public sealed class SearchOptionsTests
{
    [Fact]
    public void TorrentSearchOptions_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        var options = new TorrentSearchOptions
        {
            Metadata = true,
            Season = 3,
            Episode = 12,
            CheckCache = true,
            CheckOwned = false,
            SearchUserEngines = true,
            CachedOnly = false,
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.True(root.GetProperty("metadata").GetBoolean());
        Assert.Equal(3, root.GetProperty("season").GetInt32());
        Assert.Equal(12, root.GetProperty("episode").GetInt32());
        Assert.True(root.GetProperty("check_cache").GetBoolean());
        Assert.False(root.GetProperty("check_owned").GetBoolean());
        Assert.True(root.GetProperty("search_user_engines").GetBoolean());
        Assert.False(root.GetProperty("cached_only").GetBoolean());
    }

    [Fact]
    public void TorrentSearchOptions_Serialize_WithNullProperties_OmitsThem()
    {
        // Arrange
        var options = new TorrentSearchOptions
        {
            CheckCache = true,
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.True(root.GetProperty("check_cache").GetBoolean());
        Assert.False(root.TryGetProperty("metadata", out _));
        Assert.False(root.TryGetProperty("season", out _));
        Assert.False(root.TryGetProperty("episode", out _));
        Assert.False(root.TryGetProperty("check_owned", out _));
        Assert.False(root.TryGetProperty("search_user_engines", out _));
        Assert.False(root.TryGetProperty("cached_only", out _));
    }

    [Fact]
    public void UsenetSearchOptions_Serialize_WithAllProperties_ProducesExpectedJson()
    {
        // Arrange
        var options = new UsenetSearchOptions
        {
            Metadata = false,
            Season = 1,
            Episode = 5,
            CheckCache = true,
            CheckOwned = true,
            SearchUserEngines = false,
            CachedOnly = true,
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.GetProperty("metadata").GetBoolean());
        Assert.Equal(1, root.GetProperty("season").GetInt32());
        Assert.Equal(5, root.GetProperty("episode").GetInt32());
        Assert.True(root.GetProperty("check_cache").GetBoolean());
        Assert.True(root.GetProperty("check_owned").GetBoolean());
        Assert.False(root.GetProperty("search_user_engines").GetBoolean());
        Assert.True(root.GetProperty("cached_only").GetBoolean());
    }

    [Fact]
    public void UsenetSearchOptions_Serialize_WithNullProperties_OmitsThem()
    {
        // Arrange
        var options = new UsenetSearchOptions();

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("metadata", out _));
        Assert.False(root.TryGetProperty("season", out _));
        Assert.False(root.TryGetProperty("episode", out _));
        Assert.False(root.TryGetProperty("check_cache", out _));
        Assert.False(root.TryGetProperty("check_owned", out _));
        Assert.False(root.TryGetProperty("search_user_engines", out _));
        Assert.False(root.TryGetProperty("cached_only", out _));
    }

    [Fact]
    public void MetaSearchOptions_Serialize_WithType_ProducesExpectedJson()
    {
        // Arrange
        var options = new MetaSearchOptions
        {
            Type = "movie",
        };

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.Equal("movie", root.GetProperty("type").GetString());
    }

    [Fact]
    public void MetaSearchOptions_Serialize_WithNullType_OmitsProperty()
    {
        // Arrange
        var options = new MetaSearchOptions();

        // Act
        string json = JsonSerializer.Serialize(options, TorBoxJsonOptions.Default);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        Assert.False(root.TryGetProperty("type", out _));
    }

    [Fact]
    public void TorrentSearchOptions_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "metadata": true,
                "season": 2,
                "episode": 8,
                "check_cache": false,
                "check_owned": true,
                "search_user_engines": true,
                "cached_only": false
            }
            """;

        // Act
        TorrentSearchOptions? result = JsonSerializer.Deserialize<TorrentSearchOptions>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Metadata);
        Assert.Equal(2, result.Season);
        Assert.Equal(8, result.Episode);
        Assert.False(result.CheckCache);
        Assert.True(result.CheckOwned);
        Assert.True(result.SearchUserEngines);
        Assert.False(result.CachedOnly);
    }

    [Fact]
    public void UsenetSearchOptions_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "metadata": false,
                "season": 4,
                "episode": 1,
                "check_cache": true,
                "check_owned": false,
                "search_user_engines": false,
                "cached_only": true
            }
            """;

        // Act
        UsenetSearchOptions? result = JsonSerializer.Deserialize<UsenetSearchOptions>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Metadata);
        Assert.Equal(4, result.Season);
        Assert.Equal(1, result.Episode);
        Assert.True(result.CheckCache);
        Assert.False(result.CheckOwned);
        Assert.False(result.SearchUserEngines);
        Assert.True(result.CachedOnly);
    }
}
