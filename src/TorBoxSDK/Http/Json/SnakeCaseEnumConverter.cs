using System.Text.Json;
using System.Text.Json.Serialization;

namespace TorBoxSDK.Http.Json;

/// <summary>
/// A <see cref="JsonConverterFactory"/> that serializes and deserializes enum values
/// as snake_case strings, working correctly on all supported .NET versions.
/// </summary>
/// <remarks>
/// On .NET 6, <see cref="JsonStringEnumConverter"/> with a naming policy only applies
/// the policy during serialization, not deserialization. This converter builds an explicit
/// lookup dictionary so that deserialization works correctly on all targets.
/// </remarks>
internal sealed class SnakeCaseEnumConverterFactory : JsonConverterFactory
{
    private readonly JsonNamingPolicy _namingPolicy;

    internal SnakeCaseEnumConverterFactory(JsonNamingPolicy namingPolicy)
    {
        _namingPolicy = namingPolicy;
    }

    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsEnum;

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type converterType = typeof(SnakeCaseEnumConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter?)Activator.CreateInstance(converterType, _namingPolicy);
    }
}

internal sealed class SnakeCaseEnumConverter<T> : JsonConverter<T>
    where T : struct, Enum
{
    private readonly JsonNamingPolicy _namingPolicy;

    // Maps converted snake_case name → enum value (for reading)
    private readonly Dictionary<string, T> _readMap;

    // Maps enum value → converted snake_case name (for writing)
    private readonly Dictionary<T, string> _writeMap;

    public SnakeCaseEnumConverter(JsonNamingPolicy namingPolicy)
    {
        _namingPolicy = namingPolicy;

        T[] values = Enum.GetValues<T>();
        string[] names = Enum.GetNames<T>();

        _readMap = [];
        _writeMap = [];

        for (int i = 0; i < names.Length; i++)
        {
            string converted = namingPolicy.ConvertName(names[i]);
            _readMap[converted] = values[i];

            // Also allow original name for robustness
            _readMap.TryAdd(names[i], values[i]);
            _writeMap[values[i]] = converted;
        }
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? value = reader.GetString();
            if (value is not null && _readMap.TryGetValue(value, out T result))
            {
                return result;
            }
        }

        return reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int intValue) ? (T)(object)intValue : default;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (_writeMap.TryGetValue(value, out string? name))
        {
            writer.WriteStringValue(name);
        }
        else
        {
            writer.WriteStringValue(_namingPolicy.ConvertName(value.ToString()));
        }
    }
}
