using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities.Api
{
    public record MemberLayout(
        [property: JsonPropertyName("code")] Field Code,
        [property: JsonPropertyName("name")] Field Name,
        [property: JsonPropertyName("name_kana")] Field NameKana,
        [property: JsonPropertyName("mail")] Field Mail,
        [property: JsonPropertyName("entered_date")] Field EnteredDate,
        [property: JsonPropertyName("retired_date")] Field RetiredDate,
        [property: JsonPropertyName("gender")] Field Gender,
        [property: JsonPropertyName("birthday")] Field Birthday,
        [property: JsonPropertyName("department")] Field Department,
        [property: JsonPropertyName("sub_departments")] Field SubDepartments,
        [property: JsonPropertyName("custom_fields")] IEnumerable<CustomField> CustomFields
    );
}
