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
    [Test, Category("JSON Deserialize")]
    [DisplayName(
        $"{nameof(FieldType)} > $json から {nameof(FieldType)}.$expected にデシリアライズできる。"
    )]
    [Arguments("\"string\"", FieldType.String)]
    [Arguments("\"number\"", FieldType.Number)]
    [Arguments("\"date\"", FieldType.Date)]
    [Arguments("\"enum\"", FieldType.Enum)]
    [Arguments("\"calc\"", FieldType.Calc)]
    [Arguments("\"department\"", FieldType.Department)]
    [Arguments("\"department[]\"", FieldType.DepartmentArray)]
    public async Task FieldType_Can_Deserialize_FromJSON(string json, FieldType expected) =>
        await Assert
            .That(JsonSerializer.Deserialize(json, JsonContext.Default.FieldType))
            .IsEqualTo(expected);

    /// <summary><see cref="FieldLayout_Can_Deserialize_FromJSON"/>のテストデータ</summary>
    public static IEnumerable<
        TestDataRow<(
            string json,
            string name,
            bool required,
            FieldType type,
            int? maxLength,
            string?[] enums,
            bool readOnly
        )>
    > TestData
    {
        get
        {
            yield return new(
                ( /*lang=json,strict*/
                    """
                    {
                      "name": "社員番号",
                      "required": true,
                      "type": "string",
                      "max_length": 50,
                      "enum": []
                    }
                    """,
                    "社員番号",
                    true,
                    FieldType.String,
                    50,
                    [],
                    false
                )
            );
            yield return new(
                ( /*lang=json,strict*/
                    """
                    {
                      "name": "入社日",
                      "required": false,
                      "type": "date",
                      "max_length": null,
                      "enum": []
                    }
                    """,
                    "入社日",
                    false,
                    FieldType.Date,
                    null,
                    [],
                    false
                )
            );
            yield return new(
                ( /*lang=json,strict*/
                    """
                    {
                      "name": "性別",
                      "required": false,
                      "type": "enum",
                      "max_length": null,
                      "enum": ["男性", "女性"]
                    }
                    """,
                    "性別",
                    false,
                    FieldType.Enum,
                    null,
                    ["男性", "女性"],
                    false
                )
            );
            yield return new(
                ( /*lang=json,strict*/
                    """
                    {
                      "name": "勤続年数",
                      "required": false,
                      "type": "calc",
                      "max_length": null,
                      "enum": [],
                      "read_only": true
                    }
                    """,
                    "勤続年数",
                    false,
                    FieldType.Calc,
                    null,
                    [],
                    true
                )
            );
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
    [Test, Category("JSON Deserialize")]
    [DisplayName($"{nameof(FieldLayout)} > $json からデシリアライズできる。")]
    [MethodDataSource(nameof(TestData))]
    public async Task FieldLayout_Can_Deserialize_FromJSON(
        string json,
        string name,
        bool required,
        FieldType type,
        int? maxLength,
        string?[] enums,
        bool readOnly
    )
    {
        // Arrange - Act
        var fieldLayout = JsonSerializer.Deserialize(json, JsonContext.Default.FieldLayout);

        // Assert
        await Assert
            .That(fieldLayout)
            .IsNotNull()
            .And.Member(sut => sut.Name, o => o.IsEqualTo<string>(name))
            .And.Member(sut => sut.Required, o => o.IsEqualTo(required))
            .And.Member(sut => sut.Type, o => o.IsEqualTo(type))
            .And.Member(sut => sut.MaxLength, o => o.IsEqualTo(maxLength))
            .And.Member(sut => sut.Enum, o => o.IsSequenceEqualTo(enums))
            .And.Member(sut => sut.ReadOnly, o => o.IsEqualTo(readOnly));
    }

    /// <summary>
    /// JSONから<see cref="FieldInput"/>にデシリアライズできる。
    /// </summary>
    [Test, Category("JSON Deserialize")]
    [DisplayName(
        $"{nameof(FieldInput)} > $json から {nameof(FieldInput)}.$expected にデシリアライズできる。"
    )]
    [Arguments("\"text_box\"", FieldInput.TextBox)]
    [Arguments("\"text_area\"", FieldInput.TextArea)]
    [Arguments("\"number_box\"", FieldInput.NumberBox)]
    [Arguments("\"pull_down\"", FieldInput.PullDown)]
    [Arguments("\"radio_button\"", FieldInput.RadioButton)]
    [Arguments("\"check_box\"", FieldInput.CheckBox)]
    [Arguments("\"link\"", FieldInput.Link)]
    [Arguments("\"date\"", FieldInput.Date)]
    [Arguments("\"year_month\"", FieldInput.YearMonth)]
    [Arguments("\"attach_file\"", FieldInput.AttachFile)]
    [Arguments("\"face_image\"", FieldInput.FaceImage)]
    [Arguments("\"calc_formula\"", FieldInput.CalcFormula)]
    public async Task FieldInput_Can_Deserialize_FromJSON(string json, FieldInput expected)
    {
        var result = JsonSerializer.Deserialize(json, JsonContext.Default.FieldInput);
        await Assert.That(result).IsEqualTo(expected);
    }

    /// <summary>
    /// JSONから<see cref="CustomFieldLayout"/>にデシリアライズできる。
    /// </summary>
    [Test, Category("JSON Deserialize")]
    [DisplayName($"{nameof(CustomFieldLayout)} > JSONからデシリアライズできる。")]
    public async Task CustomFieldLayout_Can_Deserialize_FromJSON()
    {
        // Arrange
        /*lang=json,strict*/
        var json =
            """
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
        var customFieldLayout = JsonSerializer.Deserialize(
            json,
            JsonContext.Default.CustomFieldLayout
        );

        // Assert
        await Assert
            .That(customFieldLayout)
            .IsNotNull()
            .And.Member(static o => o.Id, static o => o.IsEqualTo(100))
            .And.Member(static o => o.Name, static o => o.IsEqualTo<string>("血液型"))
            .And.Member(static o => o.Required, static o => o.IsFalse())
            .And.Member(static o => o.Type, static o => o.IsEqualTo(FieldType.Enum))
            .And.Member(static o => o.MaxLength, static o => o.IsNull())
            .And.Member(
                static o => o.Enum,
                static o => o.IsSequenceEqualTo((string?[])["A", "B", "O", "AB"])
            )
            .And.Member(static o => o.ReadOnly, static o => o.IsFalse())
            .And.Member(static o => o.TypeDetail, static o => o.IsEqualTo(FieldInput.TextBox));
    }
}
