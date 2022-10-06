using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="SheetData"/>の単体テスト
/// </summary>
public class SheetDataTest
{
    /// <summary>
    /// <see cref="RecordType.Single"/>のJSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SheetData)} > 単一レコードのJSONからデシリアライズできる。")]
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
        #endregion JSON

        // Act
        var sheetData = JsonSerializer.Deserialize<SheetData>(jsonString, JsonConfig.Default);

        // Assert
        _ = sheetData.Should().NotBeNull();
        _ = sheetData!.Code.Should().Be("A0002");
        _ = sheetData.Records.Should().ContainSingle()
            .Which.Should().AllBeAssignableTo<CustomFieldValue>();
    }

    /// <summary>
    /// <see cref="RecordType.Multiple"/>のJSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SheetData)} > 複数レコードのJSONからデシリアライズできる。")]
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
        #endregion JSON

        // Act
        var sheetData = JsonSerializer.Deserialize<SheetData>(jsonString, JsonConfig.Default);

        // Assert
        _ = sheetData.Should().NotBeNull();
        _ = sheetData!.Code.Should().Be("A0001");
        _ = sheetData.Records.Should().HaveCount(2)
            .And.AllBeAssignableTo<IReadOnlyCollection<CustomFieldValue>>();
    }
}
