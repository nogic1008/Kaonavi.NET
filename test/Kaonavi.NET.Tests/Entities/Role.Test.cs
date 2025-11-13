using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="Role"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class RoleTest
{
    private const string TestName = $"{nameof(Role)} > JSONからデシリアライズできる。";

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><inheritdoc cref="Role.Id" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="Role.Name" path="/summary"/></param>
    /// <param name="type"><inheritdoc cref="Role.Type" path="/summary"/></param>
    [TestMethod(DisplayName = TestName), TestCategory("JSON Deserialize")]
    [DataRow(/*lang=json,strict*/ """{ "id": 1, "name": "カオナビ管理者", "type": "Adm" }""", 1, "カオナビ管理者", "Adm", DisplayName = TestName)]
    [DataRow(/*lang=json,strict*/ """{ "id": 2, "name": "カオナビマネージャー", "type": "一般" }""", 2, "カオナビマネージャー", "一般", DisplayName = TestName)]
    public void CanDeserializeJSON(string json, int id, string name, string type)
        => JsonSerializer.Deserialize(json, Context.Default.Role).ShouldBe(new(id, name, type));
}
