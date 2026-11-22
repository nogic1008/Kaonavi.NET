using System.Globalization;

namespace Kaonavi.Net.Json;

/// <summary>
/// <c>""</c>を<c>null</c>とみなす<see cref="Nullable{DateOnly}"/>用のJsonConverter実装。
/// </summary>
public class BlankNullableDateConverter : JsonConverter<DateOnly?>
{
    private readonly BlankNullableConverter<DateOnly> _converter = new(new DateOnlyConverter());

    /// <inheritdoc/>
    public override bool HandleNull => _converter.HandleNull;

    /// <inheritdoc/>
    public override DateOnly? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) => _converter.Read(ref reader, typeToConvert, options);

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        DateOnly? value,
        JsonSerializerOptions options
    ) => _converter.Write(writer, value, options);

    private class DateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        ) => DateOnly.Parse(reader.GetString()!);

        public override void Write(
            Utf8JsonWriter writer,
            DateOnly value,
            JsonSerializerOptions options
        )
        {
            Span<byte> buffer = stackalloc byte[12]; // "yyyy-MM-dd".Length
            buffer[0] = (byte)'"';
            _ = value.TryFormat(buffer[1..], out int written, "o", CultureInfo.InvariantCulture);
            buffer[written + 1] = (byte)'"';
            writer.WriteRawValue(buffer[0..(written + 2)], false);
        }
    }
}
