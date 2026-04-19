using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Relay;

namespace TorboxSDK.UnitTests.Models.Relay;

public sealed class RelayModelTests
{
	// ──── RelayStatus ────

	[Fact]
	public void RelayStatus_Deserialize_PopulatesAllProperties()
	{
		// Arrange
		string json = """
            {
                "status": "online",
                "data": {
                    "current_online": 1500,
                    "current_workers": 25
                }
            }
            """;

		// Act
		RelayStatus? result = JsonSerializer.Deserialize<RelayStatus>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("online", result.Status);
		Assert.NotNull(result.Data);
		Assert.Equal(1500, result.Data.CurrentOnline);
		Assert.Equal(25, result.Data.CurrentWorkers);
	}

	[Fact]
	public void RelayStatus_Deserialize_WithNullData_ReturnsNull()
	{
		// Arrange
		string json = """
            {
                "status": "maintenance",
                "data": null
            }
            """;

		// Act
		RelayStatus? result = JsonSerializer.Deserialize<RelayStatus>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("maintenance", result.Status);
		Assert.Null(result.Data);
	}

	// ──── RelayData ────

	[Fact]
	public void RelayData_Deserialize_PopulatesProperties()
	{
		// Arrange
		string json = """
            {
                "current_online": 3200,
                "current_workers": 48
            }
            """;

		// Act
		RelayData? result = JsonSerializer.Deserialize<RelayData>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(3200, result.CurrentOnline);
		Assert.Equal(48, result.CurrentWorkers);
	}

	// ──── InactiveCheckResult ────

	[Fact]
	public void InactiveCheckResult_Deserialize_PopulatesAllProperties()
	{
		// Arrange
		string json = """
            {
                "status": "checked",
                "is_inactive": true,
                "last_active": "2025-01-05T18:30:00Z"
            }
            """;

		// Act
		InactiveCheckResult? result = JsonSerializer.Deserialize<InactiveCheckResult>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("checked", result.Status);
		Assert.True(result.IsInactive);
		Assert.NotNull(result.LastActive);
		Assert.Equal(new DateTimeOffset(2025, 1, 5, 18, 30, 0, TimeSpan.Zero), result.LastActive);
	}

	[Fact]
	public void InactiveCheckResult_Deserialize_ActiveTorrent_ReturnsFalse()
	{
		// Arrange
		string json = """
            {
                "status": "checked",
                "is_inactive": false,
                "last_active": "2025-04-15T10:00:00Z"
            }
            """;

		// Act
		InactiveCheckResult? result = JsonSerializer.Deserialize<InactiveCheckResult>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsInactive);
		Assert.NotNull(result.LastActive);
	}

	[Fact]
	public void InactiveCheckResult_Deserialize_WithNullOptionals_ReturnsNulls()
	{
		// Arrange
		string json = """
            {
                "is_inactive": false
            }
            """;

		// Act
		InactiveCheckResult? result = JsonSerializer.Deserialize<InactiveCheckResult>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Null(result.Status);
		Assert.False(result.IsInactive);
		Assert.Null(result.LastActive);
	}


}
