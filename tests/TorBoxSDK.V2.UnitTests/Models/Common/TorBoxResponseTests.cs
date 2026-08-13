using System.Net;
using System.Text.Json;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Serialization;

namespace TorBoxSDK.V2.UnitTests.Models.Common;

public sealed class TorBoxResponseTests
{
	[Fact]
	public void TorBoxResponse_WithStructuredApiFailure_PreservesTheEnvelopeAndStatus()
	{
		// Arrange
		TorBoxResponse response = new()
		{
			Success = false,
			Error = "BAD_TOKEN",
			Detail = "Invalid token.",
			StatusCode = HttpStatusCode.Unauthorized,
		};

		// Act
		HttpStatusCode statusCode = response.StatusCode;

		// Assert
		Assert.False(response.Success);
		Assert.Equal("BAD_TOKEN", response.Error);
		Assert.Equal("Invalid token.", response.Detail);
		Assert.Equal(HttpStatusCode.Unauthorized, statusCode);
	}

	[Fact]
	public void TorBoxResponseOfT_WithData_PreservesThePayloadAndStatusOutsideJson()
	{
		// Arrange
		TorBoxResponse<string> response = new()
		{
			Success = true,
			Detail = "Found.",
			Data = "payload",
			StatusCode = HttpStatusCode.OK,
		};

		// Act
		string? data = response.Data;

		// Assert
		Assert.Equal("payload", data);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public void ResponseTypes_AreSealedIndependentRecords()
	{
		// Arrange
		Type responseType = typeof(TorBoxResponse);
		Type genericResponseType = typeof(TorBoxResponse<string>);

		// Act
		Type? responseBaseType = responseType.BaseType;
		Type? genericResponseBaseType = genericResponseType.BaseType;

		// Assert
		Assert.True(responseType.IsSealed);
		Assert.True(genericResponseType.IsSealed);
		Assert.Equal(typeof(object), responseBaseType);
		Assert.Equal(typeof(object), genericResponseBaseType);
	}

	[Fact]
	public void TorBoxJsonOptions_Default_ReturnsTheReusableInstance()
	{
		// Arrange
		JsonSerializerOptions first = TorBoxJsonOptions.Default;

		// Act
		JsonSerializerOptions second = TorBoxJsonOptions.Default;

		// Assert
		Assert.Same(first, second);
	}

	[Fact]
	public void TorBoxJsonOptions_Default_PreservesExplicitResponseWireMappings()
	{
		// Arrange
		TorBoxResponse<string> response = new()
		{
			Success = true,
			Error = null,
			Detail = "Found.",
			Data = "payload",
			StatusCode = HttpStatusCode.OK,
		};

		// Act
		string json = JsonSerializer.Serialize(response, TorBoxJsonOptions.Default);

		// Assert
		Assert.Equal("{\"success\":true,\"error\":null,\"detail\":\"Found.\",\"data\":\"payload\"}", json);
	}
}
