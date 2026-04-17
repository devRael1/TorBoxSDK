using System.Reflection;
using System.Text.Json.Serialization;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

/// <summary>
/// Provides reflection-based utilities for extracting JSON property metadata from SDK model types.
/// </summary>
internal static class ModelReflector
{
    /// <summary>
    /// Returns all JSON property names declared on the given type via
    /// <see cref="JsonPropertyNameAttribute"/>.
    /// </summary>
    /// <param name="type">The SDK model type to inspect.</param>
    /// <returns>
    /// A read-only set of JSON property names exactly as they appear in the
    /// <see cref="JsonPropertyNameAttribute"/> values on the type's public properties.
    /// </returns>
    public static IReadOnlySet<string> GetJsonPropertyNames(Type type)
    {
        HashSet<string> names = new(StringComparer.Ordinal);

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            JsonPropertyNameAttribute? attr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (attr is not null)
                names.Add(attr.Name);
        }

        return names;
    }

    /// <summary>
    /// Returns a mapping from JSON property name to <see cref="PropertyInfo"/> for the given type.
    /// </summary>
    /// <param name="type">The SDK model type to inspect.</param>
    /// <returns>
    /// A dictionary mapping each JSON property name (from <see cref="JsonPropertyNameAttribute"/>)
    /// to its corresponding <see cref="PropertyInfo"/>.
    /// </returns>
    public static IReadOnlyDictionary<string, PropertyInfo> GetJsonPropertyMap(Type type)
    {
        Dictionary<string, PropertyInfo> map = new(StringComparer.Ordinal);

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            JsonPropertyNameAttribute? attr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (attr is not null)
                map[attr.Name] = prop;
        }

        return map;
    }
}
