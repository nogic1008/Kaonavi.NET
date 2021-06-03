using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// Unit test for <see cref="CustomFieldValue"/>
    /// </summary>
    public class CustomFieldValueTest
    {
        [Theory]
        [InlineData("{\"id\":100,\"name\":\"血液型\",\"values\":[\"A\"]}", 100, "血液型", "A")]
        [InlineData("{\"id\":100,\"values\":[\"\"]}", 100, null, "")]
        [InlineData("{\"id\": 1,\"values\":[\"Aコース\",\"Bコース\"]}", 1, null, "Aコース,Bコース")]
        public void CanDeserializeJSON(string json, int id, string? name, string values)
        {
            // Arrange - Act
            var fieldValue = JsonSerializer.Deserialize<CustomFieldValue>(json);

            // Assert
            fieldValue.Should().NotBeNull();
            fieldValue!.Id.Should().Be(id);
            fieldValue.Name.Should().Be(name);
            fieldValue.Values.Should().NotBeNullOrEmpty();
            string.Join(",", fieldValue.Values!).Should().Be(values);
        }
    }
}
