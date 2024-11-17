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
    /// JSONから<see cref="FieldType"/>にデシリアライズできる。
    /// </summary>
    [TestMethod($"{nameof(FieldType)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    [DataRow("\"string\"", FieldType.String, DisplayName = $"\"string\" -> {nameof(FieldType)}.{nameof(FieldType.String)} にデシリアライズできる。")]
    [DataRow("\"number\"", FieldType.Number, DisplayName = $"\"number\" -> {nameof(FieldType)}.{nameof(FieldType.Number)} にデシリアライズできる。")]
    [DataRow("\"date\"", FieldType.Date, DisplayName = $"\"date\" -> {nameof(FieldType)}.{nameof(FieldType.Date)} にデシリアライズできる。")]
    [DataRow("\"enum\"", FieldType.Enum, DisplayName = $"\"enum\" -> {nameof(FieldType)}.{nameof(FieldType.Enum)} にデシリアライズできる。")]
    [DataRow("\"calc\"", FieldType.Calc, DisplayName = $"\"calc\" -> {nameof(FieldType)}.{nameof(FieldType.Calc)} にデシリアライズできる。")]
    [DataRow("\"department\"", FieldType.Department, DisplayName = $"\"department\" -> {nameof(FieldType)}.{nameof(FieldType.Department)} にデシリアライズできる。")]
    [DataRow("\"department[]\"", FieldType.DepartmentArray, DisplayName = $"\"department[]\" -> {nameof(FieldType)}.{nameof(FieldType.DepartmentArray)} にデシリアライズできる。")]
    public void FieldType_Can_Deserialize_FromJSON(string json, FieldType expected)
        => JsonSerializer.Deserialize(json, Context.Default.FieldType).Should().Be(expected);

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
    [DataRow(StringFieldJson, "社員番号", true, FieldType.String, 50, (string[])[], false, DisplayName = TestName)]
    [DataRow(DateFieldJson, "入社日", false, FieldType.Date, null, (string[])[], false, DisplayName = TestName)]
    [DataRow(EnumFieldJson, "性別", false, FieldType.Enum, null, (string[])["男性", "女性"], false, DisplayName = TestName)]
    [DataRow(CalcFieldJson, "勤続年数", false, FieldType.Calc, null, (string[])[], true, DisplayName = TestName)]
    public void FieldLayout_Can_Deserialize_FromJSON(string json, string name, bool required, FieldType type, int? maxLength, string[] enums, bool readOnly)
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
    /// JSONから<see cref="FieldInput"/>にデシリアライズできる。
    /// </summary>
    [TestMethod($"{nameof(FieldInput)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    [DataRow("\"text_box\"", FieldInput.TextBox, DisplayName = $"\"text_box\" -> {nameof(FieldInput)}.{nameof(FieldInput.TextBox)} にデシリアライズできる。")]
    [DataRow("\"text_area\"", FieldInput.TextArea, DisplayName = $"\"text_area\" -> {nameof(FieldInput)}.{nameof(FieldInput.TextArea)} にデシリアライズできる。")]
    [DataRow("\"number_box\"", FieldInput.NumberBox, DisplayName = $"\"number_box\" -> {nameof(FieldInput)}.{nameof(FieldInput.NumberBox)} にデシリアライズできる。")]
    [DataRow("\"pull_down\"", FieldInput.PullDown, DisplayName = $"\"pull_down\" -> {nameof(FieldInput)}.{nameof(FieldInput.PullDown)} にデシリアライズできる。")]
    [DataRow("\"radio_button\"", FieldInput.RadioButton, DisplayName = $"\"radio_buton\" -> {nameof(FieldInput)}.{nameof(FieldInput.RadioButton)} にデシリアライズできる。")]
    [DataRow("\"check_box\"", FieldInput.CheckBox, DisplayName = $"\"check_box\" -> {nameof(FieldInput)}.{nameof(FieldInput.CheckBox)} にデシリアライズできる。")]
    [DataRow("\"link\"", FieldInput.Link, DisplayName = $"\"link\" -> {nameof(FieldInput)}.{nameof(FieldInput.Link)} にデシリアライズできる。")]
    [DataRow("\"date\"", FieldInput.Date, DisplayName = $"\"date\" -> {nameof(FieldInput)}.{nameof(FieldInput.Date)} にデシリアライズできる。")]
    [DataRow("\"year_month\"", FieldInput.YearMonth, DisplayName = $"\"year_month\" -> {nameof(FieldInput)}.{nameof(FieldInput.YearMonth)} にデシリアライズできる。")]
    [DataRow("\"attach_file\"", FieldInput.AttachFile, DisplayName = $"\"attach_file\" -> {nameof(FieldInput)}.{nameof(FieldInput.AttachFile)} にデシリアライズできる。")]
    [DataRow("\"face_image\"", FieldInput.FaceImage, DisplayName = $"\"face_image\" -> {nameof(FieldInput)}.{nameof(FieldInput.FaceImage)} にデシリアライズできる。")]
    [DataRow("\"calc_formula\"", FieldInput.CalcFormula, DisplayName = $"\"calc_formula\" -> {nameof(FieldInput)}.{nameof(FieldInput.CalcFormula)} にデシリアライズできる。")]
    public void FieldInput_Can_Deserialize_FromJSON(string json, FieldInput expected)
        => JsonSerializer.Deserialize(json, Context.Default.FieldInput).Should().Be(expected);

    /// <summary>
    /// JSONから<see cref="CustomFieldLayout"/>にデシリアライズできる。
    /// </summary>
    [TestMethod($"{nameof(CustomFieldLayout)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CustomFieldLayout_Can_Deserialize_FromJSON()
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
            "enum": ["A", "B", "O", "AB"],
            "type_detail": "text_box"
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
        _ = customField.ReadOnly.Should().BeFalse();
        _ = customField.TypeDetail.Should().Be(FieldInput.TextBox);
    }
}
