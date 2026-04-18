using System.Reflection;
using System.Text.Json;

namespace TorBoxSDK.TestUtilities;

/// <summary>
/// Detects JSON fields present in a raw API response that are not mapped to any property
/// via <c>[JsonPropertyName]</c> in the corresponding SDK model type.
/// </summary>
/// <remarks>
/// No deserialization into the target type is performed; the detection is purely structural,
/// comparing the JSON property names against the <c>[JsonPropertyName]</c> attribute values
/// obtained via reflection. Nested objects and collections are traversed recursively.
/// </remarks>
public static class UnmappedFieldDetector
{
    /// <summary>
    /// Analyses a raw JSON string from an API response and returns every JSON field path
    /// that has no matching <c>[JsonPropertyName]</c> attribute in <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The SDK model type to check against (e.g., <c>TorBoxResponse&lt;IReadOnlyList&lt;Torrent&gt;&gt;</c>).
    /// </typeparam>
    /// <param name="json">Raw JSON string returned by the TorBox API.</param>
    /// <returns>
    /// A read-only list of dot-separated JSON paths (e.g., <c>"data[].files[].short_name"</c>)
    /// for every field that is not covered by the SDK model.
    /// An empty list means full coverage.
    /// </returns>
    public static IReadOnlyList<string> FindUnmappedFields<T>(string json)
    {
        using var doc = JsonDocument.Parse(json);
        List<string> unmapped = [];
        TraverseElement(doc.RootElement, typeof(T), pathPrefix: string.Empty, unmapped);
        return unmapped;
    }

    // ── Private helpers ──────────────────────────────────────────────────────────────

    private static void TraverseElement(
        JsonElement element,
        Type targetType,
        string pathPrefix,
        List<string> unmapped)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        IReadOnlyDictionary<string, PropertyInfo> knownProps =
            ModelReflector.GetJsonPropertyMap(targetType);

        foreach (JsonProperty jsonProp in element.EnumerateObject())
        {
            string currentPath = string.IsNullOrEmpty(pathPrefix)
                ? jsonProp.Name
                : $"{pathPrefix}.{jsonProp.Name}";

            if (!knownProps.TryGetValue(jsonProp.Name, out PropertyInfo? propInfo))
            {
                unmapped.Add(currentPath);
                continue;
            }

            // Unwrap Nullable<T> for value types (e.g., bool? → bool).
            // Nullable reference types are not wrapped in Nullable<T> at runtime.
            Type propType = Nullable.GetUnderlyingType(propInfo.PropertyType)
                ?? propInfo.PropertyType;

            switch (jsonProp.Value.ValueKind)
            {
                case JsonValueKind.Object when IsComplexType(propType):
                    TraverseElement(jsonProp.Value, propType, currentPath, unmapped);
                    break;

                case JsonValueKind.Array:
                    Type? elementType = GetCollectionElementType(propType);
                    if (elementType is not null && IsComplexType(elementType))
                    {
                        // All array items share the same shape; checking the first is sufficient.
                        foreach (JsonElement item in jsonProp.Value.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.Object)
                            {
                                TraverseElement(item, elementType, $"{currentPath}[]", unmapped);
                            }

                            break;
                        }
                    }
                    break;
            }
        }
    }

    private static Type? GetCollectionElementType(Type type)
    {
        if (!type.IsGenericType)
        {
            return null;
        }

        Type def = type.GetGenericTypeDefinition();
        return def == typeof(IReadOnlyList<>) || def == typeof(List<>) ||
            def == typeof(IEnumerable<>) || def == typeof(IReadOnlyCollection<>) ||
            def == typeof(ICollection<>)
            ? type.GetGenericArguments()[0]
            : null;
    }

    /// <summary>
    /// Returns <see langword="true"/> for types whose JSON representation is an object
    /// with named properties (i.e., types that have <c>[JsonPropertyName]</c> attributes to check).
    /// </summary>
    private static bool IsComplexType(Type type) =>
        type.IsClass && type != typeof(string) && type != typeof(object);
}
