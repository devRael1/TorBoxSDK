using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;
using TorBoxSDK.TestUtilities;

namespace TorBoxSDK.IntegrationTests.Schema;

/// <summary>
/// Schema validation integration tests for the Search resource.
/// Verifies that every JSON field in live API responses is mapped
/// by a <c>[JsonPropertyName]</c> attribute in the SDK model.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
[Trait("Category", "Schema")]
public sealed class SearchSchemaIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task SearchTorrents_Schema_AllFieldsMapped()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        string endpoint = "/v1/api/search/torrents/ubuntu";

        // Act
        IReadOnlyList<string> unmapped = await SchemaAssert
            .FindUnmappedFieldsAsync<TorBoxResponse<TorrentSearchResponse>>(
                _fixture.RawHttpClient, endpoint, cts.Token);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            SchemaAssert.BuildFailureMessage(endpoint, typeof(TorrentSearchResponse), unmapped));
    }

    [SkippableFact]
    public async Task SearchUsenet_Schema_AllFieldsMapped()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        string endpoint = "/v1/api/search/usenet/ubuntu";

        // Act
        IReadOnlyList<string> unmapped = await SchemaAssert
            .FindUnmappedFieldsAsync<TorBoxResponse<UsenetSearchResponse>>(
                _fixture.RawHttpClient, endpoint, cts.Token);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            SchemaAssert.BuildFailureMessage(endpoint, typeof(UsenetSearchResponse), unmapped));
    }

    [SkippableFact]
    public async Task SearchMeta_Schema_AllFieldsMapped()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        string endpoint = "/v1/api/search/meta/inception";

        // Act
        IReadOnlyList<string> unmapped = await SchemaAssert
            .FindUnmappedFieldsAsync<TorBoxResponse<IReadOnlyList<MetaSearchResult>>>(
                _fixture.RawHttpClient, endpoint, cts.Token);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            SchemaAssert.BuildFailureMessage(endpoint, typeof(MetaSearchResult), unmapped));
    }
}
