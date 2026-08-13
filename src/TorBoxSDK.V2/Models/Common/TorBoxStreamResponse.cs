using System.Net;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents an owning TorBox response for stream and redirect operations.
/// </summary>
/// <remarks>
/// Dispose this response after consuming <see cref="Stream"/>. Structured API failures
/// remain response values and do not expose a stream.
/// </remarks>
public sealed class TorBoxStreamResponse : IDisposable
{
	private readonly HttpResponseMessage? _ownedResponse;
	private bool _disposed;

	internal TorBoxStreamResponse(
		HttpResponseMessage? ownedResponse,
		bool success,
		HttpStatusCode statusCode,
		System.IO.Stream? stream = null,
		string? error = null,
		string? detail = null,
		string? mediaType = null,
		long? contentLength = null,
		string? fileName = null,
		Uri? redirectUri = null)
	{
		if (success && ownedResponse is null)
		{
			throw new ArgumentNullException(nameof(ownedResponse), "A successful stream response must own its HTTP response.");
		}

		if (success && (stream is null) == (redirectUri is null))
		{
			throw new ArgumentException("A successful response must contain exactly one stream or redirect URI.", nameof(stream));
		}

		if (!success && (ownedResponse is not null || stream is not null || redirectUri is not null))
		{
			throw new ArgumentException("A structured API failure cannot own an HTTP response, content stream, or redirect URI.", nameof(ownedResponse));
		}

		_ownedResponse = ownedResponse;
		Success = success;
		StatusCode = statusCode;
		Stream = stream;
		Error = TorBoxProtocolException.BoundDiagnostic(error);
		Detail = TorBoxProtocolException.BoundDiagnostic(detail);
		MediaType = mediaType;
		ContentLength = contentLength;
		FileName = fileName;
		RedirectUri = redirectUri;
	}

	/// <summary>
	/// Gets a value indicating whether the API reports success.
	/// </summary>
	public bool Success { get; }

	/// <summary>
	/// Gets the bounded TorBox error identifier, or <see langword="null"/> when none was returned.
	/// </summary>
	public string? Error { get; }

	/// <summary>
	/// Gets the bounded response detail, or <see langword="null"/> when none was returned.
	/// </summary>
	public string? Detail { get; }

	/// <summary>
	/// Gets the unbuffered content stream, or <see langword="null"/> for failures and streamless redirects.
	/// </summary>
	public System.IO.Stream? Stream { get; }

	/// <summary>
	/// Gets the content media type, or <see langword="null"/> when the server did not provide one.
	/// </summary>
	public string? MediaType { get; }

	/// <summary>
	/// Gets the content length, or <see langword="null"/> when the server did not provide one.
	/// </summary>
	public long? ContentLength { get; }

	/// <summary>
	/// Gets the suggested file name, or <see langword="null"/> when the server did not provide one.
	/// </summary>
	public string? FileName { get; }

	/// <summary>
	/// Gets the redirect destination, or <see langword="null"/> when the response is not a redirect.
	/// </summary>
	public Uri? RedirectUri { get; }

	/// <summary>
	/// Gets the HTTP status associated with the response.
	/// </summary>
	public HttpStatusCode StatusCode { get; }

	internal static TorBoxStreamResponse CreateForTesting(
		System.IO.Stream? stream,
		HttpStatusCode statusCode,
		bool success = true,
		string? error = null,
		string? detail = null,
		string? mediaType = null,
		long? contentLength = null,
		string? fileName = null,
		Uri? redirectUri = null)
	{
		HttpResponseMessage? ownedResponse = null;

		if (success)
		{
			ownedResponse = new HttpResponseMessage(statusCode);

			if (stream is not null)
			{
				ownedResponse.Content = new StreamContent(stream);
			}
		}

		return new TorBoxStreamResponse(
			ownedResponse,
			success,
			statusCode,
			stream,
			error,
			detail,
			mediaType,
			contentLength,
			fileName,
			redirectUri);
	}

	/// <summary>
	/// Disposes the owned HTTP response and its content stream.
	/// </summary>
	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;

		try
		{
			Stream?.Dispose();
		}
		finally
		{
			_ownedResponse?.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
