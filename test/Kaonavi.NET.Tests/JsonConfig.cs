using System.Text.Json;
using System.Text.Json.Serialization;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests
{
    public static class JsonConfig
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
}
