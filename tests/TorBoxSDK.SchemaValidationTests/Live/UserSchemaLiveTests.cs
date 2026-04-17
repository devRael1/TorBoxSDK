using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;
using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.Live;

/// <summary>
/// Live schema tests for the User resource client.
/// Calls the real TorBox API and verifies that every JSON field in the response
/// is mapped by a <c>[JsonPropertyName]</c> attribute in the SDK model.
/// </summary>
[Collection("SchemaLive")]
[Trait("Category", "Live")]
public sealed class UserSchemaLiveTests(SchemaLiveTestFixture fixture)
{
    private readonly SchemaLiveTestFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetMe_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/user/me", cts.Token)
            .ConfigureAwait(false);

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<UserProfile>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/user/me", typeof(UserProfile), unmapped));
    }

    [SkippableFact]
    public async Task GetSubscriptions_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/user/subscriptions", cts.Token)
            .ConfigureAwait(false);

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<Subscription>>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/user/subscriptions", typeof(Subscription), unmapped));
    }

    [SkippableFact]
    public async Task GetTransactions_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/user/transactions", cts.Token)
            .ConfigureAwait(false);

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<Transaction>>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/user/transactions", typeof(Transaction), unmapped));
    }

    [SkippableFact]
    public async Task GetSearchEngines_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/user/settings/searchengines", cts.Token)
            .ConfigureAwait(false);

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<SearchEngine>>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/user/settings/searchengines", typeof(SearchEngine), unmapped));
    }

    [SkippableFact]
    public async Task GetReferralData_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        // Act
        HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/user/referraldata", cts.Token)
            .ConfigureAwait(false);

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<ReferralData>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/user/referraldata", typeof(ReferralData), unmapped));
    }

    private static string BuildMessage(
        string endpoint,
        Type modelType,
        IReadOnlyList<string> unmapped) =>
        $"Endpoint '{endpoint}' returned {unmapped.Count} unmapped field path(s) " +
        $"not covered by '{modelType.Name}' (including nested types):{Environment.NewLine}" +
        string.Join(Environment.NewLine, unmapped.Select(f => $"  - {f}"));
}
