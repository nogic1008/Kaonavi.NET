using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="FieldLayout"/>および<see cref="CustomFieldLayout"/>の単体テスト</summary>
[Category("Entities")]
public sealed class FieldLayoutTest
{
    /// <summary>
    /// JSONから<see cref="FieldType"/>にデシリアライズできる。
    /// </summary>
    [Test($"{nameof(FieldType)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    [Arguments("\"string\"", FieldType.String, DisplayName = $"string -> {nameof(FieldType)}.{nameof(FieldType.String)} にデシリアライズできる。")]
    [Arguments("\"number\"", FieldType.Number, DisplayName = $"number -> {nameof(FieldType)}.{nameof(FieldType.Number)} にデシリアライズできる。")]
    [Arguments("\"date\"", FieldType.Date, DisplayName = $"date -> {nameof(FieldType)}.{nameof(FieldType.Date)} にデシリアライズできる。")]
    [Arguments("\"enum\"", FieldType.Enum, DisplayName = $"enum -> {nameof(FieldType)}.{nameof(FieldType.Enum)} にデシリアライズできる。")]
    [Arguments("\"calc\"", FieldType.Calc, DisplayName = $"calc -> {nameof(FieldType)}.{nameof(FieldType.Calc)} にデシリアライズできる。")]
    [Arguments("\"department\"", FieldType.Department, DisplayName = $"department -> {nameof(FieldType)}.{nameof(FieldType.Department)} にデシリアライズできる。")]
    [Arguments("\"department[]\"", FieldType.DepartmentArray, DisplayName = $"department[] -> {nameof(FieldType)}.{nameof(FieldType.DepartmentArray)} にデシリアライズできる。")]
    public async Task FieldType_Can_Deserialize_FromJSON(string json, FieldType expected)
    {
        var result = JsonSerializer.Deserialize(json, JsonContext.Default.FieldType);
        await Assert.That(result).IsEqualTo(expected);
    }

    private const string TestName = $"{nameof(FieldLayout)} > JSONからデシリアライズできる。";

