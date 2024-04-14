using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="FieldType"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public class FieldTypeTest
{
    /// <summary>JSONにシリアライズできる。</summary>
    /// <param name="fieldType">シリアライズ対象となる<see cref="FieldType"/></param>
    /// <param name="expectedJson">JSON文字列</param>
    [TestMethod($"{nameof(FieldType)} > JSONにシリアライズできる。"), TestCategory("JSON Serialize")]
    [DataRow(FieldType.String, "\"string\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.String)} > \"string\" にシリアライズされる。")]
    [DataRow(FieldType.Number, "\"number\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.Number)} > \"number\" にシリアライズされる。")]
    [DataRow(FieldType.Date, "\"date\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.Date)} > \"date\" にシリアライズされる。")]
    [DataRow(FieldType.Enum, "\"enum\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.Enum)} > \"enum\" にシリアライズされる。")]
    [DataRow(FieldType.Department, "\"department\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.Department)} > \"department\" にシリアライズされる。")]
    [DataRow(FieldType.DepartmentArray, "\"department[]\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.DepartmentArray)} > \"department[]\" にシリアライズされる。")]
    public void CanSerializeJSON(FieldType fieldType, string expectedJson)
        => JsonSerializer.Serialize(fieldType).Should().Be(expectedJson);

    /// <summary>無効な<see cref="FieldType"/>の場合、<see cref="JsonException"/>をスローする。 </summary>
    /// <param name="invalidValue"><see cref="FieldType"/>の範囲外にあたる値</param>
    [TestMethod($"{nameof(FieldType)} > 無効な値のとき、 ${nameof(JsonException)}をスローする。"), TestCategory("JSON Serialize")]
    [DataRow(-1, DisplayName = $"({nameof(FieldType)})-1 > ${nameof(JsonException)}をスローする。")]
    [DataRow(6, DisplayName = $"({nameof(FieldType)})6 > ${nameof(JsonException)}をスローする。")]
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
    [TestMethod($"{nameof(FieldType)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    [DataRow("\"string\"", FieldType.String, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.String)} にデシリアライズされる。")]
    [DataRow("\"number\"", FieldType.Number, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.Number)} にデシリアライズされる。")]
    [DataRow("\"date\"", FieldType.Date, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.Date)} にデシリアライズされる。")]
    [DataRow("\"enum\"", FieldType.Enum, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.Enum)} にデシリアライズされる。")]
    [DataRow("\"department\"", FieldType.Department, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.Department)} にデシリアライズされる。")]
    [DataRow("\"department[]\"", FieldType.DepartmentArray, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.DepartmentArray)} にデシリアライズされる。")]
    public void CanDeserializeJSON(string json, FieldType expected)
        => JsonSerializer.Deserialize(json, Context.Default.FieldType).Should().Be(expected);

    /// <summary>無効なJSONの場合、<see cref="JsonException"/>をスローする。</summary>
    /// <param name="json">JSON文字列</param>
    [TestMethod($"{nameof(FieldType)} > 無効なJSONのとき、JsonExceptionをスローする。"), TestCategory("JSON Deserialize")]
    [DataRow("\"\"", DisplayName = $"\"\" > ${nameof(JsonException)}をスローする。")]
    [DataRow("\"integer\"", DisplayName = $"\"integer\" > ${nameof(JsonException)}をスローする。")]
    [DataRow("\"string,number\"", DisplayName = $"\"string,number\" > ${nameof(JsonException)}をスローする。")]
    public void CannotDeserializeJSON_IfInvalidJson(string json)
    {
        var action = () => JsonSerializer.Deserialize(json, Context.Default.FieldType);
        _ = action.Should().ThrowExactly<JsonException>();
    }
}
