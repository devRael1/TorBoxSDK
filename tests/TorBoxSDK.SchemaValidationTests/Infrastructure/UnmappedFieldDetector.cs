namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

/// <summary>
/// Detects JSON fields present in a raw API response that are not mapped to any property
/// via <c>[JsonPropertyName]</c> in the corresponding SDK model type.
/// Delegates to <see cref="TestUtilities.UnmappedFieldDetector"/> for the actual implementation.
/// </summary>
internal static class UnmappedFieldDetector
{
    /// <inheritdoc cref="TestUtilities.UnmappedFieldDetector.FindUnmappedFields{T}"/>
    public static IReadOnlyList<string> FindUnmappedFields<T>(string json) =>
        TestUtilities.UnmappedFieldDetector.FindUnmappedFields<T>(json);
}
