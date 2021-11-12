namespace Kaonavi.Net.Entities;

using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    /// <summary><see cref="MemberDepartment"/></summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    Department,
    /// <summary><see cref="MemberDepartment"/>の配列</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    DepartmentArray,
}

/// <inheritdoc/>
internal class FieldTypeJsonConverter : JsonConverter<FieldType>
{
    /// <inheritdoc/>
    public override FieldType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetString() switch
        {
            "string" => FieldType.String,
            "number" => FieldType.Number,
            "date" => FieldType.Date,
            "enum" => FieldType.Enum,
            "department" => FieldType.Department,
            "department[]" => FieldType.DepartmentArray,
            _ => throw new JsonException(),
        };

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, FieldType value, JsonSerializerOptions options)
        => writer.WriteStringValue(value switch
        {
            FieldType.String => "string",
            FieldType.Number => "number",
            FieldType.Date => "date",
            FieldType.Enum => "enum",
            FieldType.Department => "department",
            FieldType.DepartmentArray => "department[]",
            _ => throw new JsonException(),
        });
}
