using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="DepartmentTree"/>の単体テスト</summary>
[Category("Entities")]
public sealed class DepartmentTreeTest
{
    private const string TestName = $"{nameof(DepartmentTree)} > JSONからデシリアライズできる。";

    /// <summary><see cref="CanDeserializeJSON"/>のテストデータ</summary>
    public static IEnumerable<TestDataRow<(string json, string code, string name, string? parentCode, string? leaderMemberCode, int order, string? memo)>> TestData
    {
        get
        {
            yield return new((/*lang=json,strict*/ """
            {
              "code": "1000",
              "name": "取締役会",
              "parent_code": null,
              "leader_member_code": "A0002",
              "order": 1,
              "memo": ""
            }
            """, "1000", "取締役会", null, "A0002", 1, "")) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
            {
              "code": "1200",
              "name": "営業本部",
              "parent_code": null,
              "leader_member_code": null,
              "order": 2,
              "memo": null
            }
            """, "1200", "営業本部", null, null, 2, null)) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
            {
              "code": "2000",
              "name": "ITグループ",
              "parent_code": "1500",
              "leader_member_code": "A0001",
              "order": 1,
              "memo": "example"
            }
            """, "2000", "ITグループ", "1500", "A0001", 1, "example")) { DisplayName = TestName };
        }
    }

    /// <summary>JSONからデシリアライズできる。</summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="code"><inheritdoc cref="DepartmentTree" path="/param[@name='Code']"/></param>
    /// <param name="name"><inheritdoc cref="DepartmentTree" path="/param[@name='Name']"/></param>
    /// <param name="parentCode"><inheritdoc cref="DepartmentTree" path="/param[@name='ParentCode']"/></param>
    /// <param name="leaderMemberCode"><inheritdoc cref="DepartmentTree" path="/param[@name='LeaderMemberCode']"/></param>
    /// <param name="order"><inheritdoc cref="DepartmentTree" path="/param[@name='Order']"/></param>
    /// <param name="memo"><inheritdoc cref="DepartmentTree" path="/param[@name='Memo']"/></param>
    [Test(TestName)]
    [Category("JSON Deserialize")]
    [MethodDataSource(nameof(TestData))]
    public async Task CanDeserializeJSON([StringSyntax(StringSyntaxAttribute.Json)] string json, string code, string name, string? parentCode, string? leaderMemberCode, int order, string? memo)
    {
        var result = JsonSerializer.Deserialize(json, JsonContext.Default.DepartmentTree);
        await Assert.That(result).IsEqualTo(new(code, name, parentCode, leaderMemberCode, order, memo));
    }
}
