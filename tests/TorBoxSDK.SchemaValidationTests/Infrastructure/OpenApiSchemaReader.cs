using System.Text.Json;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

/// <summary>
/// Reads and parses the TorBox OpenAPI specification to extract schema property definitions.
/// The specification is fetched from the public TorBox API endpoint and cached for the
/// lifetime of the process.
/// </summary>
internal static class OpenApiSchemaReader
{
    /// <summary>
    /// Public URL of the TorBox OpenAPI specification.
    /// </summary>
    internal const string OpenApiUrl = "https://api.torbox.app/openapi.json";

    private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> _cachedSchemas = new(FetchAndParse);

    /// <summary>
    /// Returns all schema definitions from the TorBox OpenAPI specification.
    /// The specification is downloaded once from <see cref="OpenApiUrl"/> and cached
    /// for the lifetime of the process.
    /// </summary>
    /// <returns>
    /// A dictionary keyed by schema name. Each value is a dictionary of property name
    /// to its OpenAPI type string (e.g., <c>"string"</c>, <c>"integer"</c>,
    /// <c>"boolean"</c>, <c>"array"</c>, <c>"object"</c>, or a <c>$ref</c> type name).
    /// </returns>
    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> ReadFromApi() => _cachedSchemas.Value;

    /// <summary>
    /// Parses the OpenAPI specification from a raw JSON string.
    /// </summary>
    /// <param name="json">The raw JSON content of the OpenAPI specification.</param>
    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);

        Dictionary<string, IReadOnlyDictionary<string, string>> result = [];

        if (!doc.RootElement.TryGetProperty("components", out JsonElement components))
        {
            return result;
        }

        if (!components.TryGetProperty("schemas", out JsonElement schemas))
        {
            return result;
        }

        foreach (JsonProperty schema in schemas.EnumerateObject())
        {
            if (!schema.Value.TryGetProperty("properties", out JsonElement props))
            {
                continue;
            }

            Dictionary<string, string> properties = [];
            foreach (JsonProperty prop in props.EnumerateObject())
            {
                properties[prop.Name] = ExtractType(prop.Value);
            }

            result[schema.Name] = properties;
        }

        return result;
    }

    private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> FetchAndParse()
    {
        using HttpClient client = new();
        string json = client.GetStringAsync(OpenApiUrl).GetAwaiter().GetResult();
        return Parse(json);
    }

    private static string ExtractType(JsonElement propDef)
    {
        if (propDef.TryGetProperty("type", out JsonElement typeElem))
        {
            return typeElem.GetString() ?? "unknown";
        }

        if (propDef.TryGetProperty("$ref", out JsonElement refElem))
        {
            return refElem.GetString()?.Split('/').Last() ?? "unknown";
        }

        if (propDef.TryGetProperty("anyOf", out JsonElement anyOf))
        {
            List<string> parts = [];
            foreach (JsonElement item in anyOf.EnumerateArray())
            {
                if (item.TryGetProperty("type", out JsonElement t))
                {
                    parts.Add(t.GetString() ?? "null");
                }
                else if (item.TryGetProperty("$ref", out JsonElement r))
                {
                    parts.Add(r.GetString()?.Split('/').Last() ?? "unknown");
                }
                else
                {
                    parts.Add("null");
                }
            }
            return string.Join("|", parts);
        }

        return "unknown";
    }
}
