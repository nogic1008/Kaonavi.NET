using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="CustomFieldValue"/>の単体テスト
/// </summary>
public class CustomFieldValueTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><see cref="CustomFieldValue.Id"/></param>
    /// <param name="name"><see cref="CustomFieldValue.Name"/></param>
    /// <param name="values"><see cref="CustomFieldValue.Values"/></param>
    [Theory(DisplayName = $"{nameof(CustomFieldValue)} > JSONからデシリアライズできる。")]
    [InlineData(/*lang=json,strict*/ """{ "id": 100, "name": "血液型", "values": ["A"] }""", 100, "血液型", "A")]
    [InlineData(/*lang=json,strict*/ """{ "id": 100, "values": [""] }""", 100, null, "")]
    [InlineData(/*lang=json,strict*/ """{ "id": 1, "values": ["Aコース", "Bコース"] }""", 1, null, "Aコース", "Bコース")]
    public void CanDeserializeJSON(string json, int id, string? name, params string[] values)
    {
        // Arrange - Act
        var fieldValue = JsonSerializer.Deserialize(json, Context.Default.CustomFieldValue);

        // Assert
        _ = fieldValue.Should().NotBeNull();
        _ = fieldValue!.Id.Should().Be(id);
        _ = fieldValue.Name.Should().Be(name);
        _ = fieldValue.Value.Should().Be(values[0]);
        _ = fieldValue.Values.Should().NotBeNullOrEmpty()
            .And.Equal(values);
    }
}
