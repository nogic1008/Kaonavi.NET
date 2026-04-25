using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="SheetLayout"/>の単体テスト</summary>
[Category("Entities")]
public sealed class SheetLayoutTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [Test($"{nameof(SheetLayout)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task CanDeserializeJSON()
    {
        /*lang=json,strict*/
        var json = """
        {
          "id": 12,
          "name": "住所・連絡先",
          "record_type": 1,
          "custom_fields": [
            {
              "id": 1000,
              "name": "住所",
              "required": false,
              "type": "string",
              "max_length": 250,
              "enum": []
            },
            {
              "id": 1001,
              "name": "電話番号",
              "required": false,
              "type": "string",
              "max_length": 50,
              "enum": []
            }
          ]
        }
        """u8;

        // Act
        var sheetLayout = JsonSerializer.Deserialize(json, JsonContext.Default.SheetLayout);

        // Assert
        await Assert.That(sheetLayout).IsNotNull()
            .And.Member(static o => o.Id, o => o.IsEqualTo(12))
            .And.Member(static o => o.Name, o => o.IsEqualTo<string>("住所・連絡先"))
            .And.Member(static o => o.RecordType, o => o.IsEqualTo(RecordType.Multiple))
            .And.Member(static o => o.CustomFields.Count, o => o.IsEqualTo(2));
    }
}
