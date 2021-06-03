using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities.Api
{
    public record SheetLayout(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("record_type")] RecordType RecordType,
        [property: JsonPropertyName("custom_fields")] IEnumerable<CustomField> CustomFields
    );

    public enum RecordType
    {
        Single = 0,
        Multiple = 1,
    }
}
