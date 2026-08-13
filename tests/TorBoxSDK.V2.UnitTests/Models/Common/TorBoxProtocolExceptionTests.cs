using System.Net;
using System.Text;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.V2.UnitTests.Models.Common;

public sealed class TorBoxProtocolExceptionTests
{
	[Fact]
	public void Constructor_WithNullMessage_ThrowsArgumentNullException()
	{
		// Arrange
		Uri requestUri = new("https://api.torbox.app/v1/api/user/me");

		// Act
		Action create = () => _ = new TorBoxProtocolException(
#pragma warning disable CS8625 // Intentionally violate the nullable contract to verify the runtime guard.
			null,
#pragma warning restore CS8625
			requestUri,
			HttpStatusCode.BadGateway,
			"Unexpected HTML response.");

		// Assert
		ArgumentNullException exception = Assert.Throws<ArgumentNullException>(create);
		Assert.Equal("message", exception.ParamName);
	}

	[Fact]
	public void Constructor_WithKnownResponse_PreservesRequestStatusAndDetail()
	{
		// Arrange
		Uri requestUri = new("https://api.torbox.app/v1/api/user/me");

		// Act
		TorBoxProtocolException exception = new(
			"The response envelope is invalid.",
			requestUri,
			HttpStatusCode.BadGateway,
			"Unexpected HTML response.");

		// Assert
		Assert.Equal("The response envelope is invalid.", exception.Message);
		Assert.Equal(requestUri, exception.RequestUri);
		Assert.Equal(HttpStatusCode.BadGateway, exception.StatusCode);
		Assert.Equal("Unexpected HTML response.", exception.Detail);
	}

	[Fact]
	public void Constructor_WithMultibyteDiagnostic_BoundsTheRetainedUtf8Bytes()
	{
		// Arrange
		string oversizedDetail = new('é', 65_536);

		// Act
		TorBoxProtocolException exception = new(
			"The response envelope is invalid.",
			new Uri("https://api.torbox.app/v1/api/user/me"),
			HttpStatusCode.BadGateway,
			oversizedDetail);

		// Assert
		Assert.NotNull(exception.Detail);
		Assert.True(Encoding.UTF8.GetByteCount(exception.Detail) <= 65_536);
		Assert.True(exception.Detail.Length < oversizedDetail.Length);
	}

	[Fact]
	public void PublicContract_DoesNotAcceptRetainOrExposeAnHttpResponseMessage()
	{
		// Arrange
		Type exceptionType = typeof(TorBoxProtocolException);

		// Act
		IReadOnlyList<Type> constructorParameterTypes = exceptionType
			.GetConstructors()
			.SelectMany(static constructor => constructor.GetParameters())
			.Select(static parameter => parameter.ParameterType)
			.ToArray();
		IReadOnlyList<Type> instanceFieldTypes = exceptionType
			.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
			.Select(static field => field.FieldType)
			.ToArray();
		IReadOnlyList<Type> publicPropertyTypes = exceptionType
			.GetProperties()
			.Select(static property => property.PropertyType)
			.ToArray();

		// Assert
		Assert.DoesNotContain(typeof(HttpResponseMessage), constructorParameterTypes);
		Assert.DoesNotContain(typeof(HttpResponseMessage), instanceFieldTypes);
		Assert.DoesNotContain(typeof(HttpResponseMessage), publicPropertyTypes);
	}
}
