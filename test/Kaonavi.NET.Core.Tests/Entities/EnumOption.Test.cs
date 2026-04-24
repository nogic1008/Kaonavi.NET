using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="EnumOption"/>の単体テスト</summary>
[Category("Entities")]
public sealed class EnumOptionTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [Test($"{nameof(EnumOption)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task CanDeserializeJSON()
    {
        // Arrange
        /*lang=json,strict*/
        var json = """
        {
          "sheet_name": "役職情報",
          "id": 10,
          "name": "役職",
          "enum_option_data": [
            { "id": 1, "name": "社長" },
            { "id": 2, "name": "部長" },
            { "id": 3, "name": "課長" }
          ]
        }
        """u8;

        // Act
        var enumOption = JsonSerializer.Deserialize(json, JsonContext.Default.EnumOption);

        // Assert
        await Assert.That(enumOption).IsNotNull()
            .And.Member(static o => o.SheetName, static o => o.IsEqualTo<string>("役職情報"))
            .And.Member(static o => o.Id, static o => o.IsEqualTo(10))
            .And.Member(static o => o.Name, static o => o.IsEqualTo<string>("役職"))
            .And.Member(static o => o.EnumOptionData, static o => o.IsEquivalentTo((EnumOption.Data[])[new(1, "社長"), new(2, "部長"), new(3, "課長")]));
    }
}
