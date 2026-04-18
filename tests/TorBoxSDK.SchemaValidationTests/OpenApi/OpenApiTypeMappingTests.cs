using System.Reflection;
using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.OpenApi;

/// <summary>
/// Verifies that the C# property types in SDK models are compatible with the corresponding
/// OpenAPI type definitions for each mapped schema field.
/// </summary>
/// <remarks>
/// Type compatibility is checked at the OpenAPI type-category level
/// (string / integer / boolean / array / object) rather than at the exact C# type level,
/// because the API specification does not encode the full precision of C# types
/// (e.g., <c>long</c> vs <c>int</c> are both <c>integer</c> in OpenAPI).
/// </remarks>
public sealed class OpenApiTypeMappingTests
{
    private static readonly Lazy<Task<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>>> _schemas = new(OpenApiSchemaReader.ReadFromApiAsync);

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
    /// Each mapped SDK property must use a .NET type that is compatible with the OpenAPI
    /// type category declared for the same field.
    /// </summary>
    [Theory]
    [MemberData(nameof(MappedSchemas))]
    public async Task MappedSchemaFields_HaveCompatibleTypes(string schemaName, Type modelType)
    {
        // Arrange
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> schemas = await _schemas.Value;
        Assert.True(
            schemas.TryGetValue(schemaName, out IReadOnlyDictionary<string, string>? schemaFields),
            $"Schema '{schemaName}' was not found in the OpenAPI specification.");
        Assert.NotNull(schemaFields);

        IReadOnlyDictionary<string, PropertyInfo> propMap =
            ModelReflector.GetJsonPropertyMap(modelType);
        IReadOnlySet<string> knownAbsent = SchemaModelMapping.KnownOpenApiFieldsNotInSdk
            .GetValueOrDefault(schemaName) ?? new HashSet<string>();
        IReadOnlySet<string> knownMismatch = SchemaModelMapping.KnownTypeMismatches
            .GetValueOrDefault(schemaName) ?? new HashSet<string>();

        // Act
        List<string> mismatches = [];
        foreach ((string fieldName, string openApiType) in schemaFields)
        {
            if (!propMap.TryGetValue(fieldName, out PropertyInfo? prop) ||
                knownAbsent.Contains(fieldName) ||
                knownMismatch.Contains(fieldName))
            {
                continue;
            }

            string dotNetCategory = SchemaModelMapping.MapDotNetTypeToOpenApiType(prop.PropertyType);
            string baseOpenApiType = ExtractBaseType(openApiType);
            if (!AreCompatible(dotNetCategory, baseOpenApiType))
            {
                mismatches.Add(
                    $"  - {schemaName}.{fieldName}: OpenAPI '{openApiType}' vs " +
                    $"{modelType.Name}.{prop.Name} ({prop.PropertyType.Name}) -> '{dotNetCategory}'");
            }
        }

        // Assert
        Assert.True(
            mismatches.Count == 0,
            $"Type mismatch(es) found for schema '{schemaName}':{Environment.NewLine}" +
            string.Join(Environment.NewLine, mismatches));
    }

    // ── Private helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Extracts the primary (non-null) type from a possibly-nullable OpenAPI type string
    /// such as <c>"string|null"</c> or <c>"ControlSearchEngineOperation|null"</c>.
    /// </summary>
    private static string ExtractBaseType(string openApiType)
    {
        if (!openApiType.Contains('|'))
        {
            return openApiType;
        }

        string? nonNull = openApiType
            .Split('|')
            .FirstOrDefault(p => !string.Equals(p, "null", StringComparison.Ordinal));

        return nonNull ?? openApiType;
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="dotNetCategory"/> and
    /// <paramref name="openApiBase"/> represent compatible JSON types.
    /// </summary>
    private static bool AreCompatible(string dotNetCategory, string openApiBase)
    {
        if (string.Equals(dotNetCategory, openApiBase, StringComparison.Ordinal))
        {
            return true;
        }

        // integer ↔ number are both numeric in JSON — allow either direction.
        if ((dotNetCategory == "integer" && openApiBase == "number") ||
            (dotNetCategory == "number" && openApiBase == "integer"))
        {
            return true;
        }

        // DateTimeOffset is serialised as an ISO-8601 string, which is "string" in OpenAPI.
        // Enum types also serialise as strings.
        // $ref types (e.g., "ControlSearchEngineOperation") are enum references — string-compatible.
        if (dotNetCategory == "string" &&
            openApiBase != "boolean" &&
            openApiBase != "integer" &&
            openApiBase != "number" &&
            openApiBase != "array" &&
            openApiBase != "object")
        {
            return true;
        }

        return false;
    }
}
