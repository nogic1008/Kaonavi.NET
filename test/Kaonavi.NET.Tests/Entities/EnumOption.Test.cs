using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="EnumOption"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class EnumOptionTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [TestMethod($"{nameof(EnumOption)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeJSON()
    {
        // Arrange
        /*lang=json,strict*/
        const string jsonString = """
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
        """;

        // Act
        var enumOption = JsonSerializer.Deserialize(jsonString, Context.Default.EnumOption);

        // Assert
        enumOption!.ShouldSatisfyAllConditions(
            static sut => sut.ShouldNotBeNull(),
            static sut => sut.SheetName.ShouldBe("役職情報"),
            static sut => sut.Id.ShouldBe(10),
            static sut => sut.Name.ShouldBe("役職"),
            static sut => sut.EnumOptionData.ShouldBe([new(1, "社長"), new(2, "部長"), new(3, "課長")])
        );
    }
}
