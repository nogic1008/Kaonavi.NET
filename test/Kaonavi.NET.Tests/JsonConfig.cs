namespace Kaonavi.Net.Tests;

using System.Text.Json.Serialization;
using Kaonavi.Net.Entities;

internal static class JsonConfig
{
    internal static readonly JsonSerializerOptions Default;
    static JsonConfig()
    {
        Default = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        Default.Converters.Add(new NullableDateTimeConverter());
    }
}
