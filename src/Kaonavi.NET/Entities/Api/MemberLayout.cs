using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities.Api
{
    /// <summary>メンバー情報(基本情報/所属/兼務情報) レイアウト定義</summary>
    public record MemberLayout
    {
        /// <summary>
        /// MemberLayoutの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="code">社員番号</param>
        /// <param name="name">氏名</param>
        /// <param name="nameKana">フリガナ</param>
        /// <param name="mail">メールアドレス</param>
        /// <param name="enteredDate">入社日</param>
        /// <param name="retiredDate">退職日</param>
        /// <param name="gender">性別</param>
        /// <param name="birthday">生年月日</param>
        /// <param name="department">所属</param>
        /// <param name="subDepartments">兼務情報</param>
        /// <param name="customFields">基本情報のカスタム項目のレイアウト定義リスト</param>
        public MemberLayout(Field code, Field name, Field nameKana, Field mail, Field enteredDate, Field retiredDate, Field gender, Field birthday, Field department, Field subDepartments, IReadOnlyList<CustomField> customFields)
            => (Code, Name, NameKana, Mail, EnteredDate, RetiredDate, Gender, Birthday, Department, SubDepartments, CustomFields)
                = (code, name, nameKana, mail, enteredDate, retiredDate, gender, birthday, department, subDepartments, customFields);

        /// <summary>社員番号</summary>
        [JsonPropertyName("code")]
        public Field Code { get; init; }

        /// <summary>氏名</summary>
        [JsonPropertyName("name")]
        public Field Name { get; init; }

        /// <summary>フリガナ</summary>
        [JsonPropertyName("name_kana")]
        public Field NameKana { get; init; }

        /// <summary>メールアドレス</summary>
        [JsonPropertyName("mail")]
        public Field Mail { get; init; }

        /// <summary>入社日</summary>
        [JsonPropertyName("entered_date")]
        public Field EnteredDate { get; init; }

        /// <summary>退職日</summary>
        [JsonPropertyName("retired_date")]
        public Field RetiredDate { get; init; }

        /// <summary>性別</summary>
        [JsonPropertyName("gender")]
        public Field Gender { get; init; }

        /// <summary>生年月日</summary>
        [JsonPropertyName("birthday")]
        public Field Birthday { get; init; }

        /// <summary>所属</summary>
        [JsonPropertyName("department")]
        public Field Department { get; init; }

        /// <summary>兼務情報</summary>
        [JsonPropertyName("sub_departments")]
        public Field SubDepartments { get; init; }

        /// <summary>基本情報のカスタム項目のレイアウト定義リスト</summary>
        [JsonPropertyName("custom_fields")]
        public IReadOnlyList<CustomField> CustomFields { get; init; }
    }
}
