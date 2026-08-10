using System;
using System.Linq;
using System.Reflection;
using TorBoxSDK;
using TorBoxSDK.Internal.Contracts.Mappers;

using GeneratedOAuthRegisterRequest = TorBoxSDK.Internal.Generated.Main.Models.OAuthRegisterRequest;
using PublicOAuthRegisterRequest = TorBoxSDK.Models.Integrations.OAuthRegisterRequest;

namespace TorboxSDK.UnitTests.InternalContracts;

public sealed class GeneratedContractVisibilityTests
{
	private const string GeneratedContractNamespacePrefix = "TorBoxSDK.Internal.Generated.";
	private const string MainRootClientTypeName = "TorBoxSDK.Internal.Generated.Main.TorBoxInternalMainClient";
	private const string RelayRootClientTypeName = "TorBoxSDK.Internal.Generated.Relay.TorBoxInternalRelayClient";

	[Fact]
	public void GeneratedContractTypes_WhenLoadedFromSdkAssembly_AreNotPublic()
	{
		// Arrange
		Assembly sdkAssembly = typeof(TorBoxClient).Assembly;

		// Act
		Type[] generatedTypes = sdkAssembly.GetTypes()
			.Where(IsGeneratedContractType)
			.ToArray();

		// Assert
		Assert.NotEmpty(generatedTypes);
		Assert.All(generatedTypes, generatedType => Assert.False(
			generatedType.IsPublic || generatedType.IsNestedPublic,
			$"Generated contract type '{generatedType.FullName}' must not be public."));
	}

	[Fact]
	public void GeneratedContractRootClients_WhenLoadedFromSdkAssembly_Exist()
	{
		// Arrange
		Assembly sdkAssembly = typeof(TorBoxClient).Assembly;

		// Act
		Type? mainRootClient = sdkAssembly.GetType(MainRootClientTypeName);
		Type? relayRootClient = sdkAssembly.GetType(RelayRootClientTypeName);

		// Assert
		Assert.NotNull(mainRootClient);
		Assert.NotNull(relayRootClient);
	}

	[Fact]
	public void OAuthRegisterRequestContractMapper_WithProviderAndTokens_MapsOnlyBodyFields()
	{
		// Arrange
		PublicOAuthRegisterRequest request = new()
		{
			Provider = "googledrive",
			Token = "access-token",
			RefreshToken = "refresh-token",
		};

		// Act
		GeneratedOAuthRegisterRequest generatedRequest = OAuthRegisterRequestContractMapper.ToGenerated(request);

		// Assert
		Assert.Equal(request.Token, generatedRequest.Token);
		Assert.Equal(request.RefreshToken, generatedRequest.RefreshToken);
		Assert.Null(typeof(GeneratedOAuthRegisterRequest).GetProperty(nameof(PublicOAuthRegisterRequest.Provider)));
		Assert.Empty(generatedRequest.AdditionalData);
	}

	private static bool IsGeneratedContractType(Type type) =>
		type.Namespace?.StartsWith(GeneratedContractNamespacePrefix, StringComparison.Ordinal) == true;
}
