namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

/// <summary>
/// xUnit collection definition that shares a single <see cref="SchemaLiveTestFixture"/>
/// across all live schema test classes.
/// </summary>
[CollectionDefinition("SchemaLive")]
public sealed class SchemaTestCollection : ICollectionFixture<SchemaLiveTestFixture>
{
}
