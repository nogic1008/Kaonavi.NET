using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="AdvancedPermission"/>の単体テスト</summary>
[Category("Entities")]
public sealed class AdvancedPermissionTest
{
    // lang=json,strict
    private const string AdvancedPermissionJson = """
    {
      "user_id": 1,
      "add_codes": ["0001", "0002", "0003"],
      "exclusion_codes": ["0001", "0002", "0003"]
    }
    """;
    // lang=json,strict
    private const string AdvancedPermissionEmptyJson = """
    {
      "user_id": 2,
      "add_codes": [],
      "exclusion_codes": []
    }
    """;

    private const string TestName = $"{nameof(AdvancedPermission)} > JSONからデシリアライズできる。";

    /// <summary>JSONからデシリアライズできる。</summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="userId"><inheritdoc cref="AdvancedPermission.UserId" path="/summary"/></param>
    /// <param name="addCodes"><inheritdoc cref="AdvancedPermission.AddCodes" path="/summary"/></param>
    /// <param name="exclusionCodes"><inheritdoc cref="AdvancedPermission.ExclusionCodes" path="/summary"/></param>
    [Test(TestName), Category("JSON Deserialize")]
    [Arguments(AdvancedPermissionJson, 1, (string[])["0001", "0002", "0003"], (string[])["0001", "0002", "0003"], DisplayName = TestName)]
    [Arguments(AdvancedPermissionEmptyJson, 2, (string[])[], (string[])[], DisplayName = TestName)]
    public async Task CanDeserializeJSON(string json, int userId, string[] addCodes, string[] exclusionCodes)
    {
        // Arrange - Act
        var advancedPermission = JsonSerializer.Deserialize(json, JsonContext.Default.AdvancedPermission);

        // Assert
        await Assert.That(advancedPermission).IsNotNull()
            .And.Member(static o => o.UserId, o => o.IsEqualTo(userId))
            .And.Member(static o => o.AddCodes, o => o.IsEquivalentTo(addCodes))
            .And.Member(static o => o.ExclusionCodes, o => o.IsEquivalentTo(exclusionCodes));
    }
}
