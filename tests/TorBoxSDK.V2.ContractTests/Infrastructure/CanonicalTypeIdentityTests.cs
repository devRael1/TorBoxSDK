using TorBoxSDK.Models.Common;

namespace TorBoxSDK.V2.ContractTests.Infrastructure;

public sealed class CanonicalTypeIdentityTests
{
    [Fact]
    public void Format_WithGenericTaskEnvelope_ReturnsVersionIndependentIdentity()
    {
        // Arrange
        Type type = typeof(Task<TorBoxResponse<string>>);

        // Act
        string identity = CanonicalTypeIdentity.Format(type);

        // Assert
        Assert.Equal(
            "System.Threading.Tasks.Task`1[TorBoxSDK.Models.Common.TorBoxResponse`1[System.String]]",
            identity);
    }

    [Fact]
    public void Format_WithNonGenericType_ReturnsTheFullName()
    {
        // Arrange
        Type type = typeof(CancellationToken);

        // Act
        string identity = CanonicalTypeIdentity.Format(type);

        // Assert
        Assert.Equal("System.Threading.CancellationToken", identity);
    }
}
