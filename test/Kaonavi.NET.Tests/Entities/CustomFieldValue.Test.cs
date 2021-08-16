namespace Kaonavi.Net.Tests.Entities;

using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

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
    [Theory(DisplayName = nameof(CustomFieldValue) + " > JSONからデシリアライズできる。")]
    [InlineData("{\"id\":100,\"name\":\"血液型\",\"values\":[\"A\"]}", 100, "血液型", "A")]
    [InlineData("{\"id\":100,\"values\":[\"\"]}", 100, null, "")]
    [InlineData("{\"id\": 1,\"values\":[\"Aコース\",\"Bコース\"]}", 1, null, "Aコース", "Bコース")]
    public void CanDeserializeJSON(string json, int id, string? name, params string[] values)
    {
        // Arrange - Act
        var fieldValue = JsonSerializer.Deserialize<CustomFieldValue>(json);

        // Assert
        fieldValue.Should().NotBeNull();
        fieldValue!.Id.Should().Be(id);
        fieldValue.Name.Should().Be(name);
        fieldValue.Values.Should().NotBeNullOrEmpty()
            .And.Equal(values);
    }
}
