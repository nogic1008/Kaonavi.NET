namespace Kaonavi.Net.Tests;

using System.Text.Json.Serialization;
using Kaonavi.Net.Services;

internal static class JsonConfig
{
    internal static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonConverterFactoryForApiResult() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
