using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    public record Field(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("required")] bool Required,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("max_length")] int? MaxLength,
        [property: JsonPropertyName("enum")] IEnumerable<string> Enum
    );

    public record CustomField(
        [property: JsonPropertyName("id")] int Id,
        string Name,
        bool Required,
        string Type,
        int? MaxLength,
        IEnumerable<string> Enum
    ) : Field(Name, Required, Type, MaxLength, Enum);
}
