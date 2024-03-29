using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="FieldType"/>の単体テスト</summary>
public class FieldTypeTest
{
    /// <summary>JSONにシリアライズできる。</summary>
    /// <param name="fieldType">シリアライズ対象となる<see cref="FieldType"/></param>
    /// <param name="expectedJson">JSON文字列</param>
    [Theory(DisplayName = $"{nameof(FieldType)} > JSONにシリアライズできる。")]
    [InlineData(FieldType.String, "\"string\"")]
    [InlineData(FieldType.Number, "\"number\"")]
    [InlineData(FieldType.Date, "\"date\"")]
    [InlineData(FieldType.Enum, "\"enum\"")]
    [InlineData(FieldType.Department, "\"department\"")]
    [InlineData(FieldType.DepartmentArray, "\"department[]\"")]
    public void CanSerializeJSON(FieldType fieldType, string expectedJson)
        => JsonSerializer.Serialize(fieldType).Should().Be(expectedJson);

    /// <summary>無効な<see cref="FieldType"/>の場合、JsonExceptionをスローする。 </summary>
    /// <param name="invalidValue"><see cref="FieldType"/>の範囲外にあたる値</param>
    [Theory(DisplayName = $"{nameof(FieldType)} > 無効な値のとき、JsonExceptionをスローする。")]
    [InlineData(-1)]
    [InlineData(6)]
    public void CannotSerializeJSON_IfInvalidValue(int invalidValue)
    {
        // Arrange
        _ = Enum.IsDefined((FieldType)invalidValue).Should().BeFalse();

        // Act - Assert
        var action = () => JsonSerializer.Serialize((FieldType)invalidValue, Context.Default.FieldType);
        _ = action.Should().ThrowExactly<JsonException>();
    }

    /// <summary>JSONからデシリアライズできる。</summary>
    /// <param name="json">デシリアライズ対象となるJSON文字列</param>
    /// <param name="expected"><see cref="FieldType"/></param>
    [Theory(DisplayName = $"{nameof(FieldType)} > JSONからデシリアライズできる。")]
    [InlineData("\"string\"", FieldType.String)]
    [InlineData("\"number\"", FieldType.Number)]
    [InlineData("\"date\"", FieldType.Date)]
    [InlineData("\"enum\"", FieldType.Enum)]
    [InlineData("\"department\"", FieldType.Department)]
    [InlineData("\"department[]\"", FieldType.DepartmentArray)]
    public void CanDeserializeJSON(string json, FieldType expected)
        => JsonSerializer.Deserialize(json, Context.Default.FieldType).Should().Be(expected);

    /// <summary>無効なJSONの場合、JsonExceptionをスローする。</summary>
    /// <param name="json">JSON文字列</param>
    [Theory(DisplayName = $"{nameof(FieldType)} > 無効なJSONのとき、JsonExceptionをスローする。")]
    [InlineData("\"\"")]
    [InlineData("\"integer\"")]
    [InlineData("\"string,number\"")]
    public void CannotDeserializeJSON_IfInvalidJson(string json)
    {
        var action = () => JsonSerializer.Deserialize(json, Context.Default.FieldType);
        _ = action.Should().ThrowExactly<JsonException>();
    }
}
