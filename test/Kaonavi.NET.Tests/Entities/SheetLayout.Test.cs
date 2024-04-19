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
    [TestMethod($"{nameof(SheetLayout)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
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
        var layout = JsonSerializer.Deserialize(jsonString, Context.Default.SheetLayout);

        // Assert
        _ = layout.Should().NotBeNull();
        _ = layout!.Id.Should().Be(12);
        _ = layout.Name.Should().Be("住所・連絡先");
        _ = layout.RecordType.Should().Be(RecordType.Multiple);
        _ = layout.CustomFields.Should().HaveCount(2)
            .And.AllBeAssignableTo<CustomFieldLayout>();
    }
}
