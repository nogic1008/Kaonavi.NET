namespace Kaonavi.Net.Tests;

using System.Text.Json.Serialization;

internal static class JsonConfig
{
    internal static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
