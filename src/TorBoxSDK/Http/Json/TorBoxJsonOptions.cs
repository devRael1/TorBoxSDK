using System.Text.Json;
using System.Text.Json.Serialization;

namespace TorBoxSDK.Http.Json;

/// <summary>
/// Provides the shared <see cref="JsonSerializerOptions"/> instance used
/// across the TorBox SDK for consistent JSON serialization.
/// </summary>
internal static class TorBoxJsonOptions
{
    /// <summary>
    /// Gets the default <see cref="JsonSerializerOptions"/> configured for the TorBox API.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Property names use <c>snake_case</c>.</description></item>
    ///   <item><description>Null properties are omitted when writing.</description></item>
    ///   <item><description>Enums are serialized as their string names.</description></item>
    /// </list>
    /// </remarks>
    public static JsonSerializerOptions Default { get; } = CreateDefault();

    private static JsonSerializerOptions CreateDefault()
    {
#if NET8_0_OR_GREATER
        JsonNamingPolicy namingPolicy = JsonNamingPolicy.SnakeCaseLower;
#else
        JsonNamingPolicy namingPolicy = SnakeCaseLowerNamingPolicy.Instance;
#endif

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = namingPolicy,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        options.Converters.Add(new JsonStringEnumConverter(namingPolicy));
        options.Converters.Add(new UtcDateTimeOffsetConverter());
        options.Converters.Add(new NullableUtcDateTimeOffsetConverter());

        return options;
    }

#if !NET8_0_OR_GREATER
    /// <summary>
    /// A snake_case naming policy for .NET 6 and .NET 7 where
    /// <c>JsonNamingPolicy.SnakeCaseLower</c> is not available.
    /// </summary>
    private sealed class SnakeCaseLowerNamingPolicy : JsonNamingPolicy
    {
        public static SnakeCaseLowerNamingPolicy Instance { get; } = new();

        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var builder = new System.Text.StringBuilder(name.Length + 4);

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];

                if (char.IsUpper(c))
                {
                    if (i > 0)
                    {
                        builder.Append('_');
                    }

                    builder.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }
    }
#endif
}
