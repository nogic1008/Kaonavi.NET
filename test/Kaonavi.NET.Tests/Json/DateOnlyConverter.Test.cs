using System.Buffers;
using System.Text;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Json;

/// <summary><see cref="DateOnlyConverter"/>の単体テスト</summary>
[TestClass]
public sealed class DateOnlyConverterTest
{
    /// <summary>
    /// <see cref="DateOnlyConverter.Read"/>は、JSON文字列から<see cref="DateOnly"/>に変換できる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <inheritdoc cref="DateTime.DateTime(int, int, int)" path="/param"/>
    [TestMethod(DisplayName = $"{nameof(DateOnlyConverter)} > {nameof(DateOnlyConverter.Read)}()"), TestCategory("JSON Converter")]
    [DataRow(/*lang=json,strict*/ "\"2021-01-01\"", 2021, 1, 1)]
    [DataRow(/*lang=json,strict*/ "\"1986-05-16\"", 1986, 5, 16)]
    public void Read_Returns_DateOnly(string json, int year, int month, int day)
    {
        // Arrange
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        while (reader.TokenType == JsonTokenType.None)
            reader.Read();
        var sut = new DateOnlyConverter();

        // Act
        var actual = sut.Read(ref reader, typeof(DateOnly), JsonSerializerOptions.Default);

        // Assert
        actual.ShouldBe(new DateOnly(year, month, day));
    }

    /// <summary>
    /// <see cref="DateOnlyConverter.Write"/>は、<see cref="DateOnly"/>からJSON文字列に変換できる。
    /// </summary>
    /// <param name="year"><inheritdoc cref="DateTime.DateTime(int, int, int)" path="/param[@name='year']"/></param>
    /// <param name="month"><inheritdoc cref="DateTime.DateTime(int, int, int)" path="/param[@name='month']"/></param>
    /// <param name="day"><inheritdoc cref="DateTime.DateTime(int, int, int)" path="/param[@name='day']"/></param>
    /// <param name="json">JSON文字列</param>
    [TestMethod(DisplayName = $"{nameof(DateOnlyConverter)} > {nameof(DateOnlyConverter.Write)}()"), TestCategory("JSON Converter")]
    [DataRow(2021, 1, 1, /*lang=json,strict*/ "\"2021-01-01\"")]
    [DataRow(1986, 5, 16, /*lang=json,strict*/ "\"1986-05-16\"")]
    public void Write_Flushes_JSON(int year, int month, int day, string json)
    {
        // Arrange
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        var sut = new DateOnlyConverter();

        // Act
        sut.Write(writer, new DateOnly(year, month, day), JsonSerializerOptions.Default);
        writer.Flush();

        // Assert
        buffer.WrittenSpan.ToArray().ShouldBe(Encoding.UTF8.GetBytes(json));
    }
}
