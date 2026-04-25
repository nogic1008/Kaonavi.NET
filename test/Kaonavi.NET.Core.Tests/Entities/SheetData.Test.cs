using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="SheetData"/>の単体テスト</summary>
[Category("Entities")]
public sealed class SheetDataTest
{
    /// <summary>
    /// <see cref="RecordType.Single"/>のJSONからデシリアライズできる。
    /// </summary>
    [Test($"{nameof(SheetData)} > 単一レコードのJSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task CanDeserializeSingleJSON()
    {
        // Arrange
        /*lang=json,strict*/
        var json = """
        {
          "code": "A0002",
          "records": [
            {
              "custom_fields": [
                { "id": 1000, "name": "住所", "values": ["東京都港区x-x-x"] }
              ]
            }
          ]
        }
        """u8;

        // Act
        var sheetData = JsonSerializer.Deserialize(json, JsonContext.Default.SheetData);

        // Assert
        await Assert.That(sheetData).IsNotNull()
            .And.Member(static o => o.Code, o => o.IsEqualTo<string>("A0002"))
            .And.Member(static o => o.Records, o => o.HasSingleItem())
            .And.Member(static o => o.Records[0], o => o.HasSingleItem())
            .And.Member(
                static o => o.Records[0][0],
                o => o.IsNotNull()
                    .And.Member(static o => o.Id, o => o.IsEqualTo(1000))
                    .And.Member(static o => o.Name!, o => o.IsEqualTo<string>("住所"))
                    .And.Member(static o => o.Values, o => o.IsEquivalentTo(["東京都港区x-x-x"]))
            );
    }

    /// <summary>
    /// <see cref="RecordType.Multiple"/>のJSONからデシリアライズできる。
    /// </summary>
    [Test($"{nameof(SheetData)} > 複数レコードのJSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task CanDeserializeMultipleJSON()
    {
        // Arrange
        /*lang=json,strict*/
        var json = """
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
        """u8;

        // Act
        var sheetData = JsonSerializer.Deserialize(json, JsonContext.Default.SheetData);

        // Assert
        await Assert.That(sheetData).IsNotNull()
            .And.Member(static o => o.Code, o => o.IsEqualTo<string>("A0001"))
            .And.Member(static o => o.Records, o => o.Count().IsEqualTo(2))
            .And.Member(static o => o.Records[0], o => o.Count().IsEqualTo(2))
            .And.Member(
                static o => o.Records[0][0],
                o => o.IsNotNull()
                    .And.Member(static o => o.Id, o => o.IsEqualTo(1000))
                    .And.Member(static o => o.Name!, o => o.IsEqualTo<string>("住所"))
                    .And.Member(static o => o.Values, o => o.IsEquivalentTo(["大阪府大阪市y番y号"]))
            )
            .And.Member(
                static o => o.Records[0][1],
                o => o.IsNotNull()
                    .And.Member(static o => o.Id, o => o.IsEqualTo(1001))
                    .And.Member(static o => o.Name!, o => o.IsEqualTo<string>("電話番号"))
                    .And.Member(static o => o.Values, o => o.IsEquivalentTo(["06-yyyy-yyyy"]))
            )
            .And.Member(static o => o.Records[1], o => o.Count().IsEqualTo(2))
            .And.Member(
                static o => o.Records[1][0],
                o => o.IsNotNull()
                    .And.Member(static o => o.Id, o => o.IsEqualTo(1000))
                    .And.Member(static o => o.Name!, o => o.IsEqualTo<string>("住所"))
                    .And.Member(static o => o.Values, o => o.IsEquivalentTo(["愛知県名古屋市z丁目z番z号"]))
            )
            .And.Member(
                static o => o.Records[1][1],
                o => o.IsNotNull()
                    .And.Member(static o => o.Id, o => o.IsEqualTo(1001))
                    .And.Member(static o => o.Name!, o => o.IsEqualTo<string>("電話番号"))
                    .And.Member(static o => o.Values, o => o.IsEquivalentTo(["052-zzzz-zzzz"]))
            );
    }
}
