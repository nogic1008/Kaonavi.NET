using System.Buffers;
using System.Text;
using System.Text.Json;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Json;

/// <summary><see cref="BlankNullableDateConverter"/>の単体テスト</summary>
[Category("JSON Converter")]
public sealed class BlankNullableDateConverterTest
{
    /// <summary>
    /// <see cref="BlankNullableDateConverter.Read"/>は、JSON文字列から<see cref="DateOnly"/>?に変換できる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <inheritdoc cref="DateTime.DateTime(int, int, int, int, int, int)" path="/param"/>
    [Test($"{nameof(BlankNullableDateConverter)} > {nameof(BlankNullableDateConverter.Read)}()")]
    [Arguments(/*lang=json,strict*/ "\"2021-01-01\"", 2021, 1, 1)]
    [Arguments(/*lang=json,strict*/ "\"\"", null, 0, 0)]
    [Arguments(/*lang=json,strict*/ "null", null, 0, 0)]
    public async Task Read_Returns_NullableOfDateOnly(string json, int? year, int month, int day)
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
        await Assert.That(actual).IsEqualTo(expected);
    }

    /// <summary>
    /// <see cref="BlankNullableDateConverter.Write"/>は、<see cref="DateOnly"/>?からJSON文字列に変換できる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <inheritdoc cref="DateTime.DateTime(int, int, int, int, int, int)" path="/param"/>
    [Test($"{nameof(BlankNullableDateConverter)} > {nameof(BlankNullableDateConverter.Write)}()")]
    [Arguments(2021, 1, 1, /*lang=json,strict*/ "\"2021-01-01\"")]
    [Arguments(null, 0, 0, /*lang=json,strict*/ "\"\"")]
    public async Task Write_Flushes_JSON(int? year, int month, int day, string json)
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
        await Assert.That(buffer.WrittenSpan.ToArray()).IsEquivalentTo(Encoding.UTF8.GetBytes(json));
    }
}
