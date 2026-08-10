using GeneratedOAuthRegisterRequest = TorBoxSDK.Internal.Generated.Main.Models.OAuthRegisterRequest;
using PublicOAuthRegisterRequest = TorBoxSDK.Models.Integrations.OAuthRegisterRequest;

namespace TorBoxSDK.Internal.Contracts.Mappers;

/// <summary>
/// Converts the manual OAuth registration request into its generated Main API body.
/// </summary>
/// <remarks>
/// The public <c>Provider</c> value is intentionally not copied because it belongs to the
/// endpoint path, not to the JSON request body.
/// </remarks>
internal static class OAuthRegisterRequestContractMapper
{
	/// <summary>
	/// Converts the public request body fields to the generated contract.
	/// </summary>
	/// <param name="request">The manually curated OAuth registration request.</param>
	/// <returns>The generated request body for the Main API.</returns>
	internal static GeneratedOAuthRegisterRequest ToGenerated(PublicOAuthRegisterRequest request)
	{
		ArgumentNullException.ThrowIfNull(request);

		return new GeneratedOAuthRegisterRequest
		{
			Token = request.Token,
			RefreshToken = request.RefreshToken,
		};
	}
}
