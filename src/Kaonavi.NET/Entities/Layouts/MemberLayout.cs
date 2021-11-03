using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>メンバー情報(基本情報/所属/兼務情報) レイアウト定義</summary>
    /// <param name="Code"><inheritdoc cref="MemberData" path="/param[@name='Code']/text()"/></param>
    /// <param name="Name"><inheritdoc cref="MemberData" path="/param[@name='Name']/text()"/></param>
    /// <param name="NameKana"><inheritdoc cref="MemberData" path="/param[@name='NameKana']/text()"/></param>
    /// <param name="Mail"><inheritdoc cref="MemberData" path="/param[@name='Mail']/text()"/></param>
    /// <param name="EnteredDate"><inheritdoc cref="MemberData" path="/param[@name='EnteredDate']/text()"/></param>
    /// <param name="RetiredDate"><inheritdoc cref="MemberData" path="/param[@name='RetiredDate']/text()"/></param>
    /// <param name="Gender"><inheritdoc cref="MemberData" path="/param[@name='Gender']/text()"/></param>
    /// <param name="Birthday"><inheritdoc cref="MemberData" path="/param[@name='Birthday']/text()"/></param>
    /// <param name="Department">所属</param>
    /// <param name="SubDepartments">兼務情報</param>
    /// <param name="CustomFields">基本情報のカスタム項目のレイアウト定義リスト</param>
    public record MemberLayout(
        [property: JsonPropertyName("code")] FieldLayout Code,
        [property: JsonPropertyName("name")] FieldLayout Name,
        [property: JsonPropertyName("name_kana")] FieldLayout NameKana,
        [property: JsonPropertyName("mail")] FieldLayout Mail,
        [property: JsonPropertyName("entered_date")] FieldLayout EnteredDate,
        [property: JsonPropertyName("retired_date")] FieldLayout RetiredDate,
        [property: JsonPropertyName("gender")] FieldLayout Gender,
        [property: JsonPropertyName("birthday")] FieldLayout Birthday,
        [property: JsonPropertyName("department")] FieldLayout Department,
        [property: JsonPropertyName("sub_departments")] FieldLayout SubDepartments,
        [property: JsonPropertyName("custom_fields")] IReadOnlyList<CustomFieldLayout> CustomFields
    );
}
