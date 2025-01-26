using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="MemberData"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class MemberDataTest
{
    /*lang=json,strict*/
    private const string SingleJson = """
    {
      "code": "A0002",
      "name": "カオナビ 太郎",
      "name_kana": "カオナビ タロウ",
      "mail": "taro@example.com",
      "entered_date": "2005-09-20",
      "retired_date": "",
      "gender": "男性",
      "birthday": null,
      "age": 36,
      "years_of_service": "15年5ヵ月",
      "department": {
        "code": "1000",
        "name": "取締役会",
        "names": []
      },
      "sub_departments": [],
      "custom_fields": [
        {
          "id":100,
          "name":"血液型",
          "values":["A"]
        }
      ]
    }
    """;
    /*lang=json,strict*/
    private const string MultipleJson = """
    {
      "code": "A0001",
      "name": "カオナビ 花子",
      "name_kana": "カオナビ ハナコ",
      "mail": "hanako@example.com",
      "entered_date": "2013-05-07",
      "retired_date": "2020-03-31",
      "gender": "女性",
      "birthday": "1986-05-16",
      "department": {
        "code": "2000",
        "name": "営業本部 第一営業部 ITグループ",
        "names": [
          "営業本部",
          "第一営業部",
          "ITグループ"
        ]
      },
      "sub_departments": [
        {
          "code": "3000",
          "name": "企画部",
          "names": ["企画部"]
        },
        {
          "code": "4000",
          "name": "管理部",
          "names": ["管理部"]
        }
      ],
      "custom_fields": [
        {
          "id": 100,
          "name": "血液型",
          "values": ["O"]
        },
        {
          "id": 200,
          "name": "役職",
          "values": ["部長", "マネージャー"]
        }
      ]
    }
    """;

    private const string TestName = $"{nameof(MemberData)} > JSONからデシリアライズできる。";

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="code"><inheritdoc cref="MemberData.Code" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="MemberData.Name" path="/summary"/></param>
    /// <param name="nameKana"><inheritdoc cref="MemberData.NameKana" path="/summary"/></param>
    /// <param name="mail"><inheritdoc cref="MemberData.Mail" path="/summary"/></param>
    /// <param name="enteredDate"><see cref="MemberData.EnteredDate"/>の文字列表現</param>
    /// <param name="retiredDate"><see cref="MemberData.RetiredDate"/>の文字列表現</param>
    /// <param name="gender"><inheritdoc cref="MemberData.Gender" path="/summary"/></param>
    /// <param name="birthday"><see cref="MemberData.Birthday"/>の文字列表現</param>
    /// <param name="departmentCode"><inheritdoc cref="MemberDepartment.Code" path="/summary"/></param>
    [TestMethod(TestName), TestCategory("JSON Deserialize")]
    [DataRow(SingleJson, "A0002", "カオナビ 太郎", "カオナビ タロウ", "taro@example.com", "2005/09/20", null, "男性", null, "1000", DisplayName = TestName)]
    [DataRow(MultipleJson, "A0001", "カオナビ 花子", "カオナビ ハナコ", "hanako@example.com", "2013/05/07", "2020/03/31", "女性", "1986/05/16", "2000", DisplayName = TestName)]
    public void CanDeserializeJSON(string json, string code, string? name, string? nameKana, string? mail, string? enteredDate, string? retiredDate, string? gender, string? birthday, string departmentCode)
    {
        // Arrange - Act
        var memberData = JsonSerializer.Deserialize(json, Context.Default.MemberData);

        // Assert
        memberData!.ShouldSatisfyAllConditions(
            static sut => sut.ShouldNotBeNull(),
            sut => sut.Code.ShouldBe(code),
            sut => sut.Name.ShouldBe(name),
            sut => sut.NameKana.ShouldBe(nameKana),
            sut => sut.Mail.ShouldBe(mail),
            sut => sut.EnteredDate.ShouldBe(ParseDateOrNull(enteredDate)),
            sut => sut.RetiredDate.ShouldBe(ParseDateOrNull(retiredDate)),
            sut => sut.Gender.ShouldBe(gender),
            sut => sut.Birthday.ShouldBe(ParseDateOrNull(birthday)),
            static sut => sut.Department.ShouldNotBeNull(),
            sut => sut.Department!.Code.ShouldBe(departmentCode),
            static sut => sut.SubDepartments!.ShouldNotBeNull(),
            static sut => sut.CustomFields!.ShouldNotBeNull()
        );

        static DateOnly? ParseDateOrNull(string? value) => DateOnly.TryParseExact(value, "yyyy/MM/dd", out var date) ? date : null;
    }
}
