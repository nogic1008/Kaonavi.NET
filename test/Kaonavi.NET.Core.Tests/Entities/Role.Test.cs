using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="Role"/>の単体テスト</summary>
[Category("Entities")]
public sealed class RoleTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><inheritdoc cref="Role.Id" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="Role.Name" path="/summary"/></param>
    /// <param name="type"><inheritdoc cref="Role.Type" path="/summary"/></param>
    [Test, Category("JSON Deserialize")]
    [DisplayName($"{nameof(Role)} > $json から {nameof(Role)}($id, \"$name\", \"$type\") にデシリアライズできる。")]
    [Arguments(/*lang=json,strict*/ """{ "id": 1, "name": "カオナビ管理者", "type": "Adm" }""", 1, "カオナビ管理者", "Adm")]
    [Arguments(/*lang=json,strict*/ """{ "id": 2, "name": "カオナビマネージャー", "type": "一般" }""", 2, "カオナビマネージャー", "一般")]
    public async Task CanDeserializeJSON(string json, int id, string name, string type)
        => await Assert.That(JsonSerializer.Deserialize(json, JsonContext.Default.Role))
            .IsEqualTo(new(id, name, type));
}
