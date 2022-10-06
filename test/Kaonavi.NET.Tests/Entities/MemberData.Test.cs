using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="MemberData"/>の単体テスト
/// </summary>
public class MemberDataTest
{
    #region JSON
    private const string SampleJson1 = "{"
    + "  \"code\": \"A0002\","
    + "  \"name\": \"カオナビ 太郎\","
    + "  \"name_kana\": \"カオナビ タロウ\","
    + "  \"mail\": \"taro@example.com\","
    + "  \"entered_date\": \"2005-09-20\","
    + "  \"retired_date\": \"\","
    + "  \"gender\": \"男性\","
    + "  \"birthday\": null,"
    + "  \"age\": 36,"
    + "  \"years_of_service\": \"15年5ヵ月\","
    + "  \"department\": {"
    + "    \"code\": \"1000\","
    + "    \"name\": \"取締役会\","
    + "    \"names\": []"
    + "  },"
    + "  \"sub_departments\": [],"
    + "  \"custom_fields\": ["
    + "    {"
    + "      \"id\":100,"
    + "      \"name\":\"血液型\","
    + "      \"values\":["
    + "        \"A\""
    + "      ]"
    + "    }"
    + "  ]"
    + "}";
    private const string SampleJson2 = "{"
    + "  \"code\": \"A0001\","
    + "  \"name\": \"カオナビ 花子\","
    + "  \"name_kana\": \"カオナビ ハナコ\","
    + "  \"mail\": \"hanako@example.com\","
    + "  \"entered_date\": \"2013-05-07\","
    + "  \"retired_date\": \"2020-03-31\","
    + "  \"gender\": \"女性\","
    + "  \"birthday\": \"1986-05-16\","
    + "  \"department\": {"
    + "    \"code\": \"2000\","
    + "    \"name\": \"営業本部 第一営業部 ITグループ\","
    + "    \"names\": ["
    + "      \"営業本部\","
    + "      \"第一営業部\","
    + "      \"ITグループ\""
    + "    ]"
    + "  },"
    + "  \"sub_departments\": ["
    + "    {"
    + "      \"code\": \"3000\","
    + "      \"name\": \"企画部\","
    + "      \"names\": ["
    + "        \"企画部\""
    + "      ]"
    + "    },"
    + "    {"
    + "      \"code\": \"4000\","
    + "      \"name\": \"管理部\","
    + "      \"names\": ["
    + "        \"管理部\""
    + "      ]"
    + "    }"
    + "  ],"
    + "  \"custom_fields\": ["
    + "    {"
    + "      \"id\": 100,"
    + "      \"name\": \"血液型\","
    + "      \"values\": ["
    + "        \"O\""
    + "      ]"
    + "    },"
    + "    {"
    + "      \"id\": 200,"
    + "      \"name\": \"役職\","
    + "      \"values\": ["
    + "        \"部長\","
    + "        \"マネージャー\""
    + "      ]"
    + "    }"
    + "  ]"
    + "}";
    #endregion JSON

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="code"><see cref="MemberData.Code"/></param>
    /// <param name="name"><see cref="MemberData.Name"/></param>
    /// <param name="nameKana"><see cref="MemberData.NameKana"/></param>
    /// <param name="mail"><see cref="MemberData.Mail"/></param>
    /// <param name="enteredDate"><see cref="MemberData.EnteredDate"/>の文字列表現</param>
    /// <param name="retiredDate"><see cref="MemberData.RetiredDate"/>の文字列表現</param>
    /// <param name="gender"><see cref="MemberData.Gender"/></param>
    /// <param name="birthday"><see cref="MemberData.Birthday"/>の文字列表現</param>
    /// <param name="departmentCode"><see cref="MemberDepartment.Code"/></param>
    [Theory(DisplayName = $"{nameof(MemberData)} > JSONからデシリアライズできる。")]
    [InlineData(SampleJson1, "A0002", "カオナビ 太郎", "カオナビ タロウ", "taro@example.com", "2005/09/20", null, "男性", null, "1000")]
    [InlineData(SampleJson2, "A0001", "カオナビ 花子", "カオナビ ハナコ", "hanako@example.com", "2013/05/07", "2020/03/31", "女性", "1986/05/16", "2000")]
    public void CanDeserializeJSON(string json, string code, string? name, string? nameKana, string? mail, string? enteredDate, string? retiredDate, string? gender, string? birthday, string departmentCode)
    {
        // Arrange - Act
        var memberData = JsonSerializer.Deserialize<MemberData>(json, JsonConfig.Default);

        // Assert
        _ = memberData.Should().NotBeNull();
        _ = memberData!.Code.Should().Be(code);
        _ = memberData.Name.Should().Be(name);
        _ = memberData.NameKana.Should().Be(nameKana);
        _ = memberData.Mail.Should().Be(mail);
        _ = memberData.EnteredDate.Should().Be(DateOnly.TryParseExact(enteredDate, "yyyy/MM/dd", out var entered) ? entered : null);
        _ = memberData.RetiredDate.Should().Be(DateOnly.TryParseExact(retiredDate, "yyyy/MM/dd", out var retired) ? retired : null);
        _ = memberData.Gender.Should().Be(gender);
        _ = memberData.Birthday.Should().Be(DateOnly.TryParseExact(birthday, "yyyy/MM/dd", out var birth) ? birth : null);
        _ = memberData.Department.Should().NotBeNull();
        _ = memberData.Department!.Code.Should().Be(departmentCode);
        _ = memberData.SubDepartments.Should().AllBeAssignableTo<MemberDepartment>();
        _ = memberData.CustomFields.Should().AllBeAssignableTo<CustomFieldValue>();
    }
}
