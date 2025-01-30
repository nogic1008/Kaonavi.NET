using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="AdvancedPermission"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
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
    [TestMethod(TestName), TestCategory("JSON Deserialize")]
    [DataRow(AdvancedPermissionJson, 1, (string[])["0001", "0002", "0003"], (string[])["0001", "0002", "0003"], DisplayName = TestName)]
    [DataRow(AdvancedPermissionEmptyJson, 2, (string[])[], (string[])[], DisplayName = TestName)]
    public void CanDeserializeJSON(string json, int userId, string[] addCodes, string[] exclusionCodes)
    {
        // Arrange - Act
        var advancedPermission = JsonSerializer.Deserialize(json, Context.Default.AdvancedPermission);

        // Assert
        advancedPermission.ShouldNotBeNull().ShouldSatisfyAllConditions(
            sut => sut.UserId.ShouldBe(userId),
            sut => sut.AddCodes.ShouldBe(addCodes),
            sut => sut.ExclusionCodes.ShouldBe(exclusionCodes)
        );
    }
}
