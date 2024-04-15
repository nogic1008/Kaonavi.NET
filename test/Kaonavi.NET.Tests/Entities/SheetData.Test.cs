using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="SheetData"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class SheetDataTest
{
    /// <summary>
    /// <see cref="RecordType.Single"/>のJSONからデシリアライズできる。
    /// </summary>
    [TestMethod($"{nameof(SheetData)} > 単一レコードのJSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeSingleJSON()
    {
        // Arrange
        /*lang=json,strict*/
        const string jsonString = """
        {
            "code": "A0002",
            "records": [
                {
                    "custom_fields": [
                        {
                            "id": 1000,
                            "name": "住所",
                            "values": ["東京都港区x-x-x"]
                        }
                    ]
                }
            ]
        }
        """;

        // Act
        var sheetData = JsonSerializer.Deserialize(jsonString, Context.Default.SheetData);

        // Assert
        _ = sheetData.Should().NotBeNull();
        _ = sheetData!.Code.Should().Be("A0002");
        _ = sheetData.Records.Should().ContainSingle()
            .Which.Should().AllBeAssignableTo<CustomFieldValue>();
    }

    /// <summary>
    /// <see cref="RecordType.Multiple"/>のJSONからデシリアライズできる。
    /// </summary>
    [TestMethod($"{nameof(SheetData)} > 複数レコードのJSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeMultipleJSON()
    {
        // Arrange
        /*lang=json,strict*/
        const string jsonString = """
        {
            "code": "A0001",
            "records": [
                {
                    "custom_fields": [
                        {
                            "id": 1000,
                            "name": "住所",
                            "values": ["大阪府大阪市y番y号"]
                        },
                        {
                            "id": 1001,
                            "name": "電話番号",
                            "values": ["06-yyyy-yyyy"]
                        }
                    ]
                },
                {
                    "custom_fields": [
                        {
                            "id": 1000,
                            "name": "住所",
                            "values": ["愛知県名古屋市z丁目z番z号"]
                        },
                        {
                            "id": 1001,
                            "name": "電話番号",
                            "values": ["052-zzzz-zzzz"]
                        }
                    ]
                }
            ]
        }
        """;

        // Act
        var sheetData = JsonSerializer.Deserialize(jsonString, Context.Default.SheetData);

        // Assert
        _ = sheetData.Should().NotBeNull();
        _ = sheetData!.Code.Should().Be("A0001");
        _ = sheetData.Records.Should().HaveCount(2)
            .And.AllBeAssignableTo<IReadOnlyList<CustomFieldValue>>();
    }
}
