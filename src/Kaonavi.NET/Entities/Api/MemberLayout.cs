using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities.Api
{
    /// <summary>メンバー情報(基本情報/所属/兼務情報) レイアウト定義</summary>
    public record MemberLayout(
        /// <summary>社員番号</summary>
        [property: JsonPropertyName("code")] Field Code,
        /// <summary>氏名</summary>
        [property: JsonPropertyName("name")] Field Name,
        /// <summary>フリガナ</summary>
        [property: JsonPropertyName("name_kana")] Field NameKana,
        /// <summary>メールアドレス</summary>
        [property: JsonPropertyName("mail")] Field Mail,
        /// <summary>入社日</summary>
        [property: JsonPropertyName("entered_date")] Field EnteredDate,
        /// <summary>退職日</summary>
        [property: JsonPropertyName("retired_date")] Field RetiredDate,
        /// <summary>性別</summary>
        [property: JsonPropertyName("gender")] Field Gender,
        /// <summary>生年月日</summary>
        [property: JsonPropertyName("birthday")] Field Birthday,
        /// <summary>所属</summary>
        [property: JsonPropertyName("department")] Field Department,
        /// <summary>兼務情報</summary>
        [property: JsonPropertyName("sub_departments")] Field SubDepartments,
        /// <summary>基本情報のカスタム項目のレイアウト定義リスト</summary>
        [property: JsonPropertyName("custom_fields")] IEnumerable<CustomField> CustomFields
    );
}
