using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// Unit test for <see cref="Role"/>
    /// </summary>
    public class RoleTest
    {
        [Theory]
        [InlineData("{\"id\": 1,\"name\": \"カオナビ管理者\",\"type\": \"Adm\"}", 1, "カオナビ管理者", "Adm")]
        [InlineData("{\"id\": 2,\"name\": \"カオナビマネージャー\",\"type\": \"一般\"}", 2, "カオナビマネージャー", "一般")]
        public void CanDeserializeJSON(string json, int id, string name, string type)
        {
            // Arrange - Act
            var role = JsonSerializer.Deserialize<Role>(json);

            // Assert
            role.Should().Be(new Role(id, name, type));
        }
    }
}
