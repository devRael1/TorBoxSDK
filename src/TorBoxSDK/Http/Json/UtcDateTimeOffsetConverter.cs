using System.Text.Json;
using System.Text.Json.Serialization;

namespace TorBoxSDK.Http.Json;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="DateTimeOffset"/> that
/// normalises all deserialised values to UTC via <see cref="DateTimeOffset.ToUniversalTime"/>.
/// </summary>
internal sealed class UtcDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
	/// <inheritdoc />
	public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		DateTimeOffset value = reader.GetDateTimeOffset();
		return value.ToUniversalTime();
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToUniversalTime());
	}
}
