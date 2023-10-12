using System.ComponentModel;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Entities;

/// <summary>フィールドの入力タイプ</summary>
[JsonConverter(typeof(FieldTypeJsonConverter))]
public enum FieldType
{
    /// <summary>文字列</summary>
    String = 0,
    /// <summary>数値</summary>
    Number = 1,
    /// <summary>日付・年月</summary>
    Date = 2,
    /// <summary>リスト項目</summary>
    Enum = 3,
    /// <summary><see cref="MemberDepartment"/></summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    Department = 4,
    /// <summary><see cref="MemberDepartment"/>の配列</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    DepartmentArray = 5,
}
