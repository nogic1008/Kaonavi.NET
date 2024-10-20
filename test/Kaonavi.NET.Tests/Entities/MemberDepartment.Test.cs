using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="MemberDepartment"/>の単体テスト
/// </summary>
[TestClass, TestCategory("Entities")]
public sealed class MemberDepartmentTest
{
    /*lang=json,strict*/
    private const string SimpleJson = """{ "code": "1000" }""";
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

    private const string TestName = $"{nameof(MemberDepartment)} > JSONからデシリアライズできる。";

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="code"><inheritdoc cref="MemberDepartment.Code" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="MemberDepartment.Name" path="/summary"/></param>
    /// <param name="names"><inheritdoc cref="MemberDepartment.Names" path="/summary"/></param>
    [TestMethod(TestName), TestCategory("JSON Deserialize")]
    [DataRow(SimpleJson, "1000", null, null, DisplayName = TestName)]
    [DataRow(SingleDepJson, "1000", "取締役会", (string[])["取締役会"], DisplayName = TestName)]
    [DataRow(MultipleJson, "2000", "営業本部 第一営業部 ITグループ", (string[])["営業本部", "第一営業部", "ITグループ"], DisplayName = TestName)]
    public void CanDeserializeJSON(string json, string code, string? name, string[]? names)
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
