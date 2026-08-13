using System.Net;
using System.Text;
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
	public void Dispose_WithDistinctContentAndExposedStreams_ClosesBothOwnedStreams()
	{
		// Arrange
		using MemoryStream contentStream = new([1, 2, 3]);
		using MemoryStream exposedStream = new([4, 5, 6]);
		using HttpResponseMessage ownedResponse = new(HttpStatusCode.OK)
		{
			Content = new StreamContent(contentStream),
		};
		TorBoxStreamResponse response = new(
			ownedResponse,
			success: true,
			HttpStatusCode.OK,
			exposedStream);

		// Act
		response.Dispose();

		// Assert
		Assert.Throws<ObjectDisposedException>(() => exposedStream.ReadByte());
		Assert.Throws<ObjectDisposedException>(() => contentStream.ReadByte());
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
	public void Constructor_WithStreamAndRedirect_RejectsAmbiguousSuccess()
	{
		// Arrange
		using MemoryStream stream = new([1, 2, 3]);
		using HttpResponseMessage ownedResponse = new(HttpStatusCode.OK)
		{
			Content = new StreamContent(stream),
		};

		// Act
		Action create = () => _ = new TorBoxStreamResponse(
			ownedResponse,
			success: true,
			HttpStatusCode.OK,
			stream,
			redirectUri: new Uri("https://download.example/file.bin"));

		// Assert
		Assert.Throws<ArgumentException>(create);
	}

	[Fact]
	public void Constructor_WithStructuredFailureAndRedirect_RejectsHybridFailure()
	{
		// Arrange
		Uri redirectUri = new("https://download.example/file.bin");

		// Act
		Action create = () => _ = new TorBoxStreamResponse(
			ownedResponse: null,
			success: false,
			HttpStatusCode.BadRequest,
			redirectUri: redirectUri);

		// Assert
		Assert.Throws<ArgumentException>(create);
	}

	[Fact]
	public void Constructor_WithSuccessAndNoStreamOrRedirect_RejectsMissingResult()
	{
		// Arrange
		using HttpResponseMessage ownedResponse = new(HttpStatusCode.OK);

		// Act
		Action create = () => _ = new TorBoxStreamResponse(
			ownedResponse,
			success: true,
			HttpStatusCode.OK);

		// Assert
		Assert.Throws<ArgumentException>(create);
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
	public void CreateForTesting_WithOversizedError_BoundsTheRetainedUtf8Bytes()
	{
		// Arrange
		string oversizedError = new('E', 65_537);
		string expectedError = new('E', 65_536);

		// Act
		using TorBoxStreamResponse response = TorBoxStreamResponse.CreateForTesting(
			stream: null,
			HttpStatusCode.BadRequest,
			success: false,
			error: oversizedError);

		// Assert
		Assert.NotNull(response.Error);
		Assert.Equal(expectedError, response.Error);
		Assert.Equal(65_536, Encoding.UTF8.GetByteCount(response.Error));
	}

	[Fact]
	public void CreateForTesting_WithOversizedDetail_DoesNotSplitAUnicodeScalar()
	{
		// Arrange
		string expectedDetail = new('a', 65_533);
		string oversizedDetail = expectedDetail + "🙂tail";

		// Act
		using TorBoxStreamResponse response = TorBoxStreamResponse.CreateForTesting(
			stream: null,
			HttpStatusCode.BadRequest,
			success: false,
			detail: oversizedDetail);

		// Assert
		Assert.NotNull(response.Detail);
		Assert.Equal(expectedDetail, response.Detail);
		Assert.Equal(65_533, Encoding.UTF8.GetByteCount(response.Detail));
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
