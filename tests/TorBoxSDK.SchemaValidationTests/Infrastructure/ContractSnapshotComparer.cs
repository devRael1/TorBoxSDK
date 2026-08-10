using System.Text.Json;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

internal static class ContractSnapshotComparer
{
	private static readonly HashSet<string> _httpMethods = new(StringComparer.Ordinal)
	{
		"delete",
		"get",
		"head",
		"options",
		"patch",
		"post",
		"put",
		"trace"
	};

	internal static ContractComparisonResult Compare(ContractSnapshot baseline, string candidateJson)
	{
		ArgumentNullException.ThrowIfNull(baseline);
		ArgumentNullException.ThrowIfNull(candidateJson);

		return baseline.Descriptor.Format switch
		{
			"openapi" => CompareOpenApi(baseline.ReadUtf8Text(), candidateJson),
			"postman-public-api-collection" => ComparePostmanCollection(baseline.ReadUtf8Text(), candidateJson),
			_ => throw new NotSupportedException(
				$"Contract format '{baseline.Descriptor.Format}' is not supported by the offline comparer.")
		};
	}

	internal static ContractComparisonResult CompareOpenApi(string baselineJson, string candidateJson)
	{
		ArgumentNullException.ThrowIfNull(baselineJson);
		ArgumentNullException.ThrowIfNull(candidateJson);

		using JsonDocument baselineDocument = JsonDocument.Parse(baselineJson);
		using JsonDocument candidateDocument = JsonDocument.Parse(candidateJson);
		return CompareFacts(
			ExtractOpenApiFacts(baselineDocument.RootElement),
			ExtractOpenApiFacts(candidateDocument.RootElement));
	}

	internal static ContractComparisonResult ComparePostmanCollection(string baselineJson, string candidateJson)
	{
		ArgumentNullException.ThrowIfNull(baselineJson);
		ArgumentNullException.ThrowIfNull(candidateJson);

		using JsonDocument baselineDocument = JsonDocument.Parse(baselineJson);
		using JsonDocument candidateDocument = JsonDocument.Parse(candidateJson);
		return CompareFacts(
			ExtractPostmanFacts(baselineDocument.RootElement),
			ExtractPostmanFacts(candidateDocument.RootElement));
	}

	private static ContractComparisonResult CompareFacts(
		IEnumerable<ContractFact> baselineFacts,
		IEnumerable<ContractFact> candidateFacts)
	{
		Dictionary<string, ContractFact> baselineByKey = ToFactDictionary(baselineFacts);
		Dictionary<string, ContractFact> candidateByKey = ToFactDictionary(candidateFacts);
		HashSet<string> factKeys = new(baselineByKey.Keys, StringComparer.Ordinal);
		factKeys.UnionWith(candidateByKey.Keys);

		List<ContractDifference> differences = [];
		foreach (string factKey in factKeys.OrderBy(key => key, StringComparer.Ordinal))
		{
			bool hasBaseline = baselineByKey.TryGetValue(factKey, out ContractFact? baseline);
			bool hasCandidate = candidateByKey.TryGetValue(factKey, out ContractFact? candidate);
			if (!hasBaseline)
			{
				ContractFact candidateFact = candidate
					?? throw new InvalidDataException($"Candidate fact '{factKey}' was unexpectedly null.");
				differences.Add(new ContractDifference(
					ContractDifferenceKind.Added,
					candidateFact.Category,
					candidateFact.Subject,
					null,
					candidateFact.Value));
				continue;
			}

			if (!hasCandidate)
			{
				ContractFact baselineFact = baseline
					?? throw new InvalidDataException($"Baseline fact '{factKey}' was unexpectedly null.");
				differences.Add(new ContractDifference(
					ContractDifferenceKind.Removed,
					baselineFact.Category,
					baselineFact.Subject,
					baselineFact.Value,
					null));
				continue;
			}

			ContractFact matchingBaseline = baseline
				?? throw new InvalidDataException($"Baseline fact '{factKey}' was unexpectedly null.");
			ContractFact matchingCandidate = candidate
				?? throw new InvalidDataException($"Candidate fact '{factKey}' was unexpectedly null.");
			if (!string.Equals(matchingBaseline.Value, matchingCandidate.Value, StringComparison.Ordinal))
			{
				differences.Add(new ContractDifference(
					ContractDifferenceKind.Modified,
					matchingBaseline.Category,
					matchingBaseline.Subject,
					matchingBaseline.Value,
					matchingCandidate.Value));
			}
		}

		return new ContractComparisonResult(
			differences
				.OrderBy(difference => difference.Category, StringComparer.Ordinal)
				.ThenBy(difference => difference.Subject, StringComparer.Ordinal)
				.ThenBy(difference => difference.Kind)
				.ToArray());
	}

