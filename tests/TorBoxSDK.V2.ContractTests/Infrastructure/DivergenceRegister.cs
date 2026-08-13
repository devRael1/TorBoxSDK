using System.Globalization;
using System.Text.Json;

namespace TorBoxSDK.V2.ContractTests.Infrastructure;

internal sealed class DivergenceRegister
{
    private DivergenceRegister(IReadOnlySet<string> ids)
    {
        Ids = ids;
    }

    internal IReadOnlySet<string> Ids { get; }

    internal static DivergenceRegister Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The contract divergence register is missing.", path);
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllBytes(path));
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidDataException("The contract divergence register must be a JSON array.");
            }

            HashSet<string> ids = new(StringComparer.Ordinal);
            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidDataException("Every divergence record must be a JSON object.");
                }

                string id = GetRequiredString(element, "id");
                if (!id.StartsWith("DIV-", StringComparison.Ordinal) || !ids.Add(id))
                {
                    throw new InvalidDataException($"The contract divergence register contains an invalid or duplicate id '{id}'.");
                }

                ValidateOperationKey(GetRequiredString(element, "operationKey"), id);
                _ = GetRequiredString(element, "source");
                string observedAtText = GetRequiredString(element, "observedAtUtc");
                if (!observedAtText.EndsWith("Z", StringComparison.Ordinal) ||
                    !DateTimeOffset.TryParse(observedAtText, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset observedAtUtc) ||
                    observedAtUtc.Offset != TimeSpan.Zero)
                {
                    throw new InvalidDataException($"Divergence '{id}' observedAtUtc must be a UTC timestamp.");
                }

                _ = GetRequiredString(element, "contractBehavior");
                _ = GetRequiredString(element, "v2Behavior");
                _ = GetRequiredString(element, "evidence");
            }

            return new DivergenceRegister(ids);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("The contract divergence register contains malformed JSON.", exception);
        }
    }

    private static string GetRequiredString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property) || property.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new InvalidDataException($"Divergence property '{propertyName}' must be a non-empty string.");
        }

        return property.GetString() ?? throw new InvalidDataException($"Divergence property '{propertyName}' must be a string.");
    }

    private static void ValidateOperationKey(string operationKey, string id)
    {
        int separator = operationKey.IndexOf(" ", StringComparison.Ordinal);
        if (separator <= 0 || separator == operationKey.Length - 1 || !operationKey[(separator + 1)..].StartsWith("/", StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Divergence '{id}' operationKey must be formatted as METHOD path.");
        }
    }
}
