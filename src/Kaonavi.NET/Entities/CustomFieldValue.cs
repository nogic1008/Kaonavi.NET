using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    public record CustomFieldValue
    {
        public CustomFieldValue(int id, string? name, string value) : this(id, name, new[] { value }) { }

        [JsonConstructor]
        public CustomFieldValue(int id, string? name, IEnumerable<string> values)
            => (Id, Name, Values) = (id, name, values);

        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("values")]
        public IEnumerable<string> Values { get; init; }
    }
}
