namespace Kaonavi.Net.Tests.Entities;

using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

/// <summary>
/// <see cref="SheetData"/>の単体テスト
/// </summary>
public class SheetDataTest
{
    private const string TestName = nameof(SheetData) + " > ";

    /// <summary>
    /// <see cref="RecordType.Single"/>のJSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = TestName + "単一レコードのJSONからデシリアライズできる。")]
    public void CanDeserializeSingleJSON()
    {
        // Arrange
        #region JSON
        const string jsonString = "{"
        + "\"code\": \"A0002\","
        + "\"records\": ["
        + "  {"
        + "    \"custom_fields\": ["
        + "      {"
        + "        \"id\": 1000,"
        + "        \"name\": \"住所\","
        + "        \"values\": ["
        + "          \"東京都港区x-x-x\""
        + "        ]"
        + "      }"
        + "    ]"
        + "  }"
        + "]"
        + "}";
        #endregion

        // Act
        var sheetData = JsonSerializer.Deserialize<SheetData>(jsonString);

        // Assert
        sheetData.Should().NotBeNull();
        sheetData!.Code.Should().Be("A0002");
        sheetData.Records.Should().ContainSingle()
            .Which.CustomFields.Should().AllBeAssignableTo<CustomFieldValue>();
    }

    /// <summary>
    /// <see cref="RecordType.Multiple"/>のJSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = TestName + "複数レコードのJSONからデシリアライズできる。")]
    public void CanDeserializeMultipleJSON()
    {
        // Arrange
        #region JSON
        const string jsonString = "{"
        + "\"code\": \"A0001\","
        + "\"records\": ["
        + "  {"
        + "    \"custom_fields\": ["
        + "      {"
        + "        \"id\": 1000,"
        + "        \"name\": \"住所\","
        + "        \"values\": ["
        + "          \"大阪府大阪市y番y号\""
        + "        ]"
        + "      },"
        + "      {"
        + "        \"id\": 1001,"
        + "        \"name\": \"電話番号\","
        + "        \"values\": ["
        + "          \"06-yyyy-yyyy\""
        + "        ]"
        + "      }"
        + "    ]"
        + "  },"
        + "  {"
        + "    \"custom_fields\": ["
        + "      {"
        + "        \"id\": 1000,"
        + "        \"name\": \"住所\","
        + "        \"values\": ["
        + "          \"愛知県名古屋市z丁目z番z号\""
        + "        ]"
        + "      },"
        + "      {"
        + "        \"id\": 1001,"
        + "        \"name\": \"電話番号\","
        + "        \"values\": ["
        + "          \"052-zzzz-zzzz\""
        + "        ]"
        + "      }"
        + "    ]"
        + "  }"
        + "]"
        + "}";
        #endregion

        // Act
        var sheetData = JsonSerializer.Deserialize<SheetData>(jsonString);

        // Assert
        sheetData.Should().NotBeNull();
        sheetData!.Code.Should().Be("A0001");
        sheetData.Records.Should().HaveCount(2)
            .And.AllBeAssignableTo<SheetRecord>();
    }
}
