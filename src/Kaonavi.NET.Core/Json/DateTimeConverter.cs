using System.Globalization;

namespace Kaonavi.Net.Json;

/// <summary>"yyyy-MM-dd HH:mm:ss" - <see cref="DateTime"/>変換</summary>
public class DateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd HH:mm:ss";
    /// <inheritdoc/>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTime.ParseExact(reader.GetString()!, Format, CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
#if NETSTANDARD2_1
        // .NET Standard 2.1にDateTime.TryFormat(Span<byte>)がないため、TryFormat(Span<char>)を使った実装
        Span<char> buffer = stackalloc char[21]; // "yyyy-MM-dd HH:mm:ss".Length
        buffer[0] = '"';
        _ = value.TryFormat(buffer[1..], out int written, Format, CultureInfo.InvariantCulture);
        buffer[written + 1] = '"';
        writer.WriteRawValue(buffer[0..(written + 2)], false);
#else
        Span<byte> buffer = stackalloc byte[21]; // "yyyy-MM-dd HH:mm:ss".Length
        buffer[0] = (byte)'"';
        _ = value.TryFormat(buffer[1..], out int written, Format, CultureInfo.InvariantCulture);
        buffer[written + 1] = (byte)'"';
        writer.WriteRawValue(buffer[0..(written + 2)], false);
#endif
    }
}
