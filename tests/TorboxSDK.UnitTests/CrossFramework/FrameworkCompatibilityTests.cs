using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK;
using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Http.Validation;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;
using TorBoxSDK.Models.Notifications;

using GeneralStats = TorBoxSDK.Models.General.Stats;

namespace TorboxSDK.UnitTests.CrossFramework;

/// <summary>
/// Tests that validate cross-framework compatibility for the TorBox SDK.
/// These tests exercise framework-specific code paths, including the
/// <see cref="Guard"/> polyfill (uses built-in API on net7.0+, manual checks on net6.0) and
/// <see cref="TorBoxJsonOptions"/> (uses built-in SnakeCaseLower on net8.0+, custom policy on net6.0/net7.0).
/// The unit test project targets net8.0 and net10.0 to verify both frameworks at runtime.
/// </summary>
public sealed class FrameworkCompatibilityTests
{
    // --- Guard polyfill tests ---

    [Fact]
    public void Guard_ThrowIfNullOrEmpty_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Guard.ThrowIfNullOrEmpty(value, "param"));
    }

    [Fact]
    public void Guard_ThrowIfNullOrEmpty_WithEmpty_ThrowsArgumentException()
    {
        // Arrange
        string value = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.ThrowIfNullOrEmpty(value, "param"));
    }

    [Fact]
    public void Guard_ThrowIfNullOrEmpty_WithValidString_DoesNotThrow()
    {
        // Arrange
        string value = "valid";

        // Act & Assert (no exception thrown)
        Guard.ThrowIfNullOrEmpty(value, "param");
    }

    // --- JSON serialization with snake_case naming policy ---

    [Fact]
    public void TorBoxJsonOptions_Default_UsesSnakeCaseNaming()
    {
        // Arrange
        TorBoxResponse<string> response = new()
        {
            Success = true,
            Detail = "Found.",
            Data = "test"
        };

        // Act
        string json = JsonSerializer.Serialize(response, TorBoxJsonOptions.Default);

        // Assert — property names must be snake_case regardless of framework
        Assert.Contains("\"success\":", json);
        Assert.Contains("\"detail\":", json);
        Assert.Contains("\"data\":", json);
    }

    [Fact]
    public void TorBoxJsonOptions_Default_DeserializesSnakeCaseProperties()
    {
        // Arrange
        string json = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": {
                    "total_users": 500000,
                    "total_servers": 60
                }
            }
            """;

        // Act
        TorBoxResponse<GeneralStats>? result =
            JsonSerializer.Deserialize<TorBoxResponse<GeneralStats>>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(500000, result.Data.TotalUsers);
        Assert.Equal(60, result.Data.TotalServers);
    }

    // --- DateTimeOffset UTC converter tests ---

    [Fact]
    public void UtcDateTimeOffsetConverter_Deserialize_NormalizesToUtc()
    {
        // Arrange — offset is +05:00, should be normalized to UTC
        string json = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": {
                    "created_at": "2024-07-10T12:12:36+05:00"
                }
            }
            """;

        // Act
        TorBoxResponse<DateTimeOffsetModel>? result =
            JsonSerializer.Deserialize<TorBoxResponse<DateTimeOffsetModel>>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(TimeSpan.Zero, result.Data.CreatedAt.Offset);
        Assert.Equal(new DateTimeOffset(2024, 7, 10, 7, 12, 36, TimeSpan.Zero), result.Data.CreatedAt);
    }

    [Fact]
    public void NullableUtcDateTimeOffsetConverter_Deserialize_WithValue_NormalizesToUtc()
    {
        // Arrange
        string json = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": {
                    "updated_at": "2024-01-15T10:00:00+02:00"
                }
            }
            """;

        // Act
        TorBoxResponse<NullableDateTimeOffsetModel>? result =
            JsonSerializer.Deserialize<TorBoxResponse<NullableDateTimeOffsetModel>>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.UpdatedAt);
        Assert.Equal(TimeSpan.Zero, result.Data.UpdatedAt.Value.Offset);
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 8, 0, 0, TimeSpan.Zero), result.Data.UpdatedAt);
    }

    [Fact]
    public void NullableUtcDateTimeOffsetConverter_Deserialize_WithNull_ReturnsNull()
    {
        // Arrange
        string json = """
            {
                "success": true,
                "error": null,
                "detail": "Found.",
                "data": {
                    "updated_at": null
                }
            }
            """;

        // Act
        TorBoxResponse<NullableDateTimeOffsetModel>? result =
            JsonSerializer.Deserialize<TorBoxResponse<NullableDateTimeOffsetModel>>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Null(result.Data.UpdatedAt);
    }

    [Fact]
    public void UtcDateTimeOffsetConverter_Serialize_WritesUtcValue()
    {
        // Arrange — value with +05:00 offset
        DateTimeOffsetModel model = new()
        {
            CreatedAt = new DateTimeOffset(2024, 7, 10, 12, 0, 0, TimeSpan.FromHours(5))
        };

        // Act
        string json = JsonSerializer.Serialize(model, TorBoxJsonOptions.Default);

        // Assert — serialized value should be UTC (07:00:00+00:00)
        Assert.Contains("2024-07-10T07:00:00", json);
        Assert.Contains("+00:00", json);
    }

    // --- Enum serialization (JsonStringEnumConverter with snake_case) ---

    [Fact]
    public void TorBoxJsonOptions_Default_DeserializesEnumFromSnakeCaseString()
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
        TorBoxResponse<string>? result =
            JsonSerializer.Deserialize<TorBoxResponse<string>>(json, TorBoxJsonOptions.Default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("BAD_TOKEN", result.Error);
    }

    // --- Collection expression defaults ([] initializers) ---

    [Fact]
    public void CollectionExpressionDefaults_WithDefaultInit_ReturnsEmptyCollections()
    {
        // Arrange & Act — models created with default init should have empty (not null) collections
        ChangelogsRssChannel channel = new();
        NotificationRssFeed feed = new();

        // Assert
        Assert.NotNull(channel.Items);
        Assert.Empty(channel.Items);
        Assert.NotNull(feed.Items);
        Assert.Empty(feed.Items);
    }

    // --- DI registration works across frameworks ---

    [Fact]
    public void AddTorBox_WithValidConfig_ResolvesITorBoxClient()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddTorBox(options =>
        {
            options.ApiKey = "test-key";
        });

        // Act
        using ServiceProvider provider = services.BuildServiceProvider();
        ITorBoxClient? client = provider.GetService<ITorBoxClient>();

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(client.Main);
        Assert.NotNull(client.Search);
        Assert.NotNull(client.Relay);
    }

    [Fact]
    public void AddTorBox_WithValidConfig_AllResourceClientsAccessible()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddTorBox(options =>
        {
            options.ApiKey = "test-key";
        });

        // Act
        using ServiceProvider provider = services.BuildServiceProvider();
        ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();

        // Assert
        Assert.NotNull(client.Main.General);
        Assert.NotNull(client.Main.Torrents);
        Assert.NotNull(client.Main.Usenet);
        Assert.NotNull(client.Main.WebDownloads);
        Assert.NotNull(client.Main.User);
        Assert.NotNull(client.Main.Notifications);
        Assert.NotNull(client.Main.Rss);
        Assert.NotNull(client.Main.Stream);
        Assert.NotNull(client.Main.Integrations);
        Assert.NotNull(client.Main.Vendors);
        Assert.NotNull(client.Main.Queued);
    }

    // --- TorBoxResponse record equality and immutability ---

    [Fact]
    public void TorBoxResponse_SealedRecord_SupportsValueEquality()
    {
        // Arrange
        TorBoxResponse<string> a = new() { Success = true, Detail = "OK.", Data = "value" };
        TorBoxResponse<string> b = new() { Success = true, Detail = "OK.", Data = "value" };
        TorBoxResponse<string> c = new() { Success = false, Detail = "Fail.", Data = "other" };

        // Assert
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }

    // --- Test models for DateTimeOffset converter verification ---

    private sealed record DateTimeOffsetModel
    {
        [System.Text.Json.Serialization.JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }
    }

    private sealed record NullableDateTimeOffsetModel
    {
        [System.Text.Json.Serialization.JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; init; }
    }
}
