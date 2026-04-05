namespace TorBoxSDK.IntegrationTests.Helpers;

/// <summary>
/// xUnit collection definition that shares a single <see cref="TorBoxIntegrationFixture"/>
/// across all test classes in the <c>Integration</c> collection.
/// </summary>
[CollectionDefinition("Integration")]
public sealed class IntegrationTestCollection : ICollectionFixture<TorBoxIntegrationFixture>
{
}
