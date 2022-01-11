namespace Kaonavi.Net.Entities;

/// <summary>"yyyy-MM-dd" &lt;-&gt; <see langword="DateOnly?"/>変換</summary>
internal class DateOnlyConverter : JsonConverter<DateOnly?>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateOnly.TryParseExact(reader.GetString(), DateFormat, out var date) ? date : null;

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (value is DateOnly date)
            writer.WriteStringValue(date.ToString(DateFormat));
        else
            writer.WriteNullValue();
    }
}
