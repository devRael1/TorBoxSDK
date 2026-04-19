namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents an error returned by the TorBox API.
/// </summary>
/// <remarks>
/// <see cref="TorBoxException"/> carries the <see cref="ErrorCode"/> and optional
/// <see cref="Detail"/> from the API response, making it easy to handle specific
/// failure scenarios in calling code.
/// </remarks>
public sealed class TorBoxException : Exception
{
	/// <summary>
	/// Gets the TorBox-specific error code associated with this exception.
	/// </summary>
	public TorBoxErrorCode ErrorCode { get; }

	/// <summary>
	/// Gets additional detail about the error from the API response,
	/// or <see langword="null"/> if none was provided.
	/// </summary>
	public string? Detail { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TorBoxException"/> class.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="errorCode">The TorBox-specific error code.</param>
	/// <param name="detail">Optional additional detail from the API response.</param>
	public TorBoxException(string message, TorBoxErrorCode errorCode, string? detail)
		: base(message)
	{
		ErrorCode = errorCode;
		Detail = detail;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TorBoxException"/> class
	/// with an inner exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="errorCode">The TorBox-specific error code.</param>
	/// <param name="detail">Optional additional detail from the API response.</param>
	/// <param name="innerException">The exception that is the cause of this exception.</param>
	public TorBoxException(string message, TorBoxErrorCode errorCode, string? detail, Exception innerException)
		: base(message, innerException)
	{
		ErrorCode = errorCode;
		Detail = detail;
	}
}
