using Kaonavi.Net.Json;

namespace Kaonavi.Net.Entities;

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
    string Code,
    string? Name = null,
    string? NameKana = null,
    string? Mail = null,
    [property: JsonConverter(typeof(BlankNullableConverterFactory))] DateOnly? EnteredDate = default,
    [property: JsonConverter(typeof(BlankNullableConverterFactory))] DateOnly? RetiredDate = default,
    string? Gender = null,
    DateOnly? Birthday = default,
    MemberDepartment? Department = null,
    IReadOnlyList<MemberDepartment>? SubDepartments = null,
    IReadOnlyList<CustomFieldValue>? CustomFields = null
);
