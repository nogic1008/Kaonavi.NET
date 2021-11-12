using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>"yyyy-MM-dd" &lt;-&gt; <see langword="DateTime?"/>変換</summary>
    internal class DateConverter : JsonConverter<DateTime?>
    {
        private const string DateFormat = "yyyy-MM-dd";

        /// <inheritdoc/>
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.TryParseExact(reader.GetString(), DateFormat, null, DateTimeStyles.None, out var dateTime) ? dateTime
                : null;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value is DateTime dateTime)
                writer.WriteStringValue(dateTime.ToString(DateFormat));
            else
                writer.WriteNullValue();
        }
    }
}
