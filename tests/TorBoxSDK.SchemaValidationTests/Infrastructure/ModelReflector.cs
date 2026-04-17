using System.Reflection;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

/// <summary>
/// Provides reflection-based utilities for extracting JSON property metadata from SDK model types.
/// Delegates to <see cref="TestUtilities.ModelReflector"/> for the actual implementation.
/// </summary>
internal static class ModelReflector
{
    /// <inheritdoc cref="TestUtilities.ModelReflector.GetJsonPropertyNames"/>
    public static IReadOnlySet<string> GetJsonPropertyNames(Type type) =>
        TestUtilities.ModelReflector.GetJsonPropertyNames(type);

    /// <inheritdoc cref="TestUtilities.ModelReflector.GetJsonPropertyMap"/>
    public static IReadOnlyDictionary<string, PropertyInfo> GetJsonPropertyMap(Type type) =>
        TestUtilities.ModelReflector.GetJsonPropertyMap(type);
}
