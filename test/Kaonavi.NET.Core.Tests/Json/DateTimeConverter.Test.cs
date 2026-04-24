using System.Buffers;
using System.Text;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Json;

/// <summary><see cref="DateTimeConverter"/>の単体テスト</summary>
[TestClass]
public sealed class DateTimeConverterTest
{
    /// <summary>
    /// <see cref="DateTimeConverter.Read"/>は、JSON文字列から<see cref="DateTime"/>に変換できる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <inheritdoc cref="DateTime.DateTime(int, int, int, int, int, int)" path="/param"/>
    [TestMethod(DisplayName = $"{nameof(DateTimeConverter)} > {nameof(DateTimeConverter.Read)}()"), TestCategory("JSON Converter")]
    [DataRow(/*lang=json,strict*/ "\"2021-01-01 12:34:56\"", 2021, 1, 1, 12, 34, 56)]
    public void Read_Returns_DateTime(string json, int year, int month, int day, int hour, int minute, int second)
    {
        // Arrange
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        while (reader.TokenType == JsonTokenType.None)
            reader.Read();
        var sut = new DateTimeConverter();
        // Act
        var actual = sut.Read(ref reader, typeof(DateTime), JsonSerializerOptions.Default);
        // Assert
        actual.ShouldBe(new(year, month, day, hour, minute, second));
    }

    /// <summary>
    /// <see cref="DateTimeConverter.Write"/>は、<see cref="DateTime"/>からJSON文字列に変換できる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <inheritdoc cref="DateTime.DateTime(int, int, int, int, int, int)" path="/param"/>
    [TestMethod(DisplayName = $"{nameof(DateTimeConverter)} > {nameof(DateTimeConverter.Write)}()"), TestCategory("JSON Converter")]
    [DataRow(2021, 1, 1, 12, 34, 56, /*lang=json,strict*/ "\"2021-01-01 12:34:56\"")]
    public void Write_Flushes_JSON(int year, int month, int day, int hour, int minute, int second, string json)
    {
        // Arrange
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new Utf8JsonWriter(buffer);
        var sut = new DateTimeConverter();

        // Act
        sut.Write(writer, new(year, month, day, hour, minute, second), JsonSerializerOptions.Default);
        writer.Flush();

        // Assert
        buffer.WrittenSpan.ToArray().ShouldBe(Encoding.UTF8.GetBytes(json));
    }
}
