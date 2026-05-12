using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="DepartmentTree"/>の単体テスト</summary>
[Category("Entities")]
public sealed class DepartmentTreeTest
{
    /// <summary><see cref="CanDeserializeJSON"/>のテストデータ</summary>
    public static IEnumerable<TestDataRow<(string json, DepartmentTree expected)>> TestData
    {
        get
        {
            yield return new(
                ( /*lang=json,strict*/
                    """
                    {
                      "code": "1000",
                      "name": "取締役会",
                      "parent_code": null,
                      "leader_member_code": "A0002",
                      "order": 1,
                      "memo": ""
                    }
                    """,
                    new("1000", "取締役会", null, "A0002", 1, "")
                )
            );
            yield return new(
                ( /*lang=json,strict*/
                    """
                    {
                      "code": "1200",
                      "name": "営業本部",
                      "parent_code": null,
                      "leader_member_code": null,
                      "order": 2,
                      "memo": null
                    }
                    """,
                    new("1200", "営業本部", null, null, 2, null)
                )
            );
            yield return new(
                ( /*lang=json,strict*/
                    """
                    {
                      "code": "2000",
                      "name": "ITグループ",
                      "parent_code": "1500",
                      "leader_member_code": "A0001",
                      "order": 1,
                      "memo": "example"
                    }
                    """,
                    new("2000", "ITグループ", "1500", "A0001", 1, "example")
                )
            );
        }
    }

    /// <summary>JSONからデシリアライズできる。</summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="expected">期待される<see cref="DepartmentTree"/>オブジェクト</param>
    [Test, Category("JSON Deserialize")]
    [DisplayName($"{nameof(DepartmentTree)} > $json から $expected にデシリアライズできる。")]
    [MethodDataSource(nameof(TestData))]
    public async Task CanDeserializeJSON(
        [StringSyntax(StringSyntaxAttribute.Json)] string json,
        DepartmentTree expected
    )
    {
        var result = JsonSerializer.Deserialize(json, JsonContext.Default.DepartmentTree);
        await Assert.That(result).IsEqualTo(expected);
    }
}
