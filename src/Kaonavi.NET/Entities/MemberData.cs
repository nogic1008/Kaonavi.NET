using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>メンバー情報(基本情報/所属(主務)/兼務情報)</summary>
    public record MemberData(
        /// <summary>社員番号</summary>
        [property: JsonPropertyName("code")] string Code,
        /// <summary>氏名</summary>
        [property: JsonPropertyName("name")] string? Name = null,
        /// <summary>フリガナ</summary>
        [property: JsonPropertyName("name_kana")] string? NameKana = null,
        /// <summary>メールアドレス</summary>
        [property: JsonPropertyName("mail")] string? Mail = null,
        /// <summary>入社日</summary>
        [property: JsonPropertyName("entered_date")] DateTime? EnteredDate = default,
        /// <summary>退職日</summary>
        [property: JsonPropertyName("retired_date")] DateTime? RetiredDate = default,
        /// <summary>性別</summary>
        [property: JsonPropertyName("gender")] string? Gender = null,
        /// <summary>生年月日</summary>
        [property: JsonPropertyName("birthday")] DateTime? Birthday = default,
        /// <summary>主務情報</summary>
        [property: JsonPropertyName("department")] Department? Department = null,
        /// <summary>兼務情報リスト</summary>
        [property: JsonPropertyName("sub_departments")] IEnumerable<Department>? SubDepartments = null,
        /// <summary>カスタム項目値</summary>
        [property: JsonPropertyName("custom_fields")] IEnumerable<CustomFieldValue>? CustomFields = null
    );
}
