using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Json;

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
