using System.Net;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.V2.UnitTests.Models.Common;

public sealed class TorBoxStreamResponseTests
{
	[Fact]
	public void Dispose_WithSuccessfulStream_ClosesTheOwnedContentStream()
	{
		// Arrange
		MemoryStream stream = new([1, 2, 3]);
		TorBoxStreamResponse response = TorBoxStreamResponse.CreateForTesting(stream, HttpStatusCode.OK);

		// Act
		response.Dispose();

		// Assert
		Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
	}

	[Fact]
	public void CreateForTesting_WithSuccessfulStream_PreservesContentMetadata()
	{
		// Arrange
		MemoryStream stream = new([1, 2, 3]);

		// Act
		using TorBoxStreamResponse response = TorBoxStreamResponse.CreateForTesting(
			stream,
			HttpStatusCode.OK,
			mediaType: "application/octet-stream",
			contentLength: 3,
			fileName: "archive.bin");

		// Assert
		Assert.True(response.Success);
		Assert.Same(stream, response.Stream);
		Assert.Equal("application/octet-stream", response.MediaType);
		Assert.Equal(3, response.ContentLength);
		Assert.Equal("archive.bin", response.FileName);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public void Constructor_WithSuccessfulStreamAndNoHttpResponse_RejectsMissingOwner()
	{
		// Arrange
		using MemoryStream stream = new([1, 2, 3]);

		// Act
		Action create = () => _ = new TorBoxStreamResponse(
			ownedResponse: null,
			success: true,
			HttpStatusCode.OK,
			stream);

		// Assert
		Assert.Throws<ArgumentNullException>(create);
	}

	[Fact]
	public void CreateForTesting_WithStructuredApiFailure_PreservesBoundedFailureWithoutAStream()
	{
		// Arrange
		const string Error = "BAD_TOKEN";
		const string Detail = "Invalid token.";

		// Act
		using TorBoxStreamResponse response = TorBoxStreamResponse.CreateForTesting(
			stream: null,
			HttpStatusCode.Unauthorized,
			success: false,
			error: Error,
			detail: Detail);

		// Assert
		Assert.False(response.Success);
		Assert.Equal(Error, response.Error);
		Assert.Equal(Detail, response.Detail);
		Assert.Null(response.Stream);
		Assert.Null(response.RedirectUri);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public void CreateForTesting_WithRedirect_PreservesRedirectMetadataWithoutAStream()
	{
		// Arrange
		Uri redirectUri = new("https://download.example/file.bin");

		// Act
		using TorBoxStreamResponse response = TorBoxStreamResponse.CreateForTesting(
			stream: null,
			HttpStatusCode.TemporaryRedirect,
			redirectUri: redirectUri);

		// Assert
		Assert.True(response.Success);
		Assert.Null(response.Stream);
		Assert.Equal(redirectUri, response.RedirectUri);
		Assert.Equal(HttpStatusCode.TemporaryRedirect, response.StatusCode);
	}

	[Fact]
	public void PublicSurface_DoesNotExposeTheOwnedHttpResponseMessage()
	{
		// Arrange
		Type responseType = typeof(TorBoxStreamResponse);

		// Act
		IReadOnlyList<Type> publicPropertyTypes = responseType
			.GetProperties()
			.Select(static property => property.PropertyType)
			.ToArray();

		// Assert
		Assert.DoesNotContain(typeof(HttpResponseMessage), publicPropertyTypes);
	}
}
