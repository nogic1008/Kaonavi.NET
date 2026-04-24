using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="MemberLayout"/>の単体テスト</summary>
[Category("Entities")]
public sealed class MemberLayoutTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [Test($"{nameof(MemberLayout)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task CanDeserializeJSON()
    {
        // Arrange
        /*lang=json,strict*/
        var json = """
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
        """u8;

        // Act
        var memberLayout = JsonSerializer.Deserialize(json, JsonContext.Default.MemberLayout);

        // Assert
        await Assert.That(memberLayout).IsNotNull()
            .And.Member(static o => o.Code.Name, static o => o.IsEqualTo<string>("社員番号"))
            .And.Member(static o => o.Name.Required, static o => o.IsFalse())
            .And.Member(static o => o.NameKana.Type, static o => o.IsEqualTo(FieldType.String))
            .And.Member(static o => o.Mail.MaxLength, static o => o.IsEqualTo(100))
            .And.Member(static o => o.EnteredDate.Type, static o => o.IsEqualTo(FieldType.Date))
            .And.Member(static o => o.RetiredDate.Enum, static o => o.IsEmpty())
            .And.Member(static o => o.Gender.Enum, static o => o.IsEquivalentTo((string?[])["男性", "女性"]))
            .And.Member(static o => o.Birthday.MaxLength, static o => o.IsNull())
            .And.Member(static o => o.Department.Type, static o => o.IsEqualTo(FieldType.Department))
            .And.Member(static o => o.SubDepartments.Type, static o => o.IsEqualTo(FieldType.DepartmentArray))
            .And.Member(static o => o.CustomFields, static o => o.Count().IsEqualTo(2).And.Contains(l => l.Enum.SequenceEqual(["部長", "課長", "マネージャー", null])));
    }
}
