using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;
using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.Live;

/// <summary>
/// Live schema tests for the General (platform-wide) endpoints.
/// Calls the real TorBox API and verifies that every JSON field in the response
/// is mapped by a <c>[JsonPropertyName]</c> attribute in the SDK model.
/// </summary>
[Collection("SchemaLive")]
[Trait("Category", "Live")]
public sealed class GeneralSchemaLiveTests(SchemaLiveTestFixture fixture)
{
    private readonly SchemaLiveTestFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetStats_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

        // Act
        using HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/stats", cts.Token)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<Stats>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/stats", typeof(Stats), unmapped));
    }

    [SkippableFact]
    public async Task Get30DayStats_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

        // Act
        using HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/stats/30day", cts.Token)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<DailyStats>>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/stats/30day", typeof(DailyStats), unmapped));
    }

    [SkippableFact]
    public async Task GetSpeedtestFiles_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

        // Act
        using HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/general/speedtest", cts.Token)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<SpeedtestServer>>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/general/speedtest", typeof(SpeedtestServer), unmapped));
    }

    [SkippableFact]
    public async Task GetChangelogsJson_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

        // Act
        using HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/general/changelogs/json", cts.Token)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<Changelog>>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/general/changelogs/json", typeof(Changelog), unmapped));
    }

    private static string BuildMessage(
        string endpoint,
        Type modelType,
        IReadOnlyList<string> unmapped) =>
        $"Endpoint '{endpoint}' returned {unmapped.Count} unmapped field path(s) " +
        $"not covered by '{modelType.Name}' (including nested types):{Environment.NewLine}" +
        string.Join(Environment.NewLine, unmapped.Select(f => $"  - {f}"));
}
