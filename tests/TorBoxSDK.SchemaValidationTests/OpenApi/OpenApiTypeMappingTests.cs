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
    private static readonly string OpenApiFilePath = OpenApiSchemaReader.FindOpenApiFilePath();

    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Schemas =
        OpenApiSchemaReader.Read(OpenApiFilePath);

    /// <summary>
    /// Provides one row per (schema, type, fieldName, openApiType) tuple for the parameterised test.
    /// Only fields that are actually mapped in both the spec and the SDK are included.
    /// </summary>
    public static TheoryData<string, Type, string, string> MappedFields()
    {
        TheoryData<string, Type, string, string> data = new();

        foreach ((string schemaName, Type modelType) in SchemaModelMapping.SchemaToType)
        {
            if (!Schemas.TryGetValue(schemaName, out IReadOnlyDictionary<string, string>? schemaFields))
                continue;

            IReadOnlyDictionary<string, PropertyInfo> propMap =
                ModelReflector.GetJsonPropertyMap(modelType);

            IReadOnlySet<string> knownAbsent = SchemaModelMapping.KnownOpenApiFieldsNotInSdk
                .GetValueOrDefault(schemaName) ?? new HashSet<string>();

            foreach ((string fieldName, string openApiType) in schemaFields)
            {
                // Skip fields intentionally not mapped in the SDK.
                if (!propMap.ContainsKey(fieldName) || knownAbsent.Contains(fieldName))
                    continue;

                // Skip fields with known type mismatches between OpenAPI spec and SDK serialization.
                IReadOnlySet<string> knownMismatch = SchemaModelMapping.KnownTypeMismatches
                    .GetValueOrDefault(schemaName) ?? new HashSet<string>();
                if (knownMismatch.Contains(fieldName))
                    continue;

                data.Add(schemaName, modelType, fieldName, openApiType);
            }
        }

        return data;
    }

    /// <summary>
    /// Each mapped SDK property must use a .NET type that is compatible with the OpenAPI
    /// type category declared for the same field.
    /// </summary>
    [Theory]
    [MemberData(nameof(MappedFields))]
    public void MappedField_HasCompatibleType(
        string schemaName,
        Type modelType,
        string fieldName,
        string openApiType)
    {
        // Arrange
        IReadOnlyDictionary<string, PropertyInfo> propMap =
            ModelReflector.GetJsonPropertyMap(modelType);
        PropertyInfo prop = propMap[fieldName];

        string dotNetCategory = SchemaModelMapping.MapDotNetTypeToOpenApiType(prop.PropertyType);
        string baseOpenApiType = ExtractBaseType(openApiType);

        // Act & Assert
        Assert.True(
            AreCompatible(dotNetCategory, baseOpenApiType),
            $"Type mismatch for '{schemaName}.{fieldName}': " +
            $"OpenAPI declares '{openApiType}' but '{modelType.Name}.{prop.Name}' " +
            $"({prop.PropertyType.Name}) maps to OpenAPI category '{dotNetCategory}'.");
    }

    // ── Private helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Extracts the primary (non-null) type from a possibly-nullable OpenAPI type string
    /// such as <c>"string|null"</c> or <c>"ControlSearchEngineOperation|null"</c>.
    /// </summary>
    private static string ExtractBaseType(string openApiType)
    {
        if (!openApiType.Contains('|'))
            return openApiType;

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
            return true;

        // integer ↔ number are both numeric in JSON — allow either direction.
        if ((dotNetCategory == "integer" && openApiBase == "number") ||
            (dotNetCategory == "number" && openApiBase == "integer"))
            return true;

        // DateTimeOffset is serialised as an ISO-8601 string, which is "string" in OpenAPI.
        // Enum types also serialise as strings.
        // $ref types (e.g., "ControlSearchEngineOperation") are enum references — string-compatible.
        if (dotNetCategory == "string" &&
            openApiBase != "boolean" &&
            openApiBase != "integer" &&
            openApiBase != "number" &&
            openApiBase != "array" &&
            openApiBase != "object")
            return true;

        return false;
    }
}
