using System.Text.Json;
using TorBoxSDK.Http.Json;
using TorBoxSDK.Models.Vendors;

namespace TorboxSDK.UnitTests.Models.Vendors;

public sealed class VendorModelTests
{
	// ──── RegisterVendorRequest ────

	[Fact]
	public void RegisterVendorRequest_Serialize_WithAllProperties_ProducesExpectedJson()
	{
		// Arrange
		RegisterVendorRequest request = new()
		{
			VendorName = "Acme Corp",
			VendorUrl = "https://acme.example.com",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("Acme Corp", root.GetProperty("vendor_name").GetString());
		Assert.Equal("https://acme.example.com", root.GetProperty("vendor_url").GetString());
	}

	[Fact]
	public void RegisterVendorRequest_Serialize_WithNullUrl_OmitsUrlProperty()
	{
		// Arrange
		RegisterVendorRequest request = new()
		{
			VendorName = "Acme Corp",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("Acme Corp", root.GetProperty("vendor_name").GetString());
		Assert.False(root.TryGetProperty("vendor_url", out _));
	}

	// ──── RegisterVendorUserRequest ────

	[Fact]
	public void RegisterVendorUserRequest_Serialize_ProducesExpectedJson()
	{
		// Arrange
		RegisterVendorUserRequest request = new()
		{
			UserEmail = "user@example.com",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("user@example.com", root.GetProperty("user_email").GetString());
	}

	// ──── RemoveVendorUserRequest ────

	[Fact]
	public void RemoveVendorUserRequest_Serialize_ProducesExpectedJson()
	{
		// Arrange
		RemoveVendorUserRequest request = new()
		{
			UserEmail = "remove@example.com",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("remove@example.com", root.GetProperty("user_email").GetString());
	}

	// ──── UpdateVendorAccountRequest ────

	[Fact]
	public void UpdateVendorAccountRequest_Serialize_WithAllProperties_ProducesExpectedJson()
	{
		// Arrange
		UpdateVendorAccountRequest request = new()
		{
			VendorName = "New Vendor Name",
			VendorUrl = "https://new-vendor.example.com",
		};

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.Equal("New Vendor Name", root.GetProperty("vendor_name").GetString());
		Assert.Equal("https://new-vendor.example.com", root.GetProperty("vendor_url").GetString());
	}

	[Fact]
	public void UpdateVendorAccountRequest_Serialize_WithNullOptionals_OmitsNullProperties()
	{
		// Arrange
		UpdateVendorAccountRequest request = new();

		// Act
		string json = JsonSerializer.Serialize(request, TorBoxJsonOptions.Default);

		// Assert
		using JsonDocument doc = JsonDocument.Parse(json);
		JsonElement root = doc.RootElement;
		Assert.False(root.TryGetProperty("vendor_name", out _));
		Assert.False(root.TryGetProperty("vendor_url", out _));
	}

	// ──── VendorAccount ────

	[Fact]
	public void VendorAccount_Deserialize_PopulatesAllProperties()
	{
		// Arrange
		string json = """
            {
                "id": 77,
                "vendor_name": "Acme Corp",
                "vendor_url": "https://acme.example.com",
                "api_key": "vnd-api-key-abc123",
                "created_at": "2024-11-01T00:00:00Z"
            }
            """;

		// Act
		VendorAccount? result = JsonSerializer.Deserialize<VendorAccount>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(77L, result.Id);
		Assert.Equal("Acme Corp", result.VendorName);
		Assert.Equal("https://acme.example.com", result.VendorUrl);
		Assert.Equal("vnd-api-key-abc123", result.ApiKey);
		Assert.NotNull(result.CreatedAt);
		Assert.Equal(new DateTimeOffset(2024, 11, 1, 0, 0, 0, TimeSpan.Zero), result.CreatedAt);
	}

	[Fact]
	public void VendorAccount_Deserialize_WithNullOptionals_ReturnsNulls()
	{
		// Arrange
		string json = """
            {
                "id": 1
            }
            """;

		// Act
		VendorAccount? result = JsonSerializer.Deserialize<VendorAccount>(json, TorBoxJsonOptions.Default);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(1L, result.Id);
		Assert.Null(result.VendorName);
		Assert.Null(result.VendorUrl);
		Assert.Null(result.ApiKey);
		Assert.Null(result.CreatedAt);
	}
}
