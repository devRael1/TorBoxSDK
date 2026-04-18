using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.OpenApi;

/// <summary>
/// Verifies bidirectional field coverage between the TorBox OpenAPI specification
/// (fetched from <c>https://api.torbox.app/openapi.json</c>) and the corresponding SDK model types.
/// </summary>
/// <remarks>
/// <para>
/// <b>OpenApiSchema_AllFieldsMappedInSdk</b> — fails when the API adds a new field to its
/// specification that the SDK model does not yet declare. This is the primary regression signal:
/// a failing test means the SDK is missing a field.
/// </para>
/// <para>
/// <b>SdkModel_NoExtraFieldsBeyondOpenApiSchema</b> — fails when the SDK declares a JSON property
/// that does not exist in the OpenAPI specification. This may indicate a typo, a removed API field,
/// or a documented SDK extension (which should be recorded in
/// <see cref="SchemaModelMapping.KnownSdkFieldsNotInOpenApi"/>).
/// </para>
/// <para>
/// Known intentional discrepancies are recorded in <see cref="SchemaModelMapping"/> and are
/// excluded from both tests. Update that class when the SDK intentionally diverges from the spec.
/// </para>
/// </remarks>
public sealed class OpenApiFieldCoverageTests
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _schemas = OpenApiSchemaReader.ReadFromApi();

    /// <summary>
    /// Provides one row per schema→type mapping for the parameterised tests.
    /// </summary>
    public static TheoryData<string, Type> MappedSchemas()
    {
        TheoryData<string, Type> data = [];
        foreach ((string schemaName, Type modelType) in SchemaModelMapping.SchemaToType)
        {
            data.Add(schemaName, modelType);
        }

        return data;
    }

    /// <summary>
    /// Every field defined in the OpenAPI schema must have a corresponding
    /// <c>[JsonPropertyName]</c> on the SDK model type (unless it is listed as a known exclusion).
    /// </summary>
    [Theory]
    [MemberData(nameof(MappedSchemas))]
    public void OpenApiSchema_AllFieldsMappedInSdk(string schemaName, Type modelType)
    {
        // Arrange
        Assert.True(
            _schemas.TryGetValue(schemaName, out IReadOnlyDictionary<string, string>? schemaFields),
            $"Schema '{schemaName}' was not found in the OpenAPI specification.");

        IReadOnlySet<string> sdkJsonNames = ModelReflector.GetJsonPropertyNames(modelType);
        IReadOnlySet<string> knownAbsent = SchemaModelMapping.KnownOpenApiFieldsNotInSdk
            .GetValueOrDefault(schemaName) ?? new HashSet<string>();

        // Act
        var missingInSdk = schemaFields!.Keys
            .Where(field => !sdkJsonNames.Contains(field) && !knownAbsent.Contains(field))
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToList();

        // Assert
        Assert.True(
            missingInSdk.Count == 0,
            $"OpenAPI schema '{schemaName}' has {missingInSdk.Count} field(s) not mapped in " +
            $"'{modelType.Name}':{Environment.NewLine}" +
            string.Join(Environment.NewLine, missingInSdk.Select(f => $"  - {f}")));
    }

    /// <summary>
    /// The SDK model must not declare JSON properties that do not exist in the OpenAPI schema
    /// (unless they are listed as known SDK extensions).
    /// </summary>
    [Theory]
    [MemberData(nameof(MappedSchemas))]
    public void SdkModel_NoExtraFieldsBeyondOpenApiSchema(string schemaName, Type modelType)
    {
        // Arrange
        Assert.True(
            _schemas.TryGetValue(schemaName, out IReadOnlyDictionary<string, string>? schemaFields),
            $"Schema '{schemaName}' was not found in the OpenAPI specification.");

        IReadOnlySet<string> sdkJsonNames = ModelReflector.GetJsonPropertyNames(modelType);
        IReadOnlySet<string> knownExtra = SchemaModelMapping.KnownSdkFieldsNotInOpenApi
            .GetValueOrDefault(schemaName) ?? new HashSet<string>();

        // Act
        var extraInSdk = sdkJsonNames
            .Where(field => !schemaFields!.ContainsKey(field) && !knownExtra.Contains(field))
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToList();

        // Assert
        Assert.True(
            extraInSdk.Count == 0,
            $"SDK model '{modelType.Name}' has {extraInSdk.Count} JSON property name(s) not " +
            $"defined in OpenAPI schema '{schemaName}':{Environment.NewLine}" +
            string.Join(Environment.NewLine, extraInSdk.Select(f => $"  - {f}")));
    }
}
