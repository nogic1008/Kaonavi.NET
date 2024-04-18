using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="FieldLayout"/>および<see cref="CustomFieldLayout"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class FieldLayoutTest
{
    /*lang=json,strict*/
    private const string StringFieldJson = """
    {
        "name": "社員番号",
        "required": true,
        "type": "string",
        "max_length": 50,
        "enum": []
    }
    """;
    /*lang=json,strict*/
    private const string DateFieldJson = """
    {
        "name": "入社日",
        "required": false,
        "type": "date",
        "max_length": null,
        "enum": []
    }
    """;
    /*lang=json,strict*/
    private const string EnumFieldJson = """
    {
        "name": "性別",
        "required": false,
        "type": "enum",
        "max_length": null,
        "enum": ["男性", "女性"]
    }
    """;
    /*lang=json,strict*/
    private const string CalcFieldJson = """
    {
        "name": "勤続年数",
        "required": false,
        "type": "calc",
        "max_length": null,
        "enum": [],
        "read_only": true
    }
    """;

    private const string TestName = $"{nameof(FieldLayout)} > JSONからデシリアライズできる。";

    /// <summary>
    /// JSONから<see cref="FieldLayout"/>にデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="name"><inheritdoc cref="FieldLayout.Name" path="/summary"/></param>
    /// <param name="required"><inheritdoc cref="FieldLayout.Required" path="/summary"/></param>
    /// <param name="type"><inheritdoc cref="FieldLayout.Type" path="/summary"/></param>
    /// <param name="maxLength"><inheritdoc cref="FieldLayout.MaxLength" path="/summary"/></param>
    /// <param name="enums"><inheritdoc cref="FieldLayout.Enum" path="/summary"/></param>
    /// <param name="readOnly"><inheritdoc cref="FieldLayout.ReadOnly path="/summary"/></param>
    [TestMethod(TestName), TestCategory("JSON Deserialize")]
    [DataRow(StringFieldJson, "社員番号", true, FieldType.String, 50, new string[] { }, false, DisplayName = TestName)]
    [DataRow(DateFieldJson, "入社日", false, FieldType.Date, null, new string[] { }, false, DisplayName = TestName)]
    [DataRow(EnumFieldJson, "性別", false, FieldType.Enum, null, new[] { "男性", "女性" }, false, DisplayName = TestName)]
    [DataRow(CalcFieldJson, "勤続年数", false, FieldType.Calc, null, new string[] { }, true, DisplayName = TestName)]
    public void Field_CanDeserializeJSON(string json, string name, bool required, FieldType type, int? maxLength, string[] enums, bool readOnly)
    {
        // Arrange - Act
        var field = JsonSerializer.Deserialize(json, Context.Default.FieldLayout);

        // Assert
        _ = field.Should().NotBeNull();
        _ = field!.Name.Should().Be(name);
        _ = field.Required.Should().Be(required);
        _ = field.Type.Should().Be(type);
        _ = field.MaxLength.Should().Be(maxLength);
        _ = field.Enum.Should().Equal(enums);
        _ = field.ReadOnly.Should().Be(readOnly);
    }

    /// <summary>
    /// JSONから<see cref="CustomFieldLayout"/>にデシリアライズできる。
    /// </summary>
    [TestMethod($"{nameof(CustomFieldLayout)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CustomField_CanDeserializeJSON()
    {
        // Arrange
        /*lang=json,strict*/
        const string jsonString = """
        {
            "id": 100,
            "name": "血液型",
            "required": false,
            "type": "enum",
            "max_length": null,
            "enum": ["A", "B", "O", "AB"]
        }
        """;

        // Act
        var customField = JsonSerializer.Deserialize(jsonString, Context.Default.CustomFieldLayout);

        // Assert
        _ = customField.Should().NotBeNull();
        _ = customField!.Id.Should().Be(100);
        _ = customField.Name.Should().Be("血液型");
        _ = customField.Required.Should().BeFalse();
        _ = customField.Type.Should().Be(FieldType.Enum);
        _ = customField.MaxLength.Should().BeNull();
        _ = customField.Enum.Should().Equal("A", "B", "O", "AB");
    }
}
