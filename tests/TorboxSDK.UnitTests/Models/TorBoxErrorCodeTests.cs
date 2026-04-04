using TorBoxSDK.Models.Common;

namespace TorboxSDK.UnitTests.Models;

public sealed class TorBoxErrorCodeTests
{
    [Fact]
    public void Unknown_HasValueZero()
    {
        // Arrange & Act
        int value = (int)TorBoxErrorCode.Unknown;

        // Assert
        Assert.Equal(0, value);
    }

    [Theory]
    [InlineData(nameof(TorBoxErrorCode.Unknown))]
    [InlineData(nameof(TorBoxErrorCode.DatabaseError))]
    [InlineData(nameof(TorBoxErrorCode.UnknownError))]
    [InlineData(nameof(TorBoxErrorCode.NoAuth))]
    [InlineData(nameof(TorBoxErrorCode.BadToken))]
    [InlineData(nameof(TorBoxErrorCode.InvalidOption))]
    [InlineData(nameof(TorBoxErrorCode.PermissionDenied))]
    [InlineData(nameof(TorBoxErrorCode.PlanRestrictedFeature))]
    [InlineData(nameof(TorBoxErrorCode.DuplicateItem))]
    [InlineData(nameof(TorBoxErrorCode.BreachOfTos))]
    [InlineData(nameof(TorBoxErrorCode.ActiveLimit))]
    [InlineData(nameof(TorBoxErrorCode.SeedingLimit))]
    [InlineData(nameof(TorBoxErrorCode.BannedContentDetected))]
    [InlineData(nameof(TorBoxErrorCode.CouldNotPerformAction))]
    [InlineData(nameof(TorBoxErrorCode.ItemNotFound))]
    [InlineData(nameof(TorBoxErrorCode.InvalidDevice))]
    [InlineData(nameof(TorBoxErrorCode.DeviceAlreadyAuthed))]
    [InlineData(nameof(TorBoxErrorCode.TooManyRequests))]
    [InlineData(nameof(TorBoxErrorCode.DownloadTooLarge))]
    [InlineData(nameof(TorBoxErrorCode.MissingRequiredOption))]
    [InlineData(nameof(TorBoxErrorCode.BannedUser))]
    [InlineData(nameof(TorBoxErrorCode.SearchError))]
    [InlineData(nameof(TorBoxErrorCode.ServerError))]
    public void AllExpectedValues_AreDefined(string name)
    {
        // Arrange & Act
        bool isDefined = Enum.TryParse<TorBoxErrorCode>(name, out _);

        // Assert
        Assert.True(isDefined, $"Expected TorBoxErrorCode to define '{name}'.");
    }
}
