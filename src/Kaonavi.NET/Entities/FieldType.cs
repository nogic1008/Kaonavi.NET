using System.ComponentModel;

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
