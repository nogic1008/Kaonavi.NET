using System.Buffers;
using System.Text;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Json;

/// <summary><see cref="BlankNullableDateConverter"/>の単体テスト</summary>
[TestClass]
public sealed class BlankNullableDateConverterTest
{
    /// <summary>
    /// <see cref="BlankNullableDateConverter.Read"/>は、JSON文字列から<see cref="DateOnly"/>?に変換できる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <inheritdoc cref="DateTime.DateTime(int, int, int, int, int, int)" path="/param"/>
    [TestMethod(DisplayName = $"{nameof(BlankNullableDateConverter)} > {nameof(BlankNullableDateConverter.Read)}()"), TestCategory("JSON Converter")]
    [DataRow(/*lang=json,strict*/ "\"2021-01-01\"", 2021, 1, 1)]
    [DataRow(/*lang=json,strict*/ "\"\"", null, 0, 0)]
    [DataRow(/*lang=json,strict*/ "null", null, 0, 0)]
    public void Read_Returns_NullableOfDateOnly(string json, int? year, int month, int day)
    {
        // Arrange
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        while (reader.TokenType == JsonTokenType.None)
            reader.Read();
        DateOnly? expected = year is null ? null : new DateOnly(year.GetValueOrDefault(), month, day);
        var sut = new BlankNullableDateConverter();

        // Act
        var actual = sut.Read(ref reader, typeof(DateOnly?), JsonSerializerOptions.Default);

        // Assert
        actual.ShouldBe(expected);
    }

    /// <summary>
    /// <see cref="BlankNullableDateConverter.Write"/>は、<see cref="DateOnly"/>?からJSON文字列に変換できる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <inheritdoc cref="DateTime.DateTime(int, int, int, int, int, int)" path="/param"/>
    [TestMethod(DisplayName = $"{nameof(BlankNullableDateConverter)} > {nameof(BlankNullableDateConverter.Write)}()"), TestCategory("JSON Converter")]
    [DataRow(2021, 1, 1, /*lang=json,strict*/ "\"2021-01-01\"")]
    [DataRow(null, 0, 0, /*lang=json,strict*/ "\"\"")]
    public void Write_Flushes_JSON(int? year, int month, int day, string json)
    {
        // Arrange
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        DateOnly? value = year is null ? null : new DateOnly(year.GetValueOrDefault(), month, day);
        var sut = new BlankNullableDateConverter();

        // Act
        sut.Write(writer, value, JsonSerializerOptions.Default);
        writer.Flush();

        // Assert
        buffer.WrittenSpan.ToArray().ShouldBe(Encoding.UTF8.GetBytes(json));
    }
}
