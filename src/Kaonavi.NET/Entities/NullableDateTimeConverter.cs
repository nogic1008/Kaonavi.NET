namespace Kaonavi.Net.Entities;

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// "yyyy-MM-dd HH:mm:ss" &lt;-&gt; DateTime? 変換
/// </summary>
public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    /// <inheritdoc/>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTime.TryParseExact(reader.GetString(), DateTimeFormat, null, DateTimeStyles.None, out var dateTime) ? dateTime
            : DateTime.TryParseExact(reader.GetString(), DateFormat, null, DateTimeStyles.None, out dateTime) ? dateTime
            : null;

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }
        var dateTime = value.GetValueOrDefault();
        if (dateTime == dateTime.Date)
            writer.WriteStringValue(dateTime.ToString(DateFormat));
        else
            writer.WriteStringValue(dateTime.ToString(DateTimeFormat));
    }
}
