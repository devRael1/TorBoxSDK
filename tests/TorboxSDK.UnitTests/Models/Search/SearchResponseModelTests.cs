using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Search;

namespace TorboxSDK.UnitTests.Models.Search;

public sealed class SearchResponseModelTests
{
    // ──── MetaTrailer ────

    [Fact]
    public void MetaTrailer_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "youtube_id": "dQw4w9WgXcQ",
                "full_url": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                "thumbnail": "https://img.youtube.com/vi/dQw4w9WgXcQ/maxresdefault.jpg"
            }
            """;

        // Act
        MetaTrailer? result = JsonSerializer.Deserialize<MetaTrailer>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("dQw4w9WgXcQ", result.YoutubeId);
        Assert.Equal("https://www.youtube.com/watch?v=dQw4w9WgXcQ", result.FullUrl);
        Assert.Equal("https://img.youtube.com/vi/dQw4w9WgXcQ/maxresdefault.jpg", result.Thumbnail);
    }

    [Fact]
    public void MetaTrailer_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {}
            """;

        // Act
        MetaTrailer? result = JsonSerializer.Deserialize<MetaTrailer>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.YoutubeId);
        Assert.Null(result.FullUrl);
        Assert.Null(result.Thumbnail);
    }

    // ──── MetaSearchResult ────

    [Fact]
    public void MetaSearchResult_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "globalID": "global-abc-123",
                "id": "imdb_id:tt0111161",
                "imdb_id": "tt0111161",
                "tmdb_id": 278,
                "tvdb_id": 289590,
                "tvmaze_id": 100,
                "trakt_id": 28,
                "mal_id": 5114,
                "anilist_id": 1535,
                "kitsu_id": 1555,
                "simkl_id": 987654,
                "title": "The Shawshank Redemption",
                "titles": ["Die Verurteilten", "Les Évadés"],
                "keywords": ["prison", "escape", "drama"],
                "genres": ["Drama", "Crime"],
                "actors": ["Tim Robbins", "Morgan Freeman"],
                "releaseYears": 1994,
                "mediaType": "movie",
                "characters": ["Andy Dufresne", "Red"],
                "link": "https://www.imdb.com/title/tt0111161/",
                "description": "Two imprisoned men bond over a number of years.",
                "rating": 9.3,
                "languages": ["en", "de", "fr"],
                "contentRating": "R",
                "trailer": {
                    "youtube_id": "6hB3S9bIaco",
                    "full_url": "https://www.youtube.com/watch?v=6hB3S9bIaco",
                    "thumbnail": "https://img.youtube.com/vi/6hB3S9bIaco/maxresdefault.jpg"
                },
                "image": "https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg"
            }
            """;

        // Act
        MetaSearchResult? result = JsonSerializer.Deserialize<MetaSearchResult>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("global-abc-123", result.GlobalId);
        Assert.Equal("imdb_id:tt0111161", result.Id);
        Assert.Equal("tt0111161", result.ImdbId);
        Assert.Equal(278L, result.TmdbId);
        Assert.Equal(289590L, result.TvdbId);
        Assert.Equal(100L, result.TvmazeId);
        Assert.Equal(28L, result.TraktId);
        Assert.Equal(5114L, result.MalId);
        Assert.Equal(1535L, result.AnilistId);
        Assert.Equal(1555L, result.KitsuId);
        Assert.Equal(987654L, result.SimklId);
        Assert.Equal("The Shawshank Redemption", result.Title);
        Assert.Equal(2, result.Titles.Count);
        Assert.Equal("Die Verurteilten", result.Titles[0]);
        Assert.Equal("Les Évadés", result.Titles[1]);
        Assert.Equal(3, result.Keywords.Count);
        Assert.Equal("prison", result.Keywords[0]);
        Assert.Equal(2, result.Genres.Count);
        Assert.Equal("Drama", result.Genres[0]);
        Assert.Equal("Crime", result.Genres[1]);
        Assert.Equal(2, result.Actors.Count);
        Assert.Equal("Tim Robbins", result.Actors[0]);
        Assert.Equal("Morgan Freeman", result.Actors[1]);
        Assert.Equal(1994, result.ReleaseYears);
        Assert.Equal("movie", result.MediaType);
        Assert.Equal(2, result.Characters.Count);
        Assert.Equal("Andy Dufresne", result.Characters[0]);
        Assert.Equal("Red", result.Characters[1]);
        Assert.Equal("https://www.imdb.com/title/tt0111161/", result.Link);
        Assert.Equal("Two imprisoned men bond over a number of years.", result.Description);
        Assert.Equal(9.3, result.Rating);
        Assert.Equal(3, result.Languages.Count);
        Assert.Equal("en", result.Languages[0]);
        Assert.Equal("R", result.ContentRating);
        Assert.NotNull(result.Trailer);
        Assert.Equal("6hB3S9bIaco", result.Trailer.YoutubeId);
        Assert.Equal("https://www.youtube.com/watch?v=6hB3S9bIaco", result.Trailer.FullUrl);
        Assert.Equal("https://img.youtube.com/vi/6hB3S9bIaco/maxresdefault.jpg", result.Trailer.Thumbnail);
        Assert.Equal("https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg", result.Image);
    }

    [Fact]
    public void MetaSearchResult_Deserialize_WithNullOptionals_DefaultsCorrectly()
    {
        // Arrange
        string json = """
            {}
            """;

        // Act
        MetaSearchResult? result = JsonSerializer.Deserialize<MetaSearchResult>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.GlobalId);
        Assert.Null(result.Id);
        Assert.Null(result.ImdbId);
        Assert.Null(result.TmdbId);
        Assert.Null(result.TvdbId);
        Assert.Null(result.TvmazeId);
        Assert.Null(result.TraktId);
        Assert.Null(result.MalId);
        Assert.Null(result.AnilistId);
        Assert.Null(result.KitsuId);
        Assert.Null(result.SimklId);
        Assert.Null(result.Title);
        Assert.Empty(result.Titles);
        Assert.Empty(result.Keywords);
        Assert.Empty(result.Genres);
        Assert.Empty(result.Actors);
        Assert.Null(result.ReleaseYears);
        Assert.Null(result.MediaType);
        Assert.Empty(result.Characters);
        Assert.Null(result.Link);
        Assert.Null(result.Description);
        Assert.Null(result.Rating);
        Assert.Empty(result.Languages);
        Assert.Null(result.ContentRating);
        Assert.Null(result.Trailer);
        Assert.Null(result.Image);
    }

    // ──── UsenetSearchResult ────

    [Fact]
    public void UsenetSearchResult_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "id": "nzb-abc-123",
                "name": "Ubuntu.24.04.LTS-amd64.nzb",
                "size": 4700000000,
                "source": "NZBGeek",
                "category": "PC > ISO",
                "nzb_link": "https://api.nzbgeek.info/api?t=get&id=abc123",
                "age": 30,
                "posted_at": "2025-05-01T14:00:00Z"
            }
            """;

        // Act
        UsenetSearchResult? result = JsonSerializer.Deserialize<UsenetSearchResult>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("nzb-abc-123", result.Id);
        Assert.Equal("Ubuntu.24.04.LTS-amd64.nzb", result.Name);
        Assert.Equal(4700000000L, result.Size);
        Assert.Equal("NZBGeek", result.Source);
        Assert.Equal("PC > ISO", result.Category);
        Assert.Equal("https://api.nzbgeek.info/api?t=get&id=abc123", result.NzbLink);
        Assert.Equal(30, result.Age);
        Assert.Equal(new DateTimeOffset(2025, 5, 1, 14, 0, 0, TimeSpan.Zero), result.PostedAt);
    }

    [Fact]
    public void UsenetSearchResult_Deserialize_WithNullOptionals_ReturnsNulls()
    {
        // Arrange
        string json = """
            {
                "size": 0
            }
            """;

        // Act
        UsenetSearchResult? result = JsonSerializer.Deserialize<UsenetSearchResult>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Id);
        Assert.Null(result.Name);
        Assert.Equal(0L, result.Size);
        Assert.Null(result.Source);
        Assert.Null(result.Category);
        Assert.Null(result.NzbLink);
        Assert.Null(result.Age);
        Assert.Null(result.PostedAt);
    }

    // ──── UsenetSearchResponse ────

    [Fact]
    public void UsenetSearchResponse_Deserialize_PopulatesAllProperties()
    {
        // Arrange
        string json = """
            {
                "metadata": {
                    "globalID": "meta-001",
                    "title": "Test Movie",
                    "mediaType": "movie"
                },
                "nzbs": [
                    {
                        "id": "nzb-1",
                        "name": "Test.Movie.2024.1080p.nzb",
                        "size": 1500000000,
                        "source": "NZBFinder",
                        "category": "Movies > HD",
                        "nzb_link": "https://nzbfinder.ws/getnzb/abc.nzb",
                        "age": 7,
                        "posted_at": "2025-05-25T18:30:00Z"
                    },
                    {
                        "id": "nzb-2",
                        "name": "Test.Movie.2024.720p.nzb",
                        "size": 800000000,
                        "source": "NZBGeek",
                        "category": "Movies > HD",
                        "nzb_link": "https://api.nzbgeek.info/getnzb/def.nzb",
                        "age": 14,
                        "posted_at": "2025-05-18T10:00:00Z"
                    }
                ]
            }
            """;

        // Act
        UsenetSearchResponse? result = JsonSerializer.Deserialize<UsenetSearchResponse>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Metadata);
        Assert.Equal("meta-001", result.Metadata.GlobalId);
        Assert.Equal("Test Movie", result.Metadata.Title);
        Assert.Equal("movie", result.Metadata.MediaType);
        Assert.Equal(2, result.Nzbs.Count);
        Assert.Equal("nzb-1", result.Nzbs[0].Id);
        Assert.Equal("Test.Movie.2024.1080p.nzb", result.Nzbs[0].Name);
        Assert.Equal(1500000000L, result.Nzbs[0].Size);
        Assert.Equal("nzb-2", result.Nzbs[1].Id);
        Assert.Equal(800000000L, result.Nzbs[1].Size);
    }

    [Fact]
    public void UsenetSearchResponse_Deserialize_WithNullMetadata_DefaultsCorrectly()
    {
        // Arrange
        string json = """
            {}
            """;

        // Act
        UsenetSearchResponse? result = JsonSerializer.Deserialize<UsenetSearchResponse>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Metadata);
        Assert.Empty(result.Nzbs);
    }
}
