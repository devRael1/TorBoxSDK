using System.Text.Json;

namespace TorBoxSDK.V2.ContractTests.Infrastructure;

internal sealed class CoverageManifest
{
    private static readonly HashSet<string> SourceIds = new(StringComparer.Ordinal)
    {
        "main", "relay", "search"
    };

    private static readonly HashSet<string> HttpMethods = new(StringComparer.Ordinal)
    {
        "GET", "PUT", "POST", "DELETE", "OPTIONS", "HEAD", "PATCH", "TRACE"
    };

    private static readonly HashSet<string> MainResources = new(StringComparer.Ordinal)
    {
        "General", "Torrents", "Usenet", "WebDownloads", "User", "Notifications", "Rss", "Stream", "Integrations", "Vendors", "Queued"
    };

    private CoverageManifest(IReadOnlyList<CoverageRecord> records)
    {
        Records = records;
    }

    internal IReadOnlyList<CoverageRecord> Records { get; }

    internal static CoverageManifest Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The contract coverage inventory is missing.", path);
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllBytes(path));
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidDataException("The contract coverage inventory must be a JSON array.");
            }

            List<CoverageRecord> records = [];
            HashSet<string> identities = new(StringComparer.Ordinal);
            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                CoverageRecord record = ParseRecord(element);
                if (!identities.Add(record.Identity))
                {
                    throw new InvalidDataException($"The contract coverage inventory contains duplicate identity '{record.Identity}'.");
                }

                records.Add(record);
            }

            return new CoverageManifest(records);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("The contract coverage inventory contains malformed JSON.", exception);
        }
    }

    internal static void EnsureOperationSetMatches(IReadOnlyList<CoverageRecord> records, ContractBaseline baseline)
    {
        HashSet<string> coverageIdentities = new(StringComparer.Ordinal);
        foreach (CoverageRecord record in records)
        {
            if (!coverageIdentities.Add(record.Identity))
            {
                throw new InvalidDataException($"The contract coverage inventory contains duplicate identity '{record.Identity}'.");
            }
        }

        if (!coverageIdentities.SetEquals(baseline.OperationIdentities))
        {
            IEnumerable<string> missing = baseline.OperationIdentities.Except(coverageIdentities).OrderBy(static identity => identity);
            IEnumerable<string> stale = coverageIdentities.Except(baseline.OperationIdentities).OrderBy(static identity => identity);
            throw new InvalidDataException($"Coverage operations do not match the contract source union. Missing: {string.Join(", ", missing)}. Stale: {string.Join(", ", stale)}.");
        }
    }

    internal static void EnsureRecordShapesMatchSources(IReadOnlyList<CoverageRecord> records, ContractBaseline baseline)
    {
        foreach (CoverageRecord record in records)
        {
            if (!baseline.OperationIdentities.Contains(record.Identity))
            {
                throw new InvalidDataException($"Coverage operation '{record.Identity}' is not present in the contract source registry.");
            }

            ValidateRecordShape(record, baseline.RequestBodyOperationIdentities.Contains(record.Identity));
        }
    }

    internal void EnsureDivergenceIdsAreRegistered(DivergenceRegister divergences)
    {
        foreach (CoverageRecord record in Records)
        {
            foreach (string divergenceId in record.DivergenceIds)
            {
                if (!divergences.Ids.Contains(divergenceId))
                {
                    throw new InvalidDataException($"Coverage operation '{record.Identity}' references unknown divergence ID '{divergenceId}'.");
                }
            }
        }
    }

    private static CoverageRecord ParseRecord(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("Every contract coverage record must be a JSON object.");
        }

        string sourceId = GetRequiredString(element, "sourceId");
        if (!SourceIds.Contains(sourceId))
        {
            throw new InvalidDataException($"Coverage source ID '{sourceId}' is not approved.");
        }

        string operationKey = GetRequiredString(element, "operationKey");
        ValidateOperationKey(operationKey);
        string familyValue = GetRequiredString(element, "family");
        CoverageFamily family = familyValue switch
        {
            "Main" => CoverageFamily.Main,
            "Search" => CoverageFamily.Search,
            "Relay" => CoverageFamily.Relay,
            _ => throw new InvalidDataException($"Coverage operation '{sourceId}:{operationKey}' has an unapproved family '{familyValue}'.")
        };
        string resource = GetRequiredString(element, "resource");
        string? publicInterface = GetNullableString(element, "publicInterface");
        string? publicMethod = GetNullableString(element, "publicMethod");
        IReadOnlyList<string> parameterTypes = GetRequiredStringArray(element, "parameterTypes");
        string? requestType = GetNullableString(element, "requestType");
        string? resultType = GetNullableString(element, "resultType");
        CoverageResponseMode responseMode = GetResponseMode(element, sourceId, operationKey);
        CoverageImplementationState implementationState = GetImplementationState(element, sourceId, operationKey);
        IReadOnlyList<string> divergenceIds = GetRequiredStringArray(element, "divergenceIds");

        CoverageRecord record = new(
            sourceId,
            operationKey,
            family,
            resource,
            publicInterface,
            publicMethod,
            parameterTypes,
            requestType,
            resultType,
            responseMode,
            implementationState,
            divergenceIds);
        ValidateApprovedFamilyResourcePair(record);
        ValidateState(record);
        return record;
    }

    private static void ValidateRecordShape(CoverageRecord record, bool sourceHasRequestBody)
    {
        ValidateOperationKey(record.OperationKey);
        ValidateApprovedFamilyResourcePair(record);
        ValidateState(record);

        if (!sourceHasRequestBody && record.RequestType is not null)
        {
            throw new InvalidDataException($"Coverage operation '{record.Identity}' does not have a contract request body and must not declare requestType.");
        }

        if (record.ImplementationState != CoverageImplementationState.Implemented)
        {
            return;
        }

        if (sourceHasRequestBody && string.IsNullOrWhiteSpace(record.RequestType))
        {
            throw new InvalidDataException($"Implemented coverage operation '{record.Identity}' has a request body and must declare requestType.");
        }

        if (record.RequestType is not null && !record.ParameterTypes.Contains(record.RequestType, StringComparer.Ordinal))
        {
            throw new InvalidDataException($"Implemented coverage operation '{record.Identity}' must include requestType in its ordered parameterTypes.");
        }
    }

    private static void ValidateState(CoverageRecord record)
    {
        if (record.ImplementationState == CoverageImplementationState.Planned)
        {
            if (record.PublicInterface is not null || record.PublicMethod is not null || record.ParameterTypes.Count != 0 ||
                record.RequestType is not null || record.ResultType is not null)
            {
                throw new InvalidDataException($"Planned coverage operation '{record.Identity}' must not declare a public mapping.");
            }

            return;
        }

        bool hasExactTaskResultType = record.ResultType is not null &&
            (record.ResultType.StartsWith("System.Threading.Tasks.Task, ", StringComparison.Ordinal) ||
             record.ResultType.StartsWith("System.Threading.Tasks.Task`1[[", StringComparison.Ordinal));
        if (string.IsNullOrWhiteSpace(record.PublicInterface) || !record.PublicInterface.Contains('.', StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(record.PublicMethod) || !hasExactTaskResultType)
        {
            throw new InvalidDataException($"Implemented coverage operation '{record.Identity}' must declare a fully qualified public interface, public method, ordered parameter types, and exact Task result type.");
        }

        if (record.ResponseMode == CoverageResponseMode.RequiresValidation)
        {
            throw new InvalidDataException($"Implemented coverage operation '{record.Identity}' must resolve responseMode to json, stream, or redirect.");
        }
    }

    private static void ValidateApprovedFamilyResourcePair(CoverageRecord record)
    {
        bool approved = record.SourceId switch
        {
            "main" => record.Family == CoverageFamily.Main && MainResources.Contains(record.Resource),
            "search" => record.Family == CoverageFamily.Search && string.Equals(record.Resource, "Search", StringComparison.Ordinal),
            "relay" => record.Family == CoverageFamily.Relay && string.Equals(record.Resource, "Relay", StringComparison.Ordinal),
            _ => false
        };

        if (!approved)
        {
            throw new InvalidDataException($"Coverage operation '{record.Identity}' has an unapproved source/family/resource mapping '{record.SourceId}/{record.Family}/{record.Resource}'.");
        }
    }

    private static void ValidateOperationKey(string operationKey)
    {
        int separatorIndex = operationKey.IndexOf(" ", StringComparison.Ordinal);
        if (separatorIndex <= 0 || separatorIndex == operationKey.Length - 1 || operationKey.IndexOf(" ", separatorIndex + 1, StringComparison.Ordinal) >= 0)
        {
            throw new InvalidDataException($"Coverage operation key '{operationKey}' must be formatted as METHOD path.");
        }

        string method = operationKey[..separatorIndex];
        string path = operationKey[(separatorIndex + 1)..];
        if (!HttpMethods.Contains(method) || !path.StartsWith("/", StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Coverage operation key '{operationKey}' must be formatted as METHOD path.");
        }
    }

    private static string GetRequiredString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property) || property.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new InvalidDataException($"Coverage record property '{propertyName}' must be a non-empty string.");
        }

        return property.GetString() ?? throw new InvalidDataException($"Coverage record property '{propertyName}' must be a string.");
    }

    private static string? GetNullableString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property) || property.ValueKind is not (JsonValueKind.String or JsonValueKind.Null))
        {
            throw new InvalidDataException($"Coverage record property '{propertyName}' must be a string or null.");
        }

        if (property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        string? value = property.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException($"Coverage record property '{propertyName}' must be null or a non-empty string.");
        }

        return value;
    }

    private static IReadOnlyList<string> GetRequiredStringArray(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property) || property.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException($"Coverage record property '{propertyName}' must be an array.");
        }

        List<string> values = [];
        foreach (JsonElement value in property.EnumerateArray())
        {
            if (value.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(value.GetString()))
            {
                throw new InvalidDataException($"Coverage record property '{propertyName}' must contain only non-empty strings.");
            }

            values.Add(value.GetString() ?? throw new InvalidDataException($"Coverage record property '{propertyName}' must contain strings."));
        }

        return values;
    }

    private static CoverageResponseMode GetResponseMode(JsonElement element, string sourceId, string operationKey)
    {
        string responseMode = GetRequiredString(element, "responseMode");
        return responseMode switch
        {
            "json" => CoverageResponseMode.Json,
            "stream" => CoverageResponseMode.Stream,
            "redirect" => CoverageResponseMode.Redirect,
            "requires-validation" => CoverageResponseMode.RequiresValidation,
            _ => throw new InvalidDataException($"Coverage operation '{sourceId}:{operationKey}' has an unapproved responseMode '{responseMode}'.")
        };
    }

    private static CoverageImplementationState GetImplementationState(JsonElement element, string sourceId, string operationKey)
    {
        string implementationState = GetRequiredString(element, "implementationState");
        return implementationState switch
        {
            "Planned" => CoverageImplementationState.Planned,
            "Implemented" => CoverageImplementationState.Implemented,
            _ => throw new InvalidDataException($"Coverage operation '{sourceId}:{operationKey}' has an unapproved implementationState '{implementationState}'.")
        };
    }
}

internal sealed record CoverageRecord(
    string SourceId,
    string OperationKey,
    CoverageFamily Family,
    string Resource,
    string? PublicInterface,
    string? PublicMethod,
    IReadOnlyList<string> ParameterTypes,
    string? RequestType,
    string? ResultType,
    CoverageResponseMode ResponseMode,
    CoverageImplementationState ImplementationState,
    IReadOnlyList<string> DivergenceIds)
{
    internal string Identity => $"{SourceId}:{OperationKey}";
}

internal enum CoverageFamily
{
    Main,
    Search,
    Relay
}

internal enum CoverageResponseMode
{
    Json,
    Stream,
    Redirect,
    RequiresValidation
}

internal enum CoverageImplementationState
{
    Planned,
    Implemented
}
