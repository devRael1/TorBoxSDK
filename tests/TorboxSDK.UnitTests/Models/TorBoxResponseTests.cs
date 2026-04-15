using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Common;

namespace TorboxSDK.UnitTests.Models;

public sealed class TorBoxResponseTests
{
    [Fact]
    public void Deserialize_WithSuccessAndData_ReturnsPopulatedResponse()
    {
        // Arrange
        string json = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": "https://download.torbox.app/file"
            }
            """;

        // Act
        TorBoxResponse<string>? result = JsonSerializer.Deserialize<TorBoxResponse<string>>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Equal("Found.", result.Detail);
        Assert.Equal("https://download.torbox.app/file", result.Data);
    }

    [Fact]
    public void Deserialize_WithError_ReturnsFalseSuccess()
    {
        // Arrange
        string json = """
            {
                "success": false,
                "error": "BAD_TOKEN",
                "detail": "The provided token is invalid.",
                "data": null
            }
            """;

        // Act
        TorBoxResponse<string>? result = JsonSerializer.Deserialize<TorBoxResponse<string>>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("BAD_TOKEN", result.Error);
        Assert.Equal("The provided token is invalid.", result.Detail);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Deserialize_WithNullData_ReturnsNullData()
    {
        // Arrange
        string json = """
            {
                "success": true,
                "error": null,
                "detail": "Success.",
                "data": null
            }
            """;

        // Act
        TorBoxResponse<string>? result = JsonSerializer.Deserialize<TorBoxResponse<string>>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Deserialize_NonGeneric_WithSuccess_ReturnsTrue()
    {
        // Arrange
        string json = """
            {
                "success": true,
                "error": null,
                "detail": "Operation completed."
            }
            """;

        // Act
        TorBoxResponse? result = JsonSerializer.Deserialize<TorBoxResponse>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Equal("Operation completed.", result.Detail);
    }
}
