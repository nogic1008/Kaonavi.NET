using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>主務/兼務情報</summary>
    /// <param name="Code"><inheritdoc cref="DepartmentTree" path="/param[@name='Code']/text()"/></param>
    /// <param name="Name">親所属を含む全ての所属名を半角スペース区切りで返却</param>
    /// <param name="Names">親所属を含む全ての所属名を配列で返却</param>
    public record MemberDepartment(
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("name")] string? Name = null,
        [property: JsonPropertyName("names")] IReadOnlyList<string>? Names = null
    );
}
