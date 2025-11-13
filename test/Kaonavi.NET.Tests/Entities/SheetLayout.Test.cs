using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="SheetLayout"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class SheetLayoutTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(SheetLayout)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeJSON()
    {
        /*lang=json,strict*/
        const string jsonString = """
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
        """;

        // Act
        var sheetLayout = JsonSerializer.Deserialize(jsonString, Context.Default.SheetLayout);

        // Assert
        sheetLayout.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.Id.ShouldBe(12),
            static sut => sut.Name.ShouldBe("住所・連絡先"),
            static sut => sut.RecordType.ShouldBe(RecordType.Multiple),
            static sut => sut.CustomFields.Count.ShouldBe(2)
        );
    }
}
