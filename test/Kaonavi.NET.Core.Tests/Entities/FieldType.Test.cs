using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="FieldType"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class FieldTypeTest
{
    /// <summary>JSONにシリアライズできる。</summary>
    /// <param name="fieldType">シリアライズ対象となる<see cref="FieldType"/></param>
    /// <param name="expectedJson">JSON文字列</param>
    [TestMethod($"{nameof(FieldType)} > JSONにシリアライズできる。"), TestCategory("JSON Serialize")]
    [DataRow(FieldType.String, /*lang=json,strict*/ "\"string\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.String)} > \"string\" にシリアライズされる。")]
    [DataRow(FieldType.Number, /*lang=json,strict*/ "\"number\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.Number)} > \"number\" にシリアライズされる。")]
    [DataRow(FieldType.Date, /*lang=json,strict*/ "\"date\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.Date)} > \"date\" にシリアライズされる。")]
    [DataRow(FieldType.Enum, /*lang=json,strict*/ "\"enum\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.Enum)} > \"enum\" にシリアライズされる。")]
    [DataRow(FieldType.Calc, /*lang=json,strict*/ "\"calc\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.Calc)} > \"calc\" にシリアライズされる。")]
    [DataRow(FieldType.Department, /*lang=json,strict*/ "\"department\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.Department)} > \"department\" にシリアライズされる。")]
    [DataRow(FieldType.DepartmentArray, /*lang=json,strict*/ "\"department[]\"", DisplayName = $"{nameof(FieldType)}.${nameof(FieldType.DepartmentArray)} > \"department[]\" にシリアライズされる。")]
    public void CanSerializeJSON(FieldType fieldType, string expectedJson)
        => JsonSerializer.Serialize(fieldType).Should().Be(expectedJson);

    /// <summary>無効な<see cref="FieldType"/>の場合、<see cref="JsonException"/>をスローする。 </summary>
    /// <param name="invalidValue"><see cref="FieldType"/>の範囲外にあたる値</param>
    [TestMethod($"{nameof(FieldType)} > 無効な値のとき、 ${nameof(JsonException)}をスローする。"), TestCategory("JSON Serialize")]
    [DataRow((FieldType)(-1), DisplayName = $"({nameof(FieldType)})-1 > ${nameof(JsonException)}をスローする。")]
    [DataRow((FieldType)10, DisplayName = $"({nameof(FieldType)})10 > ${nameof(JsonException)}をスローする。")]
    public void When_FieldType_IsInvalid_Serialize_Throws_JsonException(FieldType invalidValue)
    {
        // Arrange
        _ = Enum.IsDefined(invalidValue).Should().BeFalse();

        // Act - Assert
        var action = () => JsonSerializer.Serialize((FieldType)invalidValue, Context.Default.FieldType);
        _ = action.Should().ThrowExactly<JsonException>();
    }

    /// <summary>JSONからデシリアライズできる。</summary>
    /// <param name="json">デシリアライズ対象となるJSON文字列</param>
    /// <param name="expected"><see cref="FieldType"/></param>
    [TestMethod($"{nameof(FieldType)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    [DataRow(/*lang=json,strict*/ "\"string\"", FieldType.String, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.String)} にデシリアライズされる。")]
    [DataRow(/*lang=json,strict*/ "\"number\"", FieldType.Number, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.Number)} にデシリアライズされる。")]
    [DataRow(/*lang=json,strict*/ "\"date\"", FieldType.Date, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.Date)} にデシリアライズされる。")]
    [DataRow(/*lang=json,strict*/ "\"enum\"", FieldType.Enum, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.Enum)} にデシリアライズされる。")]
    [DataRow(/*lang=json,strict*/ "\"calc\"", FieldType.Calc, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.Calc)} にデシリアライズされる。")]
    [DataRow(/*lang=json,strict*/ "\"department\"", FieldType.Department, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.Department)} にデシリアライズされる。")]
    [DataRow(/*lang=json,strict*/ "\"department[]\"", FieldType.DepartmentArray, DisplayName = $" > {nameof(FieldType)}.${nameof(FieldType.DepartmentArray)} にデシリアライズされる。")]
    public void CanDeserializeJSON(string json, FieldType expected)
        => JsonSerializer.Deserialize(json, Context.Default.FieldType).Should().Be(expected);

    /// <summary>無効なJSONの場合、<see cref="JsonException"/>をスローする。</summary>
    /// <param name="json">JSON文字列</param>
    [TestMethod($"{nameof(FieldType)} > 無効なJSONのとき、JsonExceptionをスローする。"), TestCategory("JSON Deserialize")]
    [DataRow(/*lang=json,strict*/ "\"\"", DisplayName = $"\"\" > ${nameof(JsonException)}をスローする。")]
    [DataRow(/*lang=json,strict*/ "\"integer\"", DisplayName = $"\"integer\" > ${nameof(JsonException)}をスローする。")]
    [DataRow(/*lang=json,strict*/ "\"string,number\"", DisplayName = $"\"string,number\" > ${nameof(JsonException)}をスローする。")]
    public void When_Json_IsNotFieldType_Deserialize_Throws_JsonException(string json)
    {
        var action = () => JsonSerializer.Deserialize(json, Context.Default.FieldType);
        _ = action.Should().ThrowExactly<JsonException>();
    }
}
