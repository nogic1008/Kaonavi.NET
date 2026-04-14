using System.Globalization;

namespace Kaonavi.Net.Json;

/// <summary>"yyyy-MM-dd" - <see cref="DateOnly"/>変換</summary>
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    /// <inheritdoc/>
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateOnly.Parse(reader.GetString()!, CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
#if NETSTANDARD2_1
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
#else
        Span<byte> buffer = stackalloc byte[12]; // "yyyy-MM-dd".Length + 2 quotes
        buffer[0] = (byte)'"';
        _ = value.TryFormat(buffer[1..], out int written, Format, CultureInfo.InvariantCulture);
        buffer[written + 1] = (byte)'"';
        writer.WriteRawValue(buffer[0..(written + 2)], false);
#endif
    }
}
