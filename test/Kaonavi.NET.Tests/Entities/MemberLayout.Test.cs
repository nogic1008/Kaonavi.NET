using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="MemberLayout"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class MemberLayoutTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [TestMethod($"{nameof(MemberLayout)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeJSON()
    {
        // Arrange
        /*lang=json,strict*/
        const string json = """
        {
          "code": {
            "name": "社員番号",
            "required": true,
            "type": "string",
            "max_length": 50,
            "enum": []
          },
          "name": {
            "name": "氏名",
            "required": false,
            "type": "string",
            "max_length": 100,
            "enum": []
          },
          "name_kana": {
            "name": "フリガナ",
            "required": false,
            "type": "string",
            "max_length": 100,
            "enum": []
          },
          "mail": {
            "name": "メールアドレス",
            "required": false,
            "type": "string",
            "max_length": 100,
            "enum": []
          },
          "entered_date": {
            "name": "入社日",
            "required": false,
            "type": "date",
            "max_length": null,
            "enum": []
          },
          "retired_date": {
            "name": "退職日",
            "required": false,
            "type": "date",
            "max_length": null,
            "enum": []
          },
          "gender": {
            "name": "性別",
            "required": false,
            "type": "enum",
            "max_length": null,
            "enum": ["男性", "女性"]
          },
          "birthday": {
            "name": "生年月日",
            "required": false,
            "type": "date",
            "max_length": null,
            "enum": []
          },
          "department": {
            "name": "所属",
            "required": false,
            "type": "department",
            "max_length": null,
            "enum": []
          },
          "sub_departments": {
            "name": "兼務情報",
            "required": false,
            "type": "department[]",
            "max_length": null,
            "enum": []
          },
          "custom_fields": [
            {
              "id": 100,
              "name": "血液型",
              "required": false,
              "type": "enum",
              "max_length": null,
              "enum": ["A", "B", "O", "AB"]
            },
            {
              "id": 200,
              "name": "役職",
              "required": false,
              "type": "enum",
              "max_length": null,
              "enum": ["部長", "課長", "マネージャー", null]
            }
          ]
        }
        """;

        // Act
        var memberLayout = JsonSerializer.Deserialize(json, Context.Default.MemberLayout);

        // Assert
        memberLayout.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.Code.Name.ShouldBe("社員番号"),
            static sut => sut.Name.Required.ShouldBeFalse(),
            static sut => sut.NameKana.Type.ShouldBe(FieldType.String),
            static sut => sut.Mail.MaxLength.ShouldBe(100),
            static sut => sut.EnteredDate.Type.ShouldBe(FieldType.Date),
            static sut => sut.RetiredDate.Enum.ShouldBeEmpty(),
            static sut => sut.Gender.Enum.ShouldBe(["男性", "女性"]),
            static sut => sut.Birthday.MaxLength.ShouldBeNull(),
            static sut => sut.Department.Type.ShouldBe(FieldType.Department),
            static sut => sut.SubDepartments.Type.ShouldBe(FieldType.DepartmentArray),
            static sut => sut.CustomFields.Count.ShouldBe(2),
            static sut => sut.CustomFields[^1].Enum.ShouldBe(["部長", "課長", "マネージャー", null])
        );
    }
}
