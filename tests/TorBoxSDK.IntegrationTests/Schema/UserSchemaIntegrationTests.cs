using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;
using TorBoxSDK.TestUtilities;

namespace TorBoxSDK.IntegrationTests.Schema;

/// <summary>
/// Schema validation integration tests for the User resource.
/// Verifies that every JSON field in live API responses is mapped
/// by a <c>[JsonPropertyName]</c> attribute in the SDK model.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
[Trait("Category", "Schema")]
public sealed class UserSchemaIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetMe_Schema_AllFieldsMapped()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        string endpoint = "/v1/api/user/me";

        // Act
        IReadOnlyList<string> unmapped = await SchemaAssert
            .FindUnmappedFieldsAsync<TorBoxResponse<UserProfile>>(
                _fixture.RawHttpClient, endpoint, cts.Token);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            SchemaAssert.BuildFailureMessage(endpoint, typeof(UserProfile), unmapped));
    }

    [SkippableFact]
    public async Task GetSubscriptions_Schema_AllFieldsMapped()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        string endpoint = "/v1/api/user/subscriptions";

        // Act
        IReadOnlyList<string> unmapped = await SchemaAssert
            .FindUnmappedFieldsAsync<TorBoxResponse<IReadOnlyList<Subscription>>>(
                _fixture.RawHttpClient, endpoint, cts.Token);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            SchemaAssert.BuildFailureMessage(endpoint, typeof(Subscription), unmapped));
    }

    [SkippableFact]
    public async Task GetTransactions_Schema_AllFieldsMapped()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        string endpoint = "/v1/api/user/transactions";

        // Act
        IReadOnlyList<string> unmapped = await SchemaAssert
            .FindUnmappedFieldsAsync<TorBoxResponse<IReadOnlyList<Transaction>>>(
                _fixture.RawHttpClient, endpoint, cts.Token);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            SchemaAssert.BuildFailureMessage(endpoint, typeof(Transaction), unmapped));
    }

    [SkippableFact]
    public async Task GetSearchEngines_Schema_AllFieldsMapped()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        string endpoint = "/v1/api/user/settings/searchengines";

        // Act
        IReadOnlyList<string> unmapped = await SchemaAssert
            .FindUnmappedFieldsAsync<TorBoxResponse<IReadOnlyList<SearchEngine>>>(
                _fixture.RawHttpClient, endpoint, cts.Token);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            SchemaAssert.BuildFailureMessage(endpoint, typeof(SearchEngine), unmapped));
    }

    [SkippableFact]
    public async Task GetReferralData_Schema_AllFieldsMapped()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        string endpoint = "/v1/api/user/referraldata";

        // Act
        IReadOnlyList<string> unmapped = await SchemaAssert
            .FindUnmappedFieldsAsync<TorBoxResponse<ReferralData>>(
                _fixture.RawHttpClient, endpoint, cts.Token);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            SchemaAssert.BuildFailureMessage(endpoint, typeof(ReferralData), unmapped));
    }
}
