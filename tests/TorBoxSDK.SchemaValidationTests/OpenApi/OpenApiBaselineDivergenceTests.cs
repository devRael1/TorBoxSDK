using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.OpenApi;

/// <summary>
/// Locks down the named divergences of the retained Main OpenAPI baseline.
/// </summary>
/// <remarks>
/// These assertions document the baseline <c>2026-08-10</c> evidence for DEC-004 / DIV-001.
/// They do not promote either observed OpenAPI variant to the effective deployed API contract.
/// </remarks>
[Trait("Category", "Contract")]
public sealed class OpenApiBaselineDivergenceTests
{
	/// <summary>
	/// Provides fields present in the retained Main baseline but intentionally unmapped by the current SDK.
	/// </summary>
	public static TheoryData<string, string> DocumentedFieldDivergences()
	{
		TheoryData<string, string> data = [];
		data.Add("BaseSettingsModel", "subscribed_email_segments");
		data.Add("EditTorrent", "airlocked");
		data.Add("EditUsenetDownload", "airlocked");
		data.Add("EditWebDownload", "airlocked");
		return data;
	}

	[Fact]
	public async Task MainBaseline_ConditionalSchemaMappings_MatchDocumentedDiv001Absences()
	{
		// Arrange
		string[] expectedSchemaNames =
		[
			"ControlSearchEngine",
			"SearchEngineEditModel",
			"SearchEngineModel"
		];
		IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> schemas =
			await OpenApiSchemaReader.ReadFromBaselineAsync();

		// Act
		string[] actualSchemaNames = SchemaModelMapping.KnownSchemaMappingsNotInMainBaseline
			.OrderBy(schemaName => schemaName, StringComparer.Ordinal)
			.ToArray();
		string[] unexpectedlyPresentSchemaNames = actualSchemaNames
			.Where(schemas.ContainsKey)
			.ToArray();

		// Assert
		Assert.Equal(expectedSchemaNames, actualSchemaNames);
		Assert.Empty(unexpectedlyPresentSchemaNames);
	}

	[Theory]
	[MemberData(nameof(DocumentedFieldDivergences))]
	public async Task MainBaseline_DocumentedFieldDivergence_IsUnmappedAndExcluded(
		string schemaName,
		string fieldName)
	{
		// Arrange
		IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> schemas =
			await OpenApiSchemaReader.ReadFromBaselineAsync();
		Type modelType = SchemaModelMapping.SchemaToType[schemaName];
		IReadOnlySet<string> knownExclusions = SchemaModelMapping.KnownOpenApiFieldsNotInSdk
			.GetValueOrDefault(schemaName) ?? new HashSet<string>();

		// Act
		bool schemaExists = schemas.TryGetValue(
			schemaName,
			out IReadOnlyDictionary<string, string>? schemaFields);
		bool fieldExists = schemaExists && schemaFields!.ContainsKey(fieldName);
		bool fieldIsMapped = ModelReflector.GetJsonPropertyNames(modelType).Contains(fieldName);

		// Assert
		Assert.True(fieldExists, $"Baseline schema '{schemaName}' does not declare '{fieldName}'.");
		Assert.False(fieldIsMapped, $"SDK model '{modelType.Name}' now maps '{fieldName}', so the exclusion must be removed.");
		Assert.Contains(fieldName, knownExclusions);
	}
}
