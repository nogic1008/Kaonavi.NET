namespace Kaonavi.Net.Tests.Entities;

using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

/// <summary>
/// <see cref="SheetLayout"/>の単体テスト
/// </summary>
public class SheetLayoutTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SheetLayout)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        #region JSON
        const string jsonString = "{"
        + "\"id\": 12,"
        + "\"name\": \"住所・連絡先\","
        + "\"record_type\": 1,"
        + "\"custom_fields\": ["
        + "  {"
        + "    \"id\": 1000,"
        + "    \"name\": \"住所\","
        + "    \"required\": false,"
        + "    \"type\": \"string\","
        + "    \"max_length\": 250,"
        + "    \"enum\": []"
        + "  },"
        + "  {"
        + "    \"id\": 1001,"
        + "    \"name\": \"電話番号\","
        + "    \"required\": false,"
        + "    \"type\": \"string\","
        + "    \"max_length\": 50,"
        + "    \"enum\": []"
        + "  }"
        + "]"
        + "}";
        #endregion

        // Act
        var layout = JsonSerializer.Deserialize<SheetLayout>(jsonString);

        // Assert
        layout.Should().NotBeNull();
        layout!.Id.Should().Be(12);
        layout.Name.Should().Be("住所・連絡先");
        layout.RecordType.Should().Be(RecordType.Multiple);
        layout.CustomFields.Should().HaveCount(2)
            .And.AllBeAssignableTo<CustomFieldLayout>();
    }
}
