using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    public record Department(
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("name")] string? Name = null,
        [property: JsonPropertyName("names")] IEnumerable<string>? Names = null
    );
}
