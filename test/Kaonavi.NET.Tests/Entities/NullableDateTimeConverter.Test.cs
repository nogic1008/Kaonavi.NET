namespace Kaonavi.Net.Tests.Entities;

using System.Buffers;
using System.Globalization;
using System.Text;
using Kaonavi.Net.Entities;

/// <summary>
/// <see cref="NullableDateTimeConverter"/>の単体テスト
/// </summary>
public class NullableDateTimeConverterTest
{
    /// <summary>
    /// <see cref="NullableDateTimeConverter.Write"/>は、<see langword="DateTime?"/>をJSON形式に正しくシリアライズできる。
    /// </summary>
    /// <param name="dateTimeString"><see cref="DateTime"/>の文字列表現</param>
    /// <param name="expectedJson">JSON文字列</param>
    [Theory(DisplayName = $"{nameof(NullableDateTimeConverter)} > ${nameof(NullableDateTimeConverter.Write)}")]
    [InlineData(null, "null")]
    [InlineData("2020/01/01", "\"2020-01-01\"")]
    [InlineData("2020/01/01 5:00:00", "\"2020-01-01 05:00:00\"")]
    public void Write_CanSerialize_NullableDateTime(string? dateTimeString, string expectedJson)
    {
        // Arrange
        DateTime? date = dateTimeString is null ? null
            : DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new Utf8JsonWriter(buffer);

        // Act
        var converter = new NullableDateTimeConverter();
        converter.Write(writer, date, new(JsonSerializerDefaults.Web));
        writer.Flush();

        // Assert
        string written = Encoding.UTF8.GetString(buffer.WrittenSpan);
        written.Should().Be(expectedJson);
    }

    /// <summary>
    /// <see cref="NullableDateTimeConverter.Read"/>は、JSON形式から<see langword="DateTime?"/>を正しくデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="expectedString"><see cref="DateTime"/>の文字列表現</param>
    [Theory(DisplayName = $"{nameof(NullableDateTimeConverter)} > ${nameof(NullableDateTimeConverter.Read)}")]
    [InlineData("null", null)]
    [InlineData("\"\"", null)]
    [InlineData("\"2020-01-01\"", "01/01/2020 00:00:00")]
    [InlineData("\"2020-01-01 05:00:00\"", "01/01/2020 05:00:00")]
    public void Read_CanDeserialize_NullableDateTime(string json, string? expectedString)
    {
        // Arrange
        byte[] jsonData = Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(jsonData);
        reader.Read();

        // Act
        var converter = new NullableDateTimeConverter();
        var date = converter.Read(ref reader, typeof(DateTime?), new());

        // Assert
        if (expectedString is null)
            date.Should().BeNull();
        else
            date.GetValueOrDefault().ToString(CultureInfo.InvariantCulture).Should().Be(expectedString);
    }
}
