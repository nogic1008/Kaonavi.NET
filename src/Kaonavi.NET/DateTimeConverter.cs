using System.Diagnostics.CodeAnalysis;

namespace Kaonavi.Net;

/// <summary>"yyyy-MM-dd hh:mm:ss" - <see cref="DateTime"/>変換</summary>
public class DateTimeConverter : JsonConverter<DateTime>
{
    /// <inheritdoc/>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTime.Parse(reader.GetString()!);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString("yyyy-MM-dd hh:mm:ss"));
}
