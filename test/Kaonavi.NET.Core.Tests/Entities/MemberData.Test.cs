using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="MemberData"/>の単体テスト</summary>
[Category("Entities")]
public sealed class MemberDataTest
{
    private const string TestName = $"{nameof(MemberData)} > JSONからデシリアライズできる。";

    /// <summary><see cref="CanDeserializeJSON"/>のテストデータ</summary>
    public static IEnumerable<TestDataRow<(string json, string code, string? name, string? nameKana, string? mail, string? enteredDate, string? retiredDate, string? gender, string? birthday, string departmentCode)>> TestData
    {
        get
        {
            yield return new((/*lang=json,strict*/ """
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
              "department": { "code": "1000", "name": "取締役会", "names": [] },
              "sub_departments": [],
              "custom_fields": [
                { "id":100, "name":"血液型", "values":["A"] }
              ]
            }
            """, "A0002", "カオナビ 太郎", "カオナビ タロウ", "taro@example.com", "2005/09/20", null, "男性", null, "1000"));
            yield return new((/*lang=json,strict*/ """
            {
              "code": "A0001",
              "name": "カオナビ 花子",
              "name_kana": "カオナビ ハナコ",
              "mail": "hanako@example.com",
              "entered_date": "2013-05-07",
              "retired_date": "2020-03-31",
              "gender": "女性",
              "birthday": "1986-05-16",
              "department": { "code": "2000", "name": "営業本部 第一営業部 ITグループ", "names": ["営業本部", "第一営業部", "ITグループ"] },
              "sub_departments": [
                { "code": "3000", "name": "企画部", "names": ["企画部"] },
                { "code": "4000", "name": "管理部", "names": ["管理部"] }
              ],
              "custom_fields": [
                { "id": 100, "name": "血液型", "values": ["O"] },
                { "id": 200, "name": "役職", "values": ["部長", "マネージャー"] }
              ]
            }
            """, "A0001", "カオナビ 花子", "カオナビ ハナコ", "hanako@example.com", "2013/05/07", "2020/03/31", "女性", "1986/05/16", "2000"));
        }
    }

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
    [Test(TestName)]
    [Category("JSON Deserialize")]
    [MethodDataSource(nameof(TestData))]
    public async Task CanDeserializeJSON(string json, string code, string? name, string? nameKana, string? mail, string? enteredDate, string? retiredDate, string? gender, string? birthday, string departmentCode)
    {
        // Arrange - Act
        var memberData = JsonSerializer.Deserialize(json, JsonContext.Default.MemberData);

        // Assert
        await Assert.That(memberData).IsNotNull()
            .And.Member(sut => sut.Code, o => o.IsEqualTo<string>(code))
            .And.Member(sut => sut.Name!, o => o.IsEqualTo<string>(name))
            .And.Member(sut => sut.NameKana!, o => o.IsEqualTo<string>(nameKana))
            .And.Member(sut => sut.Mail!, o => o.IsEqualTo<string>(mail))
            .And.Member(sut => sut.EnteredDate, o => o.IsEqualTo(ParseDateOrNull(enteredDate)))
            .And.Member(sut => sut.RetiredDate, o => o.IsEqualTo(ParseDateOrNull(retiredDate)))
            .And.Member(sut => sut.Gender!, o => o.IsEqualTo<string>(gender))
            .And.Member(sut => sut.Birthday, o => o.IsEqualTo(ParseDateOrNull(birthday)))
            .And.Member(static sut => sut.SubDepartments!, static o => o.IsNotNull())
            .And.Member(static sut => sut.CustomFields!, static o => o.IsNotNull())
            .And.Member(sut => sut.Department!, o => o.IsNotNull())
            .And.Member(sut => sut.Department!.Code, o => o.IsEqualTo<string>(departmentCode));

        static DateOnly? ParseDateOrNull(string? value) => DateOnly.TryParseExact(value, "yyyy/MM/dd", out var date) ? date : null;
    }
}
