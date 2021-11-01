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
        /// <param name="code"><inheritdoc cref="Code" path="/summary"/></param>
        /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
        /// <param name="nameKana"><inheritdoc cref="NameKana" path="/summary"/></param>
        /// <param name="mail"><inheritdoc cref="Mail" path="/summary"/></param>
        /// <param name="enteredDate"><inheritdoc cref="EnteredDate" path="/summary"/></param>
        /// <param name="retiredDate"><inheritdoc cref="RetiredDate" path="/summary"/></param>
        /// <param name="gender"><inheritdoc cref="Gender" path="/summary"/></param>
        /// <param name="birthday"><inheritdoc cref="Birthday" path="/summary"/></param>
        /// <param name="department"><inheritdoc cref="Department" path="/summary"/></param>
        /// <param name="subDepartments"><inheritdoc cref="SubDepartments" path="/summary"/></param>
        /// <param name="customFields"><inheritdoc cref="CustomFields" path="/summary"/></param>
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
