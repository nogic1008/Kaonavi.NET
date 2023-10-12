using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="EnumOption"/>の単体テスト</summary>
public class EnumOptionTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(EnumOption)} > JSONからデシリアライズできる。")]
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
        _ = enumOption.Should().NotBeNull();
        _ = enumOption!.SheetName.Should().Be("役職情報");
        _ = enumOption.Id.Should().Be(10);
        _ = enumOption.Name.Should().Be("役職");
        _ = enumOption.EnumOptionData.Should().NotBeNullOrEmpty()
            .And.Equal(new EnumOption.Data(1, "社長"), new(2, "部長"), new(3, "課長"));
    }
}
