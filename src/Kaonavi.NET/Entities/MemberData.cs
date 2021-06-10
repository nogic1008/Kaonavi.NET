using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>メンバー情報(基本情報/所属(主務)/兼務情報)</summary>
    public record MemberData
    {
        /// <summary>
        /// MemberDataの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="code">社員番号</param>
        /// <param name="name">氏名</param>
        /// <param name="nameKana">フリガナ</param>
        /// <param name="mail">メールアドレス</param>
        /// <param name="enteredDate">入社日</param>
        /// <param name="retiredDate">退職日</param>
        /// <param name="gender">性別</param>
        /// <param name="birthday">生年月日</param>
        /// <param name="department">主務情報</param>
        /// <param name="subDepartments">兼務情報リスト</param>
        /// <param name="customFields">カスタム項目値</param>
        public MemberData(
            string code,
            string? name = null,
            string? nameKana = null,
            string? mail = null,
            DateTime? enteredDate = default,
            DateTime? retiredDate = default,
            string? gender = null,
            DateTime? birthday = default,
            Department? department = null,
            IReadOnlyList<Department>? subDepartments = null,
            IReadOnlyList<CustomFieldValue>? customFields = null)
            => (Code, Name, NameKana, Mail, EnteredDate, RetiredDate, Gender, Birthday, Department, SubDepartments, CustomFields)
                = (code, name, nameKana, mail, enteredDate, retiredDate, gender, birthday, department, subDepartments, customFields);

        /// <summary>社員番号</summary>
        [JsonPropertyName("code")]
        public string Code { get; init; }

        /// <summary>氏名</summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        /// <summary>フリガナ</summary>
        [JsonPropertyName("name_kana")]
        public string? NameKana { get; init; }

        /// <summary>メールアドレス</summary>
        [JsonPropertyName("mail")]
        public string? Mail { get; init; }

        /// <summary>入社日</summary>
        [JsonPropertyName("entered_date")]
        public DateTime? EnteredDate { get; init; }

        /// <summary>退職日</summary>
        [JsonPropertyName("retired_date")]
        public DateTime? RetiredDate { get; init; }

        /// <summary>性別</summary>
        [JsonPropertyName("gender")]
        public string? Gender { get; init; }

        /// <summary>生年月日</summary>
        [JsonPropertyName("birthday")]
        public DateTime? Birthday { get; init; }

        /// <summary>主務情報</summary>
        [JsonPropertyName("department")]
        public Department? Department { get; init; }

        /// <summary>兼務情報リスト</summary>
        [JsonPropertyName("sub_departments")]
        public IReadOnlyList<Department>? SubDepartments { get; init; }

        /// <summary>カスタム項目値</summary>
        [JsonPropertyName("custom_fields")]
        public IReadOnlyList<CustomFieldValue>? CustomFields { get; init; }
    }
}