	private static Dictionary<string, ContractFact> ToFactDictionary(IEnumerable<ContractFact> facts)
	{
		Dictionary<string, ContractFact> result = new(StringComparer.Ordinal);
		foreach (ContractFact fact in facts)
		{
			result[GetFactKey(fact)] = fact;
		}

		return result;
	}

	private static IEnumerable<ContractFact> ExtractOpenApiFacts(JsonElement root)
	{
		Dictionary<string, ContractFact> facts = new(StringComparer.Ordinal);
		if (root.TryGetProperty("paths", out JsonElement paths) && paths.ValueKind == JsonValueKind.Object)
		{
			foreach (JsonProperty path in paths.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
			{
				AddFact(facts, "route", path.Name, "present");
				if (path.Value.ValueKind != JsonValueKind.Object)
				{
					continue;
				}

				foreach (JsonProperty operation in path.Value.EnumerateObject()
					.Where(property => _httpMethods.Contains(property.Name))
					.OrderBy(property => property.Name, StringComparer.Ordinal))
				{
					if (operation.Value.ValueKind != JsonValueKind.Object)
					{
						continue;
					}

					string operationKey = $"{operation.Name.ToUpperInvariant()} {path.Name}";
					AddFact(
						facts,
						"operation",
						operationKey,
						ReadOptionalString(operation.Value, "operationId") ?? string.Empty);
					AddParameterFacts(facts, operationKey, path.Value);
					AddParameterFacts(facts, operationKey, operation.Value);
					AddRequestContentFacts(facts, operationKey, operation.Value);
					AddResponseContentFacts(facts, operationKey, operation.Value);
				}
			}
		}

		AddSchemaFacts(facts, root);
		return facts.Values;
	}

	private static void AddParameterFacts(
		Dictionary<string, ContractFact> facts,
		string operationKey,
		JsonElement container)
	{
		if (!container.TryGetProperty("parameters", out JsonElement parameters) ||
			parameters.ValueKind != JsonValueKind.Array)
		{
			return;
		}

		foreach (JsonElement parameter in parameters.EnumerateArray())
		{
			if (parameter.ValueKind != JsonValueKind.Object)
			{
				continue;
			}

			string? name = ReadOptionalString(parameter, "name");
			string? location = ReadOptionalString(parameter, "in");
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(location))
			{
				string reference = ReadOptionalString(parameter, "$ref") ?? CanonicalizeValue(parameter);
				AddFact(facts, "parameter", $"{operationKey}:$ref:{reference}", reference);
				continue;
			}

			string required = parameter.TryGetProperty("required", out JsonElement requiredElement) &&
				requiredElement.ValueKind == JsonValueKind.True
				? "true"
				: "false";
			string schema = parameter.TryGetProperty("schema", out JsonElement schemaElement)
				? BuildSchemaSignature(schemaElement)
				: string.Empty;
			string content = parameter.TryGetProperty("content", out JsonElement contentElement)
				? BuildContentSignature(contentElement)
				: string.Empty;
			string value = $"required={required};schema={schema};content={content}";
			AddFact(facts, "parameter", $"{operationKey}:{location}:{name}", value);
		}
	}

	private static void AddRequestContentFacts(
		Dictionary<string, ContractFact> facts,
		string operationKey,
		JsonElement operation)
	{
		if (!operation.TryGetProperty("requestBody", out JsonElement requestBody) ||
			requestBody.ValueKind != JsonValueKind.Object ||
			!requestBody.TryGetProperty("content", out JsonElement content))
		{
			return;
		}

		AddContentFacts(facts, "request-content", operationKey, content);
	}

