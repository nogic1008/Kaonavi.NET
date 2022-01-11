namespace Kaonavi.Net.Tests;

using System.Text.Json.Serialization;
using Kaonavi.Net.Entities;

internal static class JsonConfig
{
    internal static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        Converters = { new DateOnlyConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}
