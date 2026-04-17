using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;
using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.Live;

/// <summary>
/// Live schema tests for the Notifications resource client.
/// Calls the real TorBox API and verifies that every JSON field in the response
/// is mapped by a <c>[JsonPropertyName]</c> attribute in the SDK model.
/// </summary>
[Collection("SchemaLive")]
[Trait("Category", "Live")]
public sealed class NotificationsSchemaLiveTests(SchemaLiveTestFixture fixture)
{
    private readonly SchemaLiveTestFixture _fixture = fixture;

    [SkippableFact]
    public async Task GetMyNotifications_ResponseFields_AllMappedInSdkModel()
    {
        Skip.If(!_fixture.HasApiKey, "TORBOX_API_KEY not set.");

        // Arrange
        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(1));

        // Act
        HttpResponseMessage response = await _fixture.HttpClient
            .GetAsync("/v1/api/notifications/mynotifications", cts.Token)
            .ConfigureAwait(false);

        string json = await response.Content
            .ReadAsStringAsync(cts.Token)
            .ConfigureAwait(false);

        IReadOnlyList<string> unmapped =
            UnmappedFieldDetector.FindUnmappedFields<TorBoxResponse<IReadOnlyList<Notification>>>(json);

        // Assert
        Assert.True(
            unmapped.Count == 0,
            BuildMessage("GET /v1/api/notifications/mynotifications", typeof(Notification), unmapped));
    }

    private static string BuildMessage(
        string endpoint,
        Type modelType,
        IReadOnlyList<string> unmapped) =>
        $"Endpoint '{endpoint}' returned {unmapped.Count} unmapped field path(s) " +
        $"not covered by '{modelType.Name}' (including nested types):{Environment.NewLine}" +
        string.Join(Environment.NewLine, unmapped.Select(f => $"  - {f}"));
}
