using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="MemberDepartment"/>の単体テスト
/// </summary>
public class MemberDepartmentTest
{
    /*lang=json,strict*/
    private const string SimpleJson = """{ "code": "所属コード" }""";
    /*lang=json,strict*/
    private const string SingleDepJson = """{ "code": "1000", "name":"取締役会", "names": ["取締役会"] }""";
    /*lang=json,strict*/
    private const string MultipleJson = """
    {
        "code": "2000",
        "name": "営業本部 第一営業部 ITグループ",
        "names": ["営業本部", "第一営業部", "ITグループ"]
    }
    """;

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="code"><inheritdoc cref="MemberDepartment.Code" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="MemberDepartment.Name" path="/summary"/></param>
    /// <param name="names"><inheritdoc cref="MemberDepartment.Names" path="/summary"/></param>
    [Theory(DisplayName = $"{nameof(MemberDepartment)} > JSONからデシリアライズできる。")]
    [InlineData(SimpleJson, "所属コード", null, null)]
    [InlineData(SingleDepJson, "1000", "取締役会", "取締役会")]
    [InlineData(MultipleJson, "2000", "営業本部 第一営業部 ITグループ", "営業本部", "第一営業部", "ITグループ")]
    public void CanDeserializeJSON(string json, string code, string? name, params string?[] names)
    {
        // Arrange - Act
        var department = JsonSerializer.Deserialize(json, Context.Default.MemberDepartment);

        // Assert
        _ = department.Should().NotBeNull();
        _ = department!.Code.Should().Be(code);
        _ = department.Name.Should().Be(name);
        _ = department.Names.Should().Equal(names);
    }
}
