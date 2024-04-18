using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Json;

/// <inheritdoc/>
internal class FieldTypeJsonConverter : JsonConverter<FieldType>
{
    private static ReadOnlySpan<byte> FieldTypeString => "string"u8;
    private static ReadOnlySpan<byte> FieldTypeNumber => "number"u8;
    private static ReadOnlySpan<byte> FieldTypeDate => "date"u8;
    private static ReadOnlySpan<byte> FieldTypeEnum => "enum"u8;
    private static ReadOnlySpan<byte> FieldTypeCalc => "calc"u8;
    private static ReadOnlySpan<byte> FieldTypeDepartment => "department"u8;
    private static ReadOnlySpan<byte> FieldTypeDepartmentArray => "department[]"u8;

    /// <inheritdoc/>
    public override FieldType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.ValueSpan.SequenceEqual(FieldTypeString) ? FieldType.String
        : reader.ValueSpan.SequenceEqual(FieldTypeNumber) ? FieldType.Number
        : reader.ValueSpan.SequenceEqual(FieldTypeDate) ? FieldType.Date
        : reader.ValueSpan.SequenceEqual(FieldTypeEnum) ? FieldType.Enum
        : reader.ValueSpan.SequenceEqual(FieldTypeCalc) ? FieldType.Calc
        : reader.ValueSpan.SequenceEqual(FieldTypeDepartment) ? FieldType.Department
        : reader.ValueSpan.SequenceEqual(FieldTypeDepartmentArray) ? FieldType.DepartmentArray
        : throw new JsonException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, FieldType value, JsonSerializerOptions options)
        => writer.WriteStringValue(value switch
        {
            FieldType.String => FieldTypeString,
            FieldType.Number => FieldTypeNumber,
            FieldType.Date => FieldTypeDate,
            FieldType.Enum => FieldTypeEnum,
            FieldType.Calc => FieldTypeCalc,
            FieldType.Department => FieldTypeDepartment,
            FieldType.DepartmentArray => FieldTypeDepartmentArray,
            _ => throw new JsonException(),
        });
}
