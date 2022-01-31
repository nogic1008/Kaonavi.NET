using System.Text.Json.Serialization;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests;

internal static class JsonConfig
{
    internal static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        Converters = { new DateOnlyConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}
