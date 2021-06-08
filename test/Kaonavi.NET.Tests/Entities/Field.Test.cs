using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// <see cref="Field"/>および<see cref="CustomField"/>の単体テスト
    /// </summary>
    public class FieldTest
    {
        #region Field
        private const string FieldJson1 = "{\"name\":\"社員番号\",\"required\":true,\"type\":\"string\",\"max_length\":50,\"enum\":[]}";
        private const string FieldJson2 = "{\"name\":\"入社日\",\"required\":false,\"type\":\"date\",\"max_length\":null,\"enum\":[]}";
        private const string FieldJson3 = "{\"name\":\"性別\",\"required\":false,\"type\":\"enum\",\"max_length\":null,\"enum\":[\"男性\",\"女性\"]}";

        /// <summary>
        /// JSONから<see cref="Field"/>にデシリアライズできる。
        /// </summary>
        /// <param name="json">JSON文字列</param>
        /// <param name="name"><see cref="Field.Name"/></param>
        /// <param name="required"><see cref="Field.Required"/></param>
        /// <param name="type"><see cref="Field.Type"/></param>
        /// <param name="maxLength"><see cref="Field.MaxLength"/></param>
        /// <param name="enums"><see cref="Field.Enum"/></param>
        [Theory(DisplayName = nameof(Field) + " > JSONからデシリアライズできる。")]
        [InlineData(FieldJson1, "社員番号", true, "string", 50)]
        [InlineData(FieldJson2, "入社日", false, "date", null)]
        [InlineData(FieldJson3, "性別", false, "enum", null, "男性", "女性")]
        public void Field_CanDeserializeJSON(string json, string name, bool required, string type, int? maxLength, params string[] enums)
        {
            // Arrange - Act
            var field = JsonSerializer.Deserialize<Field>(json);

            // Assert
            field.Should().NotBeNull();
            field!.Name.Should().Be(name);
            field.Required.Should().Be(required);
            field.Type.Should().Be(type);
            field.MaxLength.Should().Be(maxLength);
            field.Enum.Should().Equal(enums);
        }
        #endregion

        /// <summary>
        /// JSONから<see cref="CustomField"/>にデシリアライズできる。
        /// </summary>
        [Fact(DisplayName = nameof(CustomField) + " > JSONからデシリアライズできる。")]
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
            customField.Enum.Should().Equal("A", "B", "O", "AB");
        }
    }
}
