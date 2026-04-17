namespace TorBoxSDK.TestUtilities;

/// <summary>
/// Provides assertion helpers for verifying that API responses have no unmapped JSON fields
/// relative to the SDK model types.
/// </summary>
/// <remarks>
/// Use <see cref="NoUnmappedFieldsAsync{T}"/> in integration or live tests to assert
/// that every field in a raw API response is covered by a <c>[JsonPropertyName]</c> attribute
/// on the corresponding SDK model.
/// </remarks>
public static class SchemaAssert
{
    /// <summary>
    /// Fetches the given endpoint via <paramref name="httpClient"/>, then verifies that
    /// every JSON field in the response is mapped by a <c>[JsonPropertyName]</c> attribute
    /// in the SDK model type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The full response type including the <c>TorBoxResponse</c> wrapper
    /// (e.g., <c>TorBoxResponse&lt;IReadOnlyList&lt;Torrent&gt;&gt;</c>).
    /// </typeparam>
    /// <param name="httpClient">An <see cref="HttpClient"/> configured with the TorBox API base address and auth.</param>
    /// <param name="endpoint">The API endpoint path (e.g., <c>"/v1/api/torrents/mylist"</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A read-only list of unmapped field paths. An empty list means full coverage.
    /// Callers should assert that this list is empty.
    /// </returns>
    public static async Task<IReadOnlyList<string>> FindUnmappedFieldsAsync<T>(
        HttpClient httpClient,
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient
            .GetAsync(endpoint, cancellationToken)
            .ConfigureAwait(false);

        string json = await response.Content
            .ReadAsStringAsync(
#if NET5_0_OR_GREATER
                cancellationToken
#endif
            )
            .ConfigureAwait(false);

        return UnmappedFieldDetector.FindUnmappedFields<T>(json);
    }

    /// <summary>
    /// Builds a descriptive failure message listing all unmapped field paths.
    /// </summary>
    /// <param name="endpoint">The API endpoint that was called.</param>
    /// <param name="modelType">The primary SDK model type (without the TorBoxResponse wrapper).</param>
    /// <param name="unmapped">The list of unmapped field paths.</param>
    /// <returns>A formatted message suitable for use in <c>Assert.True</c>.</returns>
    public static string BuildFailureMessage(
        string endpoint,
        Type modelType,
        IReadOnlyList<string> unmapped) =>
        $"Endpoint '{endpoint}' returned {unmapped.Count} unmapped field path(s) " +
        $"not covered by '{modelType.Name}' (including nested types):{Environment.NewLine}" +
        string.Join(Environment.NewLine, unmapped.Select(f => $"  - {f}"));
}
