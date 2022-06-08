using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="EnumOption"/>の単体テスト</summary>
public class EnumOptionTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(EnumOption)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange
        const string jsonString = "{"
        + "\"sheet_name\": \"役職情報\","
        + "\"id\": 10,"
        + "\"name\": \"役職\","
        + "\"enum_option_data\": ["
        + "  { \"id\": 1, \"name\": \"社長\" },"
        + "  { \"id\": 2, \"name\": \"部長\" },"
        + "  { \"id\": 3, \"name\": \"課長\" }"
        + "]"
        + "}";

        // Act
        var enumOption = JsonSerializer.Deserialize<EnumOption>(jsonString, JsonConfig.Default);

        // Assert
        enumOption.Should().NotBeNull();
        enumOption!.SheetName.Should().Be("役職情報");
        enumOption.Id.Should().Be(10);
        enumOption.Name.Should().Be("役職");
        enumOption.EnumOptionData.Should().NotBeNullOrEmpty()
            .And.Equal(new EnumOption.Data(1, "社長"), new(2, "部長"), new(3, "課長"));
    }
}
