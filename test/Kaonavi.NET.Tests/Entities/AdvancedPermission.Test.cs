using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="AdvancedPermission"/>の単体テスト
/// </summary>
public sealed class AdvancedPermissionTest
{
    /// <summary>
    /// <see cref="CanDeserializeJSON"/>のテストデータ
    /// </summary>
    public static TheoryData<string, int, string[], string[]> TestDataForCanDeserializeJSON => new()
    {
        { /*lang=json,strict*/ """
            {
              "user_id": 1,
              "add_codes": [
                "0001",
                "0002",
                "0003"
              ],
              "exclusion_codes": [
                "0001",
                "0002",
                "0003"
              ]
            }
            """, 1, new[] { "0001", "0002", "0003" }, new[] { "0001", "0002", "0003" }
        },
        { /*lang=json,strict*/ """
            {
                "user_id": 2,
                "add_codes": [],
                "exclusion_codes": []
            }
            """, 2, Array.Empty<string>(), Array.Empty<string>()
        },
    };
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="userId"><inheritdoc cref="AdvancedPermission.UserId" path="/summary"/></param>
    /// <param name="addCodes"><inheritdoc cref="AdvancedPermission.AddCodes" path="/summary"/></param>
    /// <param name="exclusionCodes"><inheritdoc cref="AdvancedPermission.ExclusionCodes" path="/summary"/></param>
    [Theory(DisplayName = $"{nameof(AdvancedPermission)} > JSONからデシリアライズできる。")]
    [MemberData(nameof(TestDataForCanDeserializeJSON))]
    public void CanDeserializeJSON(string json, int userId, string[] addCodes, params string[] exclusionCodes)
    {
        // Arrange - Act
        var sut = JsonSerializer.Deserialize(json, Context.Default.AdvancedPermission);

        // Assert
        _ = sut.Should().NotBeNull();
        _ = sut!.UserId.Should().Be(userId);
        _ = sut.AddCodes.Should().Equal(addCodes);
        _ = sut.ExclusionCodes.Should().Equal(exclusionCodes);
    }
}