    /// <summary><see cref="FieldLayout_Can_Deserialize_FromJSON"/>のテストデータ</summary>
    public static IEnumerable<TestDataRow<(string json, string name, bool required, FieldType type, int? maxLength, string?[] enums, bool readOnly)>> TestData
    {
        get
        {
            yield return new((/*lang=json,strict*/ """
            {
              "name": "社員番号",
              "required": true,
              "type": "string",
              "max_length": 50,
              "enum": []
            }
            """, "社員番号", true, FieldType.String, 50, [], false)) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
            {
              "name": "入社日",
              "required": false,
              "type": "date",
              "max_length": null,
              "enum": []
            }
            """, "入社日", false, FieldType.Date, null, [], false)) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
            {
              "name": "性別",
              "required": false,
              "type": "enum",
              "max_length": null,
              "enum": ["男性", "女性"]
            }
            """, "性別", false, FieldType.Enum, null, ["男性", "女性"], false)) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
            {
              "name": "勤続年数",
              "required": false,
              "type": "calc",
              "max_length": null,
              "enum": [],
              "read_only": true
            }
            """, "勤続年数", false, FieldType.Calc, null, [], true)) { DisplayName = TestName };
        }
    }

    /// <summary>
    /// JSONから<see cref="FieldLayout"/>にデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="name"><inheritdoc cref="FieldLayout.Name" path="/summary"/></param>
    /// <param name="required"><inheritdoc cref="FieldLayout.Required" path="/summary"/></param>
    /// <param name="type"><inheritdoc cref="FieldLayout.Type" path="/summary"/></param>
    /// <param name="maxLength"><inheritdoc cref="FieldLayout.MaxLength" path="/summary"/></param>
    /// <param name="enums"><inheritdoc cref="FieldLayout.Enum" path="/summary"/></param>
    /// <param name="readOnly"><inheritdoc cref="FieldLayout.ReadOnly" path="/summary"/></param>
    [Test(TestName)]
    [Category("JSON Deserialize")]
    [MethodDataSource(nameof(TestData))]
    public async Task FieldLayout_Can_Deserialize_FromJSON(string json, string name, bool required, FieldType type, int? maxLength, string?[] enums, bool readOnly)
    {
        // Arrange - Act
        var fieldLayout = JsonSerializer.Deserialize(json, JsonContext.Default.FieldLayout);

        // Assert
        await Assert.That(fieldLayout).IsNotNull()
            .And.Member(sut => sut.Name, o => o.IsEqualTo<string>(name))
            .And.Member(sut => sut.Required, o => o.IsEqualTo(required))
            .And.Member(sut => sut.Type, o => o.IsEqualTo(type))
            .And.Member(sut => sut.MaxLength, o => o.IsEqualTo(maxLength))
            .And.Member(sut => sut.Enum, o => o.IsEquivalentTo(enums))
            .And.Member(sut => sut.ReadOnly, o => o.IsEqualTo(readOnly));
    }

    /// <summary>
    /// JSONから<see cref="FieldInput"/>にデシリアライズできる。
    /// </summary>
    [Test($"{nameof(FieldInput)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    [Arguments("\"text_box\"", FieldInput.TextBox, DisplayName = $"text_box -> {nameof(FieldInput)}.{nameof(FieldInput.TextBox)} にデシリアライズできる。")]
    [Arguments("\"text_area\"", FieldInput.TextArea, DisplayName = $"text_area -> {nameof(FieldInput)}.{nameof(FieldInput.TextArea)} にデシリアライズできる。")]
    [Arguments("\"number_box\"", FieldInput.NumberBox, DisplayName = $"number_box -> {nameof(FieldInput)}.{nameof(FieldInput.NumberBox)} にデシリアライズできる。")]
    [Arguments("\"pull_down\"", FieldInput.PullDown, DisplayName = $"pull_down -> {nameof(FieldInput)}.{nameof(FieldInput.PullDown)} にデシリアライズできる。")]
    [Arguments("\"radio_button\"", FieldInput.RadioButton, DisplayName = $"radio_button -> {nameof(FieldInput)}.{nameof(FieldInput.RadioButton)} にデシリアライズできる。")]
    [Arguments("\"check_box\"", FieldInput.CheckBox, DisplayName = $"check_box -> {nameof(FieldInput)}.{nameof(FieldInput.CheckBox)} にデシリアライズできる。")]
    [Arguments("\"link\"", FieldInput.Link, DisplayName = $"link -> {nameof(FieldInput)}.{nameof(FieldInput.Link)} にデシリアライズできる。")]
    [Arguments("\"date\"", FieldInput.Date, DisplayName = $"date -> {nameof(FieldInput)}.{nameof(FieldInput.Date)} にデシリアライズできる。")]
    [Arguments("\"year_month\"", FieldInput.YearMonth, DisplayName = $"year_month -> {nameof(FieldInput)}.{nameof(FieldInput.YearMonth)} にデシリアライズできる。")]
    [Arguments("\"attach_file\"", FieldInput.AttachFile, DisplayName = $"attach_file -> {nameof(FieldInput)}.{nameof(FieldInput.AttachFile)} にデシリアライズできる。")]
    [Arguments("\"face_image\"", FieldInput.FaceImage, DisplayName = $"face_image -> {nameof(FieldInput)}.{nameof(FieldInput.FaceImage)} にデシリアライズできる。")]
    [Arguments("\"calc_formula\"", FieldInput.CalcFormula, DisplayName = $"calc_formula -> {nameof(FieldInput)}.{nameof(FieldInput.CalcFormula)} にデシリアライズできる。")]
    public async Task FieldInput_Can_Deserialize_FromJSON(string json, FieldInput expected)
    {
        var result = JsonSerializer.Deserialize(json, JsonContext.Default.FieldInput);
        await Assert.That(result).IsEqualTo(expected);
    }

    /// <summary>
    /// JSONから<see cref="CustomFieldLayout"/>にデシリアライズできる。
    /// </summary>
    [Test($"{nameof(CustomFieldLayout)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task CustomFieldLayout_Can_Deserialize_FromJSON()
    {
        // Arrange
        /*lang=json,strict*/
        var json = """
        {
          "id": 100,
          "name": "血液型",
          "required": false,
          "type": "enum",
          "max_length": null,
          "enum": ["A", "B", "O", "AB"],
          "type_detail": "text_box"
        }
        """u8;

        // Act
        var customFieldLayout = JsonSerializer.Deserialize(json, JsonContext.Default.CustomFieldLayout);

        // Assert
        await Assert.That(customFieldLayout).IsNotNull()
            .And.Member(static o => o.Id, static o => o.IsEqualTo(100))
            .And.Member(static o => o.Name, static o => o.IsEqualTo<string>("血液型"))
            .And.Member(static o => o.Required, static o => o.IsFalse())
            .And.Member(static o => o.Type, static o => o.IsEqualTo(FieldType.Enum))
            .And.Member(static o => o.MaxLength, static o => o.IsNull())
            .And.Member(static o => o.Enum, static o => o.IsEquivalentTo((string?[])["A", "B", "O", "AB"]))
            .And.Member(static o => o.ReadOnly, static o => o.IsFalse())
            .And.Member(static o => o.TypeDetail, static o => o.IsEqualTo(FieldInput.TextBox));
    }
}
