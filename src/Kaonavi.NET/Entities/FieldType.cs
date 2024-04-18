using System.ComponentModel;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Entities;

/// <summary>フィールドの入力タイプ</summary>
[JsonConverter(typeof(FieldTypeJsonConverter))]
public enum FieldType
{
    /// <summary>文字列</summary>
    String,
    /// <summary>数値</summary>
    Number,
    /// <summary>日付・年月</summary>
    Date,
    /// <summary>リスト項目</summary>
    Enum,
    /// <summary>計算式パーツ</summary>
    Calc,
    /// <summary><see cref="MemberDepartment"/></summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    Department,
    /// <summary><see cref="MemberDepartment"/>の配列</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    DepartmentArray,
}
