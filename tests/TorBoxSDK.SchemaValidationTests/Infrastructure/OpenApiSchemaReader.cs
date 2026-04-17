using System.Text.Json;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

/// <summary>
/// Reads and parses the TorBox OpenAPI specification to extract schema property definitions.
/// </summary>
internal static class OpenApiSchemaReader
{
    /// <summary>
    /// Reads the OpenAPI specification from the given file path and returns all schema definitions.
    /// </summary>
    /// <param name="filePath">Path to the <c>open_api.json</c> file.</param>
    /// <returns>
    /// A dictionary keyed by schema name. Each value is a dictionary of property name
    /// to its OpenAPI type string (e.g., <c>"string"</c>, <c>"integer"</c>,
    /// <c>"boolean"</c>, <c>"array"</c>, <c>"object"</c>, or a <c>$ref</c> type name).
    /// </returns>
    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Read(string filePath)
    {
        string json = File.ReadAllText(filePath);
        using JsonDocument doc = JsonDocument.Parse(json);

        Dictionary<string, IReadOnlyDictionary<string, string>> result =
            new(StringComparer.Ordinal);

        if (!doc.RootElement.TryGetProperty("components", out JsonElement components))
            return result;

        if (!components.TryGetProperty("schemas", out JsonElement schemas))
            return result;

        foreach (JsonProperty schema in schemas.EnumerateObject())
        {
            if (!schema.Value.TryGetProperty("properties", out JsonElement props))
                continue;

            Dictionary<string, string> properties = new(StringComparer.Ordinal);
            foreach (JsonProperty prop in props.EnumerateObject())
            {
                properties[prop.Name] = ExtractType(prop.Value);
            }

            result[schema.Name] = properties;
        }

        return result;
    }

    /// <summary>
    /// Locates the <c>open_api.json</c> file next to the test assembly.
    /// </summary>
    /// <exception cref="FileNotFoundException">
    /// Thrown when <c>open_api.json</c> is not found in the output directory.
    /// Ensure it is linked and configured with <c>CopyToOutputDirectory</c> in the csproj.
    /// </exception>
    public static string FindOpenApiFilePath()
    {
        string candidate = Path.Combine(AppContext.BaseDirectory, "open_api.json");
        return File.Exists(candidate)
            ? candidate
            : throw new FileNotFoundException(
                $"open_api.json not found in '{AppContext.BaseDirectory}'. " +
                "Ensure the file is linked and set to CopyToOutputDirectory in the csproj.",
                candidate);
    }

    private static string ExtractType(JsonElement propDef)
    {
        if (propDef.TryGetProperty("type", out JsonElement typeElem))
            return typeElem.GetString() ?? "unknown";

        if (propDef.TryGetProperty("$ref", out JsonElement refElem))
            return refElem.GetString()?.Split('/').Last() ?? "unknown";

        if (propDef.TryGetProperty("anyOf", out JsonElement anyOf))
        {
            List<string> parts = [];
            foreach (JsonElement item in anyOf.EnumerateArray())
            {
                if (item.TryGetProperty("type", out JsonElement t))
                    parts.Add(t.GetString() ?? "null");
                else if (item.TryGetProperty("$ref", out JsonElement r))
                    parts.Add(r.GetString()?.Split('/').Last() ?? "unknown");
                else
                    parts.Add("null");
            }
            return string.Join("|", parts);
        }

        return "unknown";
    }
}
