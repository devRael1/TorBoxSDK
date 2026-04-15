using System.Text.Json;
using System.Text.Json.Serialization;

namespace TorBoxSDK.Http.Json;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="Nullable{DateTimeOffset}"/> that
/// normalises all deserialised values to UTC via <see cref="DateTimeOffset.ToUniversalTime"/>.
/// </summary>
internal sealed class NullableUtcDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    /// <inheritdoc />
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        DateTimeOffset value = reader.GetDateTimeOffset();
        return value.ToUniversalTime();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Value.ToUniversalTime());
        }
    }
}
