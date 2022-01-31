using System.Text.Json.Serialization;
using Nogic.JsonConverters;

namespace Kaonavi.Net.Tests;

internal static class JsonConfig
{
    internal static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new BlankNullableConverter<DateOnly>(new DateOnlyConverter()),
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}
