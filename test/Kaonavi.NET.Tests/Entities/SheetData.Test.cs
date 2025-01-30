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
        sheetData.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.Code.ShouldNotBeNull("A0002"),
            static sut => sut.Records.ShouldHaveSingleItem().ShouldHaveSingleItem().ShouldSatisfyAllConditions(
                static sut => sut.Id.ShouldBe(1000),
                static sut => sut.Name.ShouldBe("住所"),
                static sut => sut.Values.ShouldBe(["東京都港区x-x-x"])
            )
        );
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
        sheetData!.ShouldSatisfyAllConditions(
            static sut => sut.ShouldNotBeNull(),
            static sut => sut.Code.ShouldBe("A0001"),
            static sut => sut.Records.ShouldSatisfyAllConditions(
                static records => records.Count.ShouldBe(2),
                static records => records[0].ShouldSatisfyAllConditions(
                    static fields => fields.Count.ShouldBe(2),
                    static fields => fields[0].ShouldSatisfyAllConditions(
                        static sut => sut.Id.ShouldBe(1000),
                        static sut => sut.Name.ShouldBe("住所"),
                        static sut => sut.Values.ShouldBe(["大阪府大阪市y番y号"])
                    ),
                    static fields => fields[1].ShouldSatisfyAllConditions(
                        static sut => sut.Id.ShouldBe(1001),
                        static sut => sut.Name.ShouldBe("電話番号"),
                        static sut => sut.Values.ShouldBe(["06-yyyy-yyyy"])
                    )
                ),
                static records => records[1].ShouldSatisfyAllConditions(
                    static fields => fields.Count.ShouldBe(2),
                    static fields => fields[0].ShouldSatisfyAllConditions(
                        static sut => sut.Id.ShouldBe(1000),
                        static sut => sut.Name.ShouldBe("住所"),
                        static sut => sut.Values.ShouldBe(["愛知県名古屋市z丁目z番z号"])
                    ),
                    static fields => fields[1].ShouldSatisfyAllConditions(
                        static sut => sut.Id.ShouldBe(1001),
                        static sut => sut.Name.ShouldBe("電話番号"),
                        static sut => sut.Values.ShouldBe(["052-zzzz-zzzz"])
                    )
                )
            )
        );
    }
}
