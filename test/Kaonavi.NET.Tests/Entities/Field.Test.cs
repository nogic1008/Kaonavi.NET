using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// Unit test for <see cref="Field"/>
    /// </summary>
    public class FieldTest
    {
        [Theory]
        [InlineData(
            "{\"name\":\"社員番号\",\"required\":true,\"type\":\"string\",\"max_length\":50,\"enum\":[]}",
            "社員番号", true, "string", 50, ""
        )]
        [InlineData(
            "{\"name\":\"入社日\",\"required\":false,\"type\":\"date\",\"max_length\":null,\"enum\":[]}",
            "入社日", false, "date", null, ""
        )]
        [InlineData(
            "{\"name\":\"性別\",\"required\":false,\"type\":\"enum\",\"max_length\":null,\"enum\":[\"男性\",\"女性\"]}",
            "性別", false, "enum", null, "男性,女性"
        )]
        public void Field_CanDeserializeJSON(string json, string name, bool required, string type, int? maxLength, string enums)
        {
            // Arrange - Act
            var field = JsonSerializer.Deserialize<Field>(json);

            // Assert
            field.Should().NotBeNull();
            field!.Name.Should().Be(name);
            field!.Required.Should().Be(required);
            field!.Type.Should().Be(type);
            field!.MaxLength.Should().Be(maxLength);
            string.Join(",", field.Enum).Should().Be(enums);
        }

        [Fact]
        public void CustomField_CanDeserializeJSON()
        {
            // Arrange
            const string jsonString = "{"
            + "\"id\": 100,"
            + "\"name\": \"血液型\","
            + "\"required\": false,"
            + "\"type\": \"enum\","
            + "\"max_length\": null,"
            + "\"enum\": [\"A\", \"B\", \"O\", \"AB\"]"
            + "}";

            // Act
            var customField = JsonSerializer.Deserialize<CustomField>(jsonString);

            // Assert
            customField.Should().NotBeNull();
            customField!.Id.Should().Be(100);
            customField.Name.Should().Be("血液型");
            customField.Required.Should().BeFalse();
            customField.Type.Should().Be("enum");
            customField.MaxLength.Should().BeNull();
            customField.Enum.Should().ContainInOrder("A", "B", "O", "AB");
        }
    }
}
