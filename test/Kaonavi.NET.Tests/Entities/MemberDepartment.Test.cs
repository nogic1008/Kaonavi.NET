using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="MemberDepartment"/>の単体テスト
/// </summary>
public class MemberDepartmentTest
{
    private const string SimpleJson = "{\"code\":\"所属コード\"}";
    private const string SingleDepJson = "{\"code\": \"1000\",\"name\":\"取締役会\",\"names\":[\"取締役会\"]}";
    private const string MultipleJson = "{\"code\":\"2000\",\"name\":\"営業本部 第一営業部 ITグループ\",\"names\":[\"営業本部\",\"第一営業部\",\"ITグループ\"]}";

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="code"><see cref="MemberDepartment.Code"/></param>
    /// <param name="name"><see cref="MemberDepartment.Name"/></param>
    /// <param name="names"><see cref="MemberDepartment.Names"/></param>
    [Theory(DisplayName = $"{nameof(MemberDepartment)} > JSONからデシリアライズできる。")]
    [InlineData(SimpleJson, "所属コード", null, null)]
    [InlineData(SingleDepJson, "1000", "取締役会", "取締役会")]
    [InlineData(MultipleJson, "2000", "営業本部 第一営業部 ITグループ", "営業本部", "第一営業部", "ITグループ")]
    public void CanDeserializeJSON(string json, string code, string? name, params string?[] names)
    {
        // Arrange - Act
        var department = JsonSerializer.Deserialize<MemberDepartment>(json, JsonConfig.Default);

        // Assert
        _ = department.Should().NotBeNull();
        _ = department!.Code.Should().Be(code);
        _ = department.Name.Should().Be(name);
        _ = department.Names.Should().Equal(names);
    }
}
