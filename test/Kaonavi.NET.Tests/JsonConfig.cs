using System.Text.Json;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests
{
    public static class JsonConfig
    {
        internal static readonly JsonSerializerOptions Default;
        static JsonConfig()
        {
            Default = new();
            Default.Converters.Add(new NullableDateTimeConverter());
        }
    }
}