	private static void AddResponseContentFacts(
		Dictionary<string, ContractFact> facts,
		string operationKey,
		JsonElement operation)
	{
		if (!operation.TryGetProperty("responses", out JsonElement responses) ||
			responses.ValueKind != JsonValueKind.Object)
		{
			return;
		}

		foreach (JsonProperty response in responses.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
		{
			string responseScope = $"{operationKey}:{response.Name}";
			AddFact(facts, "response", responseScope, BuildResponseSignature(response.Value));

			if (response.Value.ValueKind != JsonValueKind.Object ||
				!response.Value.TryGetProperty("content", out JsonElement content))
			{
				continue;
			}

			AddContentFacts(facts, "response-content", responseScope, content);
		}
	}

	private static string BuildResponseSignature(JsonElement response)
	{
		if (response.ValueKind != JsonValueKind.Object)
		{
			return CanonicalizeValue(response);
		}

		return $"{{{string.Join(",", response.EnumerateObject()
			.Where(property => !string.Equals(property.Name, "content", StringComparison.Ordinal))
			.OrderBy(property => property.Name, StringComparer.Ordinal)
			.Select(property => $"{JsonSerializer.Serialize(property.Name)}:{CanonicalizeValue(property.Value)}"))}}}";
	}

	private static void AddContentFacts(
		Dictionary<string, ContractFact> facts,
		string category,
		string scope,
		JsonElement content)
	{
		if (content.ValueKind != JsonValueKind.Object)
		{
			return;
		}

		foreach (JsonProperty contentType in content.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
		{
			string schema = contentType.Value.ValueKind == JsonValueKind.Object &&
				contentType.Value.TryGetProperty("schema", out JsonElement schemaElement)
				? BuildSchemaSignature(schemaElement)
				: string.Empty;
			AddFact(facts, category, $"{scope}:{contentType.Name}", schema);
		}
	}

	private static void AddSchemaFacts(Dictionary<string, ContractFact> facts, JsonElement root)
	{
		if (!root.TryGetProperty("components", out JsonElement components) ||
			components.ValueKind != JsonValueKind.Object ||
			!components.TryGetProperty("schemas", out JsonElement schemas) ||
			schemas.ValueKind != JsonValueKind.Object)
		{
			return;
		}

		foreach (JsonProperty schema in schemas.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
		{
			AddFact(facts, "schema", schema.Name, BuildSchemaSignature(schema.Value));
			if (schema.Value.ValueKind != JsonValueKind.Object ||
				!schema.Value.TryGetProperty("properties", out JsonElement properties) ||
				properties.ValueKind != JsonValueKind.Object)
			{
				continue;
			}

			HashSet<string> requiredFields = GetRequiredFields(schema.Value);
			foreach (JsonProperty field in properties.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
			{
				string required = requiredFields.Contains(field.Name) ? "true" : "false";
				AddFact(
					facts,
					"field",
					$"{schema.Name}.{field.Name}",
					$"required={required};schema={BuildSchemaSignature(field.Value)}");
			}
		}
	}

	private static HashSet<string> GetRequiredFields(JsonElement schema)
	{
		HashSet<string> fields = new(StringComparer.Ordinal);
		if (!schema.TryGetProperty("required", out JsonElement required) ||
			required.ValueKind != JsonValueKind.Array)
		{
			return fields;
		}

		foreach (JsonElement field in required.EnumerateArray())
		{
			if (field.ValueKind == JsonValueKind.String && field.GetString() is string fieldName)
			{
				fields.Add(fieldName);
			}
		}

		return fields;
	}

	private static IEnumerable<ContractFact> ExtractPostmanFacts(JsonElement root)
	{
		Dictionary<string, ContractFact> facts = new(StringComparer.Ordinal);
		if (root.TryGetProperty("data", out JsonElement data) &&
			data.ValueKind == JsonValueKind.Object &&
			data.TryGetProperty("requests", out JsonElement requests) &&
			requests.ValueKind == JsonValueKind.Array)
		{
			foreach (JsonElement request in requests.EnumerateArray())
			{
				AddPostmanRequestFacts(facts, request, false);
			}

			return facts.Values;
		}

		if (root.TryGetProperty("item", out JsonElement items) && items.ValueKind == JsonValueKind.Array)
		{
			AddPostmanV21ItemFacts(facts, items);
		}

		return facts.Values;
	}

	private static void AddPostmanV21ItemFacts(Dictionary<string, ContractFact> facts, JsonElement items)
	{
		foreach (JsonElement item in items.EnumerateArray())
		{
			if (item.ValueKind != JsonValueKind.Object)
			{
				continue;
			}

			if (item.TryGetProperty("request", out JsonElement request) && request.ValueKind == JsonValueKind.Object)
			{
				AddPostmanRequestFacts(facts, request, true);
			}

			if (item.TryGetProperty("item", out JsonElement childItems) && childItems.ValueKind == JsonValueKind.Array)
			{
				AddPostmanV21ItemFacts(facts, childItems);
			}
		}
	}

	private static void AddPostmanRequestFacts(
		Dictionary<string, ContractFact> facts,
		JsonElement request,
		bool isV21Collection)
	{
		if (request.ValueKind != JsonValueKind.Object)
		{
			return;
		}

		string? method = ReadOptionalString(request, "method");
		string? url = ReadPostmanUrl(request);
		if (string.IsNullOrWhiteSpace(method) || string.IsNullOrWhiteSpace(url))
		{
			return;
		}

		string operationKey = $"{method.ToUpperInvariant()} {url}";
		AddFact(facts, "route", url, "present");
		AddFact(facts, "operation", operationKey, "present");
		AddPostmanParameterFacts(facts, operationKey, request, isV21Collection);
		AddPostmanContentFacts(facts, operationKey, request, isV21Collection);
	}

	private static string? ReadPostmanUrl(JsonElement request)
	{
		if (!request.TryGetProperty("url", out JsonElement url))
		{
			return null;
		}

		if (url.ValueKind == JsonValueKind.String)
		{
			return url.GetString();
		}

		return url.ValueKind == JsonValueKind.Object
			? ReadOptionalString(url, "raw")
			: null;
	}

	private static void AddPostmanParameterFacts(
		Dictionary<string, ContractFact> facts,
		string operationKey,
		JsonElement request,
		bool isV21Collection)
	{
		string propertyName = isV21Collection ? "url" : "queryParams";
		JsonElement parameters;
		if (isV21Collection &&
			request.TryGetProperty(propertyName, out JsonElement url) &&
			url.ValueKind == JsonValueKind.Object &&
			url.TryGetProperty("query", out JsonElement query))
		{
			parameters = query;
		}
		else if (!isV21Collection && request.TryGetProperty(propertyName, out JsonElement queryParams))
		{
			parameters = queryParams;
		}
		else
		{
			return;
		}

		if (parameters.ValueKind != JsonValueKind.Array)
		{
			return;
		}

		foreach (JsonElement parameter in parameters.EnumerateArray())
		{
			string? key = ReadOptionalString(parameter, "key");
			if (string.IsNullOrWhiteSpace(key))
			{
				continue;
			}

			string disabled = parameter.TryGetProperty("disabled", out JsonElement disabledElement) &&
				disabledElement.ValueKind == JsonValueKind.True
				? "true"
				: "false";
			AddFact(facts, "parameter", $"{operationKey}:query:{key}", $"disabled={disabled}");
		}
	}

	private static void AddPostmanContentFacts(
		Dictionary<string, ContractFact> facts,
		string operationKey,
		JsonElement request,
		bool isV21Collection)
	{
		string headersProperty = isV21Collection ? "header" : "headerData";
		if (!request.TryGetProperty(headersProperty, out JsonElement headers) ||
			headers.ValueKind != JsonValueKind.Array)
		{
			request.TryGetProperty("headers", out headers);
		}

		if (headers.ValueKind == JsonValueKind.Array)
		{
			foreach (JsonElement header in headers.EnumerateArray())
			{
				string? key = ReadOptionalString(header, "key");
				string? value = ReadOptionalString(header, "value");
				bool isDisabled = header.TryGetProperty("disabled", out JsonElement disabledElement) &&
					disabledElement.ValueKind == JsonValueKind.True;
				if (isDisabled || !string.Equals(key, "Content-Type", StringComparison.OrdinalIgnoreCase) ||
					string.IsNullOrWhiteSpace(value))
				{
					continue;
				}

				AddFact(facts, "request-content", $"{operationKey}:{value.ToLowerInvariant()}", string.Empty);
			}
		}

		string? dataMode = isV21Collection
			? ReadPostmanV21BodyMode(request)
			: ReadOptionalString(request, "dataMode");
		if (!string.IsNullOrWhiteSpace(dataMode))
		{
			AddFact(facts, "postman-body-mode", operationKey, dataMode);
		}
	}

	private static string? ReadPostmanV21BodyMode(JsonElement request) =>
		request.TryGetProperty("body", out JsonElement body) && body.ValueKind == JsonValueKind.Object
			? ReadOptionalString(body, "mode")
			: null;

	private static string BuildContentSignature(JsonElement content)
	{
		if (content.ValueKind != JsonValueKind.Object)
		{
			return string.Empty;
		}

		return string.Join(
			"|",
			content.EnumerateObject()
				.OrderBy(property => property.Name, StringComparer.Ordinal)
				.Select(property =>
				{
					string schema = property.Value.ValueKind == JsonValueKind.Object &&
						property.Value.TryGetProperty("schema", out JsonElement schemaElement)
						? BuildSchemaSignature(schemaElement)
						: string.Empty;
					return $"{property.Name}={schema}";
				}));
	}

	private static string BuildSchemaSignature(JsonElement schema)
	{
		if (schema.ValueKind != JsonValueKind.Object)
		{
			return CanonicalizeValue(schema);
		}

		string[] semanticPropertyNames =
		[
			"$ref",
			"additionalProperties",
			"allOf",
			"anyOf",
			"const",
			"default",
			"enum",
			"format",
			"items",
			"maxItems",
			"maxLength",
			"maximum",
			"minItems",
			"minLength",
			"minimum",
			"multipleOf",
			"nullable",
			"oneOf",
			"pattern",
			"type",
			"uniqueItems"
		];

		List<string> parts = [];
		foreach (string propertyName in semanticPropertyNames)
		{
			if (!schema.TryGetProperty(propertyName, out JsonElement property))
			{
				continue;
			}

			string value = propertyName is "allOf" or "anyOf" or "enum" or "oneOf"
				? CanonicalizeSet(property)
				: CanonicalizeValue(property);
			parts.Add($"{propertyName}={value}");
		}

		return string.Join(";", parts);
	}

	private static string CanonicalizeSet(JsonElement value)
	{
		if (value.ValueKind != JsonValueKind.Array)
		{
			return CanonicalizeValue(value);
		}

		return $"[{string.Join(",", value.EnumerateArray().Select(CanonicalizeValue).OrderBy(item => item, StringComparer.Ordinal))}]";
	}

	private static string CanonicalizeValue(JsonElement value) => value.ValueKind switch
	{
		JsonValueKind.Array =>
			$"[{string.Join(",", value.EnumerateArray().Select(CanonicalizeValue))}]",
		JsonValueKind.False => "false",
		JsonValueKind.Null => "null",
		JsonValueKind.Number => value.GetRawText(),
		JsonValueKind.Object =>
			$"{{{string.Join(",", value.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal)
				.Select(property => $"{JsonSerializer.Serialize(property.Name)}:{CanonicalizeValue(property.Value)}"))}}}",
		JsonValueKind.String => JsonSerializer.Serialize(value.GetString()),
		JsonValueKind.True => "true",
		_ => string.Empty
	};

	private static void AddFact(
		Dictionary<string, ContractFact> facts,
		string category,
		string subject,
		string value)
	{
		ContractFact fact = new(category, subject, value);
		facts[GetFactKey(fact)] = fact;
	}

	private static string GetFactKey(ContractFact fact) => $"{fact.Category}\u001f{fact.Subject}";

	private static string? ReadOptionalString(JsonElement element, string propertyName) =>
		element.ValueKind == JsonValueKind.Object &&
		element.TryGetProperty(propertyName, out JsonElement property) &&
		property.ValueKind == JsonValueKind.String
			? property.GetString()
			: null;

	private sealed record ContractFact(string Category, string Subject, string Value);
}
