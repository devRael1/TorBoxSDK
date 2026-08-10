using System.Text.Json;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

internal static class OpenApiSchemaReader
{
	internal const string MainOpenApiSnapshotId = "main-openapi";

	private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> _cachedSchemas =
		new(ReadAndParseBaseline);

	internal static Task<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> ReadFromBaselineAsync() =>
		Task.FromResult(_cachedSchemas.Value);

	// Retained while the static test files migrate to the explicit baseline name.
	internal static Task<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> ReadFromApiAsync() =>
		ReadFromBaselineAsync();

	internal static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Parse(string json)
	{
		ArgumentNullException.ThrowIfNull(json);

		using JsonDocument document = JsonDocument.Parse(json);
		Dictionary<string, IReadOnlyDictionary<string, string>> result = new(StringComparer.Ordinal);
		if (!document.RootElement.TryGetProperty("components", out JsonElement components) ||
			components.ValueKind != JsonValueKind.Object ||
			!components.TryGetProperty("schemas", out JsonElement schemas) ||
			schemas.ValueKind != JsonValueKind.Object)
		{
			return result;
		}

		foreach (JsonProperty schema in schemas.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
		{
			if (!schema.Value.TryGetProperty("properties", out JsonElement properties) ||
				properties.ValueKind != JsonValueKind.Object)
			{
				continue;
			}

			Dictionary<string, string> fields = new(StringComparer.Ordinal);
			foreach (JsonProperty property in properties.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
			{
				fields[property.Name] = ExtractType(property.Value);
			}

			result[schema.Name] = fields;
		}

		return result;
	}

	private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> ReadAndParseBaseline()
	{
		ContractBaseline baseline = ContractBaselineReader.Load();
		ContractSnapshot snapshot = baseline.GetSnapshot(MainOpenApiSnapshotId);
		return Parse(snapshot.ReadUtf8Text());
	}

	private static string ExtractType(JsonElement propertyDefinition)
	{
		if (propertyDefinition.TryGetProperty("type", out JsonElement type))
		{
			return type.GetString() ?? "unknown";
		}

		if (propertyDefinition.TryGetProperty("$ref", out JsonElement reference))
		{
			return ExtractReference(reference.GetString());
		}

		if (propertyDefinition.TryGetProperty("anyOf", out JsonElement anyOf) &&
			anyOf.ValueKind == JsonValueKind.Array)
		{
			List<string> parts = [];
			foreach (JsonElement item in anyOf.EnumerateArray())
			{
				if (item.TryGetProperty("type", out JsonElement itemType))
				{
					parts.Add(itemType.GetString() ?? "null");
				}
				else if (item.TryGetProperty("$ref", out JsonElement itemReference))
				{
					parts.Add(ExtractReference(itemReference.GetString()));
				}
				else
				{
					parts.Add("unknown");
				}
			}

			parts.Sort(StringComparer.Ordinal);
			return string.Join("|", parts);
		}

		return "unknown";
	}

	private static string ExtractReference(string? reference) =>
		reference?.Split('/').Last() ?? "unknown";
}
