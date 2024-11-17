using System.ComponentModel;

namespace Kaonavi.Net.Entities;

/// <summary>フィールドの入力タイプ</summary>
[JsonConverter(typeof(JsonStringEnumConverter<FieldType>))]
public enum FieldType
{
    /// <summary>文字列</summary>
    [JsonStringEnumMemberName("string")] String,
    /// <summary>数値</summary>
    [JsonStringEnumMemberName("number")] Number,
    /// <summary>日付・年月</summary>
    [JsonStringEnumMemberName("date")] Date,
    /// <summary>リスト項目</summary>
    [JsonStringEnumMemberName("enum")] Enum,
    /// <summary>計算式パーツ</summary>
    [JsonStringEnumMemberName("calc")] Calc,
    /// <summary><see cref="MemberDepartment"/></summary>
    [EditorBrowsable(EditorBrowsableState.Never), JsonStringEnumMemberName("department")] Department,
    /// <summary><see cref="MemberDepartment"/>の配列</summary>
    [EditorBrowsable(EditorBrowsableState.Never), JsonStringEnumMemberName("department[]")] DepartmentArray,
}
