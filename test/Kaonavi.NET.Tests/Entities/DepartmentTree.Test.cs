using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="DepartmentTree"/>の単体テスト
/// </summary>
public class DepartmentTreeTest
{
    /*lang=json,strict*/
    private const string SimpleJson = """
    {
        "code": "1000",
        "name": "取締役会",
        "parent_code": null,
        "leader_member_code": "A0002",
        "order": 1,
        "memo": ""
    }
    """;
    /*lang=json,strict*/
    private const string NoLeaderJson = """
    {
        "code": "1200",
        "name": "営業本部",
        "parent_code": null,
        "leader_member_code": null,
        "order": 2,
        "memo": null
    }
    """;
    /*lang=json,strict*/
    private const string ChildJson = """
    {
        "code": "2000",
        "name": "ITグループ",
        "parent_code": "1500",
        "leader_member_code": "A0001",
        "order": 1,
        "memo": "example"
    }
    """;

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="code"><inheritdoc cref="DepartmentTree" path="/param[@name='Code']"/></param>
    /// <param name="name"><inheritdoc cref="DepartmentTree" path="/param[@name='Name']"/></param>
    /// <param name="parentCode"><inheritdoc cref="DepartmentTree" path="/param[@name='ParentCode']"/></param>
    /// <param name="leaderMemberCode"><inheritdoc cref="DepartmentTree" path="/param[@name='LeaderMemberCode']"/></param>
    /// <param name="order"><inheritdoc cref="DepartmentTree" path="/param[@name='Order']"/></param>
    /// <param name="memo"><inheritdoc cref="DepartmentTree" path="/param[@name='Memo']"/></param>
    [Theory(DisplayName = $"{nameof(DepartmentTree)} > JSONからデシリアライズできる。")]
    [InlineData(SimpleJson, "1000", "取締役会", null, "A0002", 1, "")]
    [InlineData(NoLeaderJson, "1200", "営業本部", null, null, 2, null)]
    [InlineData(ChildJson, "2000", "ITグループ", "1500", "A0001", 1, "example")]
    public void CanDeserializeJSON(string json, string code, string name, string? parentCode, string? leaderMemberCode, int order, string? memo)
        => JsonSerializer.Deserialize(json, Context.Default.DepartmentTree)
            .Should().Be(new DepartmentTree(code, name, parentCode, leaderMemberCode, order, memo));
}
