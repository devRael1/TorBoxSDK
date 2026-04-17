using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Rss;
using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.Live;

/// <summary>
/// Live schema tests for the RSS resource client.
/// Calls the real TorBox API and verifies that every JSON field in the response
/// is mapped by a <c>[JsonPropertyName]</c> attribute in the SDK model.
/// </summary>
[Collection("SchemaLive")]
[Trait("Category", "Live")]
public sealed class RssSchemaLiveTests(SchemaLiveTestFixture fixture)
{
    private readonly SchemaLiveTestFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetFeeds_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/rss/getfeeds", cts.Token)
            .ConfigureAwait(false);

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<RssFeed>>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/rss/getfeeds", typeof(RssFeed), unmapped));
    }

    private static string BuildMessage(
        string endpoint,
        Type modelType,
        IReadOnlyList<string> unmapped) =>
        $"Endpoint '{endpoint}' returned {unmapped.Count} unmapped field path(s) " +
        $"not covered by '{modelType.Name}' (including nested types):{Environment.NewLine}" +
        string.Join(Environment.NewLine, unmapped.Select(f => $"  - {f}"));
}
