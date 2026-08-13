using System.Net;
using System.Net.Http.Headers;
using System.Text;
using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.V2.Testing;

namespace TorBoxSDK.V2.UnitTests.Http;

public sealed class TorBoxApiTransportTests
{
	[Theory]
	[InlineData(HttpStatusCode.OK)]
	[InlineData(HttpStatusCode.Unauthorized)]
	[InlineData(HttpStatusCode.InternalServerError)]
	public async Task SendAsync_WithStructuredTorBoxFailure_ReturnsFailureEnvelope(HttpStatusCode statusCode)
	{
		// Arrange
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.Json(
			statusCode,
			"""
			{
			  "success": false,
			  "error": "BAD_TOKEN",
			  "detail": "Invalid token.",
			  "data": null
			}
			""");
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxResponse<string> response = await transport.SendAsync<string>(client, request, CancellationToken.None);

		// Assert
		Assert.False(response.Success);
		Assert.Equal("BAD_TOKEN", response.Error);
		Assert.Equal("Invalid token.", response.Detail);
		Assert.Equal(statusCode, response.StatusCode);
	}

	[Theory]
	[InlineData(HttpStatusCode.OK)]
	[InlineData(HttpStatusCode.Unauthorized)]
	[InlineData(HttpStatusCode.InternalServerError)]
	public async Task SendAsyncWithoutData_WithStructuredTorBoxFailure_ReturnsFailureEnvelope(HttpStatusCode statusCode)
	{
		// Arrange
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.Json(
			statusCode,
			"""
			{
			  "success": false,
			  "error": "BAD_TOKEN",
			  "detail": "Invalid token."
			}
			""");
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxResponse response = await transport.SendAsync(client, request, CancellationToken.None);

		// Assert
		Assert.False(response.Success);
		Assert.Equal("BAD_TOKEN", response.Error);
		Assert.Equal("Invalid token.", response.Detail);
		Assert.Equal(statusCode, response.StatusCode);
	}

	[Theory]
	[InlineData(HttpStatusCode.OK)]
	[InlineData(HttpStatusCode.Unauthorized)]
	[InlineData(HttpStatusCode.InternalServerError)]
	public async Task SendStreamAsync_WithStructuredTorBoxFailure_ReturnsFailureEnvelope(HttpStatusCode statusCode)
	{
		// Arrange
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.Json(
			statusCode,
			"""
			{
			  "success": false,
			  "error": "BAD_TOKEN",
			  "detail": "Invalid token."
			}
			""");
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "torrents/requestdl");

		// Act
		using TorBoxStreamResponse response = await transport.SendStreamAsync(client, request, CancellationToken.None);

