using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Entities.Api;
using Xunit;

namespace Kaonavi.Net.Tests.Entities.Api
{
    /// <summary>
    /// <see cref="MemberLayout"/>の単体テスト
    /// </summary>
    public class MemberLayoutTest
    {
        /// <summary>
        /// JSONからデシリアライズできる。
        /// </summary>
        [Fact(DisplayName = nameof(MemberLayout) + " > JSONからデシリアライズできる。")]
        public void CanDeserializeJSON()
        {
            // Arrange
            #region JSON
            const string jsonString = "{"
            + "\"code\": {"
            + "  \"name\": \"社員番号\","
            + "  \"required\": true,"
            + "  \"type\": \"string\","
            + "  \"max_length\": 50,"
            + "  \"enum\": []"
            + "},"
            + "\"name\": {"
            + "  \"name\": \"氏名\","
            + "  \"required\": false,"
            + "  \"type\": \"string\","
            + "  \"max_length\": 100,"
            + "  \"enum\": []"
            + "},"
            + "\"name_kana\": {"
            + "  \"name\": \"フリガナ\","
            + "  \"required\": false,"
            + "  \"type\": \"string\","
            + "  \"max_length\": 100,"
            + "  \"enum\": []"
            + "},"
            + "\"mail\": {"
            + "  \"name\": \"メールアドレス\","
            + "  \"required\": false,"
            + "  \"type\": \"string\","
            + "  \"max_length\": 100,"
            + "  \"enum\": []"
            + "},"
            + "\"entered_date\": {"
            + "  \"name\": \"入社日\","
            + "  \"required\": false,"
            + "  \"type\": \"date\","
            + "  \"max_length\": null,"
            + "  \"enum\": []"
            + "},"
            + "\"retired_date\": {"
            + "  \"name\": \"退職日\","
            + "  \"required\": false,"
            + "  \"type\": \"date\","
            + "  \"max_length\": null,"
            + "  \"enum\": []"
            + "},"
            + "\"gender\": {"
            + "  \"name\": \"性別\","
            + "  \"required\": false,"
            + "  \"type\": \"enum\","
            + "  \"max_length\": null,"
            + "  \"enum\": ["
            + "    \"男性\","
            + "    \"女性\""
            + "  ]"
            + "},"
            + "\"birthday\": {"
            + "  \"name\": \"生年月日\","
            + "  \"required\": false,"
            + "  \"type\": \"date\","
            + "  \"max_length\": null,"
            + "  \"enum\": []"
            + "},"
            + "\"department\": {"
            + "  \"name\": \"所属\","
            + "  \"required\": false,"
            + "  \"type\": \"department\","
            + "  \"max_length\": null,"
            + "  \"enum\": []"
            + "},"
            + "\"sub_departments\": {"
            + "  \"name\": \"兼務情報\","
            + "  \"required\": false,"
            + "  \"type\": \"department[]\","
            + "  \"max_length\": null,"
            + "  \"enum\": []"
            + "},"
            + "\"custom_fields\": ["
            + "  {"
            + "    \"id\": 100,"
            + "    \"name\": \"血液型\","
            + "    \"required\": false,"
            + "    \"type\": \"enum\","
            + "    \"max_length\": null,"
            + "    \"enum\": ["
            + "      \"A\","
            + "      \"B\","
            + "      \"O\","
            + "      \"AB\""
            + "    ]"
            + "  },"
            + "  {"
            + "    \"id\": 200,"
            + "    \"name\": \"役職\","
            + "    \"required\": false,"
            + "    \"type\": \"enum\","
            + "    \"max_length\": null,"
            + "    \"enum\": ["
            + "      \"部長\","
            + "      \"課長\","
            + "      \"マネージャー\","
            + "      null"
            + "    ]"
            + "  }"
            + "]"
            + "}";
            #endregion

            // Act
            var layout = JsonSerializer.Deserialize<MemberLayout>(jsonString);

            // Assert
            layout.Should().NotBeNull();
            layout!.Code.Name.Should().Be("社員番号");
            layout.Name.Required.Should().BeFalse();
            layout.NameKana.Type.Should().Be(FieldType.String);
            layout.Mail.MaxLength.Should().Be(100);
            layout.EnteredDate.Type.Should().Be(FieldType.Date);
            layout.RetiredDate.Enum.Should().BeEmpty();
            layout.Gender.Enum.Should().Equal("男性", "女性");
            layout.Birthday.MaxLength.Should().BeNull();
            layout.Department.Type.Should().Be(FieldType.Department);
            layout.SubDepartments.Type.Should().Be(FieldType.DepartmentArray);
            layout.CustomFields.Should().HaveCount(2);
            layout.CustomFields[^1].Enum.Should().Equal("部長", "課長", "マネージャー", null);
        }
    }
}
