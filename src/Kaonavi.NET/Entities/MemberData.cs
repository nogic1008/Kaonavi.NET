namespace Kaonavi.Net.Entities;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>メンバー情報(基本情報/所属(主務)/兼務情報)</summary>
/// <param name="Code">社員番号</param>
/// <param name="Name">氏名</param>
/// <param name="NameKana">フリガナ</param>
/// <param name="Mail">メールアドレス</param>
/// <param name="EnteredDate">入社日</param>
/// <param name="RetiredDate">退職日</param>
/// <param name="Gender">性別</param>
/// <param name="Birthday">生年月日</param>
/// <param name="Department">主務情報</param>
/// <param name="SubDepartments">兼務情報リスト</param>
/// <param name="CustomFields">カスタム項目値</param>
public record MemberData(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("name_kana")] string? NameKana = null,
    [property: JsonPropertyName("mail")] string? Mail = null,
    [property: JsonPropertyName("entered_date")] DateTime? EnteredDate = default,
    [property: JsonPropertyName("retired_date")] DateTime? RetiredDate = default,
    [property: JsonPropertyName("gender")] string? Gender = null,
    [property: JsonPropertyName("birthday")] DateTime? Birthday = default,
    [property: JsonPropertyName("department")] MemberDepartment? Department = null,
    [property: JsonPropertyName("sub_departments")] IReadOnlyList<MemberDepartment>? SubDepartments = null,
    [property: JsonPropertyName("custom_fields")] IReadOnlyList<CustomFieldValue>? CustomFields = null
);