		// Assert
		Assert.False(response.Success);
		Assert.Equal("BAD_TOKEN", response.Error);
		Assert.Equal("Invalid token.", response.Detail);
		Assert.Null(response.Stream);
		Assert.Null(response.RedirectUri);
		Assert.Equal(statusCode, response.StatusCode);
	}

	[Theory]
	[InlineData("""
	{
	  "detail": "Invalid token.",
	  "data": null,
	  "success": false,
	  "error": "BAD_TOKEN"
	}
	""")]
	[InlineData("""
	{
	  "success": false,
	  "error": "BAD_TOKEN",
	  "data": null,
	  "detail": "Invalid token."
	}
	""")]
	public async Task SendAsync_WithFailurePropertiesInAnyOrder_PreservesTheEnvelope(string json)
	{
		// Arrange
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.Json(HttpStatusCode.Unauthorized, json);
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxResponse<string> response = await transport.SendAsync<string>(client, request, CancellationToken.None);

		// Assert
		Assert.False(response.Success);
		Assert.Equal("BAD_TOKEN", response.Error);
		Assert.Equal("Invalid token.", response.Detail);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task SendAsync_WithLargeIgnoredPropertyBeforeFailureDetail_PreservesTheEnvelope()
	{
		// Arrange
		string ignoredValue = new('x', 70_000);
		string json = "{\"ignored\":\"" + ignoredValue + "\",\"detail\":\"Invalid token.\",\"success\":false,\"error\":\"BAD_TOKEN\"}";
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.Json(HttpStatusCode.Unauthorized, json);
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxResponse<string> response = await transport.SendAsync<string>(client, request, CancellationToken.None);

		// Assert
		Assert.False(response.Success);
		Assert.Equal("BAD_TOKEN", response.Error);
		Assert.Equal("Invalid token.", response.Detail);
	}

	[Fact]
	public async Task SendAsync_WithLargeIgnoredPropertyAndOversizedFailureDetail_RetainsAtMost64KiB()
	{
		// Arrange
		string ignoredValue = new('x', 70_000);
		string oversizedDetail = new('d', 65_537);
		string json = "{\"ignored\":\"" + ignoredValue + "\",\"success\":false,\"error\":\"BAD_TOKEN\",\"detail\":\"" + oversizedDetail + "\"}";
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.Json(HttpStatusCode.Unauthorized, json);
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxResponse<string> response = await transport.SendAsync<string>(client, request, CancellationToken.None);

		// Assert
		Assert.False(response.Success);
		Assert.NotNull(response.Detail);
		Assert.Equal(65_536, Encoding.UTF8.GetByteCount(response.Detail));
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task SendAsync_WithDataBeforeSuccess_DeserializesThePayloadWithoutDependingOnPropertyOrder()
	{
		// Arrange
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.Json(
			HttpStatusCode.OK,
			"""
			{
			  "data": { "name": "payload" },
			  "detail": "Found.",
			  "success": true,
			  "error": null
			}
			""");
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxResponse<Dictionary<string, string>> response = await transport.SendAsync<Dictionary<string, string>>(
			client,
			request,
			CancellationToken.None);

		// Assert
		Assert.True(response.Success);
		Assert.Equal("payload", response.Data?["name"]);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task SendAsync_WithMalformedResponse_RetainsAtMost64KiBDiagnostic()
	{
		// Arrange
		string oversizedInvalidBody = "{\"success\":true,\"detail\":\"" + new string('d', 65_537);
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.Json(HttpStatusCode.OK, oversizedInvalidBody);
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxProtocolException exception = await Assert.ThrowsAsync<TorBoxProtocolException>(
			() => transport.SendAsync<string>(client, request, CancellationToken.None));

		// Assert
		Assert.NotNull(exception.Detail);
		Assert.InRange(Encoding.UTF8.GetByteCount(exception.Detail), 0, 65_536);
		Assert.Equal(HttpStatusCode.OK, exception.StatusCode);
	}

	[Fact]
	public async Task SendAsync_WithUnreadableJsonStream_ThrowsTorBoxProtocolException()
	{
		// Arrange
		ThrowingReadStream responseStream = new();
		PassthroughStreamContent content = new(responseStream, "application/json");
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.FromResponse(
			(request, cancellationToken) => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
			{
				Content = content,
			});
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxProtocolException exception = await Assert.ThrowsAsync<TorBoxProtocolException>(
			() => transport.SendAsync<string>(client, request, CancellationToken.None));

		// Assert
		Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
		Assert.IsType<IOException>(exception.InnerException);
	}

	[Fact]
	public async Task SendAsync_WithUnsupportedContentType_ReadsAtMost64KiBForTheProtocolDiagnostic()
	{
		// Arrange
		CountingReadStream responseStream = new(Encoding.UTF8.GetBytes(new string('d', 131_072)));
		PassthroughStreamContent content = new(responseStream, "text/plain");
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.FromResponse(
			(request, cancellationToken) =>
			{
				return new HttpResponseMessage(HttpStatusCode.BadGateway)
				{
					Content = content,
				};
			});
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxProtocolException exception = await Assert.ThrowsAsync<TorBoxProtocolException>(
			() => transport.SendAsync<string>(client, request, CancellationToken.None));

		// Assert
		Assert.NotNull(exception.Detail);
		Assert.Equal(0, content.SerializeCallCount);
		Assert.Equal(1, content.CreateContentReadStreamCallCount);
		Assert.InRange(responseStream.BytesRead, 1, 65_536);
		Assert.InRange(Encoding.UTF8.GetByteCount(exception.Detail), 1, 65_536);
	}

	[Fact]
	public async Task SendAsync_WithOneRequest_SendsExactlyOnceAndForwardsCancellation()
	{
		// Arrange
		CancellationAwareHttpMessageHandler handler = new();
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using CancellationTokenSource cancellationTokenSource = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		Task<TorBoxResponse<string>> sendTask = transport.SendAsync<string>(client, request, cancellationTokenSource.Token);
		await handler.SendStarted;
		cancellationTokenSource.Cancel();
		await Assert.ThrowsAnyAsync<OperationCanceledException>(() => sendTask);

		// Assert
		Assert.Equal(1, handler.SendCount);
		Assert.True(handler.CancellationObserved);
	}

	[Fact]
	public async Task SendAsync_WithJsonEnvelope_DisposesTheHttpResponseAfterParsing()
	{
		// Arrange
		MemoryStream responseStream = new(Encoding.UTF8.GetBytes("""
		{
		  "success": true,
		  "error": null,
		  "detail": "Found.",
		  "data": "payload"
		}
		"""));
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.FromResponse(
			(request, cancellationToken) =>
			{
				HttpResponseMessage response = new(HttpStatusCode.OK)
				{
					Content = new StreamContent(responseStream),
				};
				response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				return response;
			});
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "user/me");

		// Act
		TorBoxResponse<string> response = await transport.SendAsync<string>(client, request, CancellationToken.None);

		// Assert
		Assert.True(response.Success);
		Assert.Throws<ObjectDisposedException>(() => responseStream.ReadByte());
	}

	[Fact]
	public async Task SendStreamAsync_WithBinaryResponse_TransfersTheOpenResponseAndMetadataOwnership()
	{
		// Arrange
		MemoryStream responseStream = new([1, 2, 3]);
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.FromResponse(
			(request, cancellationToken) =>
			{
				StreamContent content = new(responseStream);
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				content.Headers.ContentLength = 3;
				content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
				{
					FileNameStar = "archive.bin",
				};

				return new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = content,
				};
			});
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "torrents/requestdl");

		// Act
		using TorBoxStreamResponse response = await transport.SendStreamAsync(client, request, CancellationToken.None);

		// Assert
		Assert.True(response.Success);
		Assert.NotNull(response.Stream);
		Assert.Equal(1, response.Stream.ReadByte());
		Assert.Equal("application/octet-stream", response.MediaType);
		Assert.Equal(3, response.ContentLength);
		Assert.Equal("archive.bin", response.FileName);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task SendStreamAsync_WithRedirectResponse_TransfersTheRedirectOwnership()
	{
		// Arrange
		Uri redirectUri = new("https://downloads.torbox.app/archive.bin");
		RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.FromResponse(
			(request, cancellationToken) =>
			{
				HttpResponseMessage response = new(HttpStatusCode.TemporaryRedirect);
				response.Headers.Location = redirectUri;
				return response;
			});
		using HttpClient client = new(handler)
		{
			BaseAddress = new Uri("https://api.torbox.app/v1/api/"),
		};
		TorBoxApiTransport transport = new();
		using HttpRequestMessage request = new(HttpMethod.Get, "torrents/requestdl");

		// Act
		using TorBoxStreamResponse response = await transport.SendStreamAsync(client, request, CancellationToken.None);

		// Assert
		Assert.True(response.Success);
		Assert.Null(response.Stream);
		Assert.Equal(redirectUri, response.RedirectUri);
		Assert.Equal(HttpStatusCode.TemporaryRedirect, response.StatusCode);
	}

	[Fact]
	public void TorBoxHttpClientHandlerFactory_Create_DisablesAutomaticRedirectFollowing()
	{
		// Arrange
		HttpClientHandler handler = TorBoxHttpClientHandlerFactory.Create();

		// Act
		bool allowAutoRedirect = handler.AllowAutoRedirect;

		// Assert
		Assert.False(allowAutoRedirect);
	}

	private sealed class CancellationAwareHttpMessageHandler : HttpMessageHandler
	{
		private readonly TaskCompletionSource<object?> _sendStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);

		internal bool CancellationObserved { get; private set; }

		internal int SendCount { get; private set; }

		internal Task SendStarted => _sendStarted.Task;

		protected override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			SendCount++;
			_sendStarted.TrySetResult(null);

			TaskCompletionSource<HttpResponseMessage> response = new(TaskCreationOptions.RunContinuationsAsynchronously);
			cancellationToken.Register(
				() =>
				{
					CancellationObserved = true;
					response.TrySetCanceled(cancellationToken);
				});

			return response.Task;
		}
	}

	private sealed class CountingReadStream : MemoryStream
	{
		internal CountingReadStream(byte[] buffer)
			: base(buffer, writable: false)
		{
		}

		internal int BytesRead { get; private set; }

		public override int Read(byte[] buffer, int offset, int count)
		{
			int bytesRead = base.Read(buffer, offset, count);
			BytesRead += bytesRead;
			return bytesRead;
		}

		public override Task<int> ReadAsync(
			byte[] buffer,
			int offset,
			int count,
			CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			int bytesRead = base.Read(buffer, offset, count);
			BytesRead += bytesRead;
			return Task.FromResult(bytesRead);
		}
	}

	private sealed class PassthroughStreamContent : HttpContent
	{
		private readonly Stream _stream;

		internal PassthroughStreamContent(Stream stream, string mediaType)
		{
			_stream = stream;
			Headers.ContentType = new MediaTypeHeaderValue(mediaType);
		}

		internal int CreateContentReadStreamCallCount { get; private set; }

		internal int SerializeCallCount { get; private set; }

		protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
		{
			SerializeCallCount++;
			return _stream.CopyToAsync(stream);
		}

		protected override bool TryComputeLength(out long length)
		{
			length = _stream.CanSeek ? _stream.Length : 0;
			return _stream.CanSeek;
		}

		protected override Task<Stream> CreateContentReadStreamAsync()
		{
			CreateContentReadStreamCallCount++;
			return Task.FromResult(_stream);
		}

		protected override Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			CreateContentReadStreamCallCount++;
			return Task.FromResult(_stream);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_stream.Dispose();
			}

			base.Dispose(disposing);
		}
	}

	private sealed class ThrowingReadStream : Stream
	{
		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => throw new NotSupportedException();

		public override long Position
		{
			get => throw new NotSupportedException();
			set => throw new NotSupportedException();
		}

		public override void Flush()
		{
		}

		public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

		public override int Read(byte[] buffer, int offset, int count) => throw new IOException("The test response stream cannot be read.");

		public override Task<int> ReadAsync(
			byte[] buffer,
			int offset,
			int count,
			CancellationToken cancellationToken) => Task.FromException<int>(new IOException("The test response stream cannot be read."));

		public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

		public override void SetLength(long value) => throw new NotSupportedException();

		public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	}
}
