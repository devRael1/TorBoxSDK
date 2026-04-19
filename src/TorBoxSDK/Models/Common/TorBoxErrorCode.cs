namespace TorBoxSDK.Models.Common;

/// <summary>
/// Enumerates the known error codes returned by the TorBox API.
/// </summary>
/// <remarks>
/// Use these codes to programmatically distinguish between different
/// failure modes when handling <see cref="TorBoxException"/>.
/// </remarks>
public enum TorBoxErrorCode
{
	/// <summary>An unknown or unmapped error code was returned.</summary>
	Unknown = 0,

	/// <summary>A database error occurred on the server.</summary>
	DatabaseError,

	/// <summary>An unspecified server-side error occurred.</summary>
	UnknownError,

	/// <summary>No authentication credentials were provided.</summary>
	NoAuth,

	/// <summary>The provided authentication token is invalid or expired.</summary>
	BadToken,

	/// <summary>An invalid option or parameter was supplied.</summary>
	InvalidOption,

	/// <summary>The authenticated user does not have permission for this action.</summary>
	PermissionDenied,

	/// <summary>The requested feature is restricted to a higher-tier plan.</summary>
	PlanRestrictedFeature,

	/// <summary>A duplicate item was detected.</summary>
	DuplicateItem,

	/// <summary>The action constitutes a breach of the terms of service.</summary>
	BreachOfTos,

	/// <summary>The active download limit has been reached.</summary>
	ActiveLimit,

	/// <summary>The seeding limit has been reached.</summary>
	SeedingLimit,

	/// <summary>Banned content was detected in the requested item.</summary>
	BannedContentDetected,

	/// <summary>The requested action could not be performed.</summary>
	CouldNotPerformAction,

	/// <summary>The requested item was not found.</summary>
	ItemNotFound,

	/// <summary>The specified device is invalid.</summary>
	InvalidDevice,

	/// <summary>The device has already been authenticated.</summary>
	DeviceAlreadyAuthed,

	/// <summary>Too many requests have been made; rate limiting is in effect.</summary>
	TooManyRequests,

	/// <summary>The requested download exceeds the maximum allowed size.</summary>
	DownloadTooLarge,

	/// <summary>A required option or parameter is missing.</summary>
	MissingRequiredOption,

	/// <summary>The user account has been banned.</summary>
	BannedUser,

	/// <summary>An error occurred during a search operation.</summary>
	SearchError,

	/// <summary>An internal server error occurred.</summary>
	ServerError,
}
