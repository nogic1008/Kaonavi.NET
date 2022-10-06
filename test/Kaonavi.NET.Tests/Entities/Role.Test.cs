using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="Role"/>の単体テスト
/// </summary>
public class RoleTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><see cref="Role.Id"/></param>
    /// <param name="name"><see cref="Role.Name"/></param>
    /// <param name="type"><see cref="Role.Type"/></param>
    [Theory(DisplayName = $"{nameof(Role)} > JSONからデシリアライズできる。")]
    [InlineData("{\"id\": 1,\"name\": \"カオナビ管理者\",\"type\": \"Adm\"}", 1, "カオナビ管理者", "Adm")]
    [InlineData("{\"id\": 2,\"name\": \"カオナビマネージャー\",\"type\": \"一般\"}", 2, "カオナビマネージャー", "一般")]
    public void CanDeserializeJSON(string json, int id, string name, string type)
    {
        // Arrange - Act
        var role = JsonSerializer.Deserialize<Role>(json, JsonConfig.Default);

        // Assert
        _ = role.Should().Be(new Role(id, name, type));
    }
}
