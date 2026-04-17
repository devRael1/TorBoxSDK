using TorBoxSDK.IntegrationTests.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;
using TorBoxSDK.TestUtilities;

namespace TorBoxSDK.IntegrationTests.Schema;

/// <summary>
/// Schema validation integration tests for the Notifications resource.
/// Verifies that every JSON field in live API responses is mapped
/// by a <c>[JsonPropertyName]</c> attribute in the SDK model.
/// </summary>
[Collection("Integration")]
[Trait("Category", "Integration")]
[Trait("Category", "Schema")]
public sealed class NotificationsSchemaIntegrationTests(TorBoxIntegrationFixture fixture)
{
    private readonly TorBoxIntegrationFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetMyNotifications_Schema_AllFieldsMapped()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        string endpoint = "/v1/api/notifications/mynotifications";

        // Act
        IReadOnlyList<string> unmapped = await SchemaAssert
            .FindUnmappedFieldsAsync<TorBoxResponse<IReadOnlyList<Notification>>>(
                _fixture.RawHttpClient, endpoint, cts.Token);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            SchemaAssert.BuildFailureMessage(endpoint, typeof(Notification), unmapped));
    }
}
