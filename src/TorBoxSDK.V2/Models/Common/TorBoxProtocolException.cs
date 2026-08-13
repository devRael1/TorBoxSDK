using System.Net;
using System.Text;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents an HTTP response that cannot be interpreted according to the TorBox endpoint contract.
/// </summary>
public sealed class TorBoxProtocolException : Exception
{
	private const int MaxDiagnosticByteCount = 64 * 1024;

	/// <summary>
	/// Initializes a new instance of the <see cref="TorBoxProtocolException"/> class.
	/// </summary>
	/// <param name="message">The protocol failure message.</param>
	/// <param name="requestUri">The request URI, or <see langword="null"/> when it is unavailable.</param>
	/// <param name="statusCode">The HTTP status, or <see langword="null"/> when no response was received.</param>
	/// <param name="detail">The diagnostic response detail to retain within the bounded limit.</param>
	/// <param name="innerException">The exception that caused the protocol failure, if any.</param>
	public TorBoxProtocolException(
		string message,
		Uri? requestUri,
		HttpStatusCode? statusCode,
		string? detail,
		Exception? innerException = null)
		: base(message, innerException)
	{
		RequestUri = requestUri;
		StatusCode = statusCode;
		Detail = BoundDiagnostic(detail);
	}

	/// <summary>
	/// Gets the request URI, or <see langword="null"/> when it is unavailable.
	/// </summary>
	public Uri? RequestUri { get; }

	/// <summary>
	/// Gets the HTTP status, or <see langword="null"/> when no response was received.
	/// </summary>
	public HttpStatusCode? StatusCode { get; }

	/// <summary>
	/// Gets the retained diagnostic detail, bounded to 65,536 UTF-8 bytes.
	/// </summary>
	public string? Detail { get; }

	private static string? BoundDiagnostic(string? detail)
	{
		if (detail is null || Encoding.UTF8.GetByteCount(detail) <= MaxDiagnosticByteCount)
		{
			return detail;
		}

		int retainedByteCount = 0;
		int retainedCharacterCount = 0;

		while (retainedCharacterCount < detail.Length)
		{
			char character = detail[retainedCharacterCount];
			int characterCount = 1;
			int byteCount;

			if (char.IsHighSurrogate(character)
				&& retainedCharacterCount + 1 < detail.Length
				&& char.IsLowSurrogate(detail[retainedCharacterCount + 1]))
			{
				characterCount = 2;
				byteCount = 4;
			}
			else if (character <= '\u007F')
			{
				byteCount = 1;
			}
			else if (character <= '\u07FF')
			{
				byteCount = 2;
			}
			else
			{
				byteCount = 3;
			}

			if (retainedByteCount + byteCount > MaxDiagnosticByteCount)
			{
				break;
			}

			retainedByteCount += byteCount;
			retainedCharacterCount += characterCount;
		}

		return detail.Substring(0, retainedCharacterCount);
	}
}
