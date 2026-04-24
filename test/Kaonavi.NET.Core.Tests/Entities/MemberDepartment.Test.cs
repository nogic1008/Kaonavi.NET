using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="MemberDepartment"/>の単体テスト
/// </summary>
[Category("Entities")]
public sealed class MemberDepartmentTest
{
    private const string TestName = $"{nameof(MemberDepartment)} > JSONからデシリアライズできる。";

    /// <summary><see cref="CanDeserializeJSON"/>のテストデータ</summary>
    public static IEnumerable<TestDataRow<(string json, string code, string? name, string[]? names)>> TestData
    {
        get
        {
            yield return new((/*lang=json,strict*/ """{ "code": "1000" }""", "1000", null, null)) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
            {
              "code": "1000",
              "name":"取締役会",
              "names": ["取締役会"]
            }
            """, "1000", "取締役会", ["取締役会"])) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
            {
              "code": "2000",
              "name": "営業本部 第一営業部 ITグループ",
              "names": ["営業本部", "第一営業部", "ITグループ"]
            }
            """, "2000", "営業本部 第一営業部 ITグループ", ["営業本部", "第一営業部", "ITグループ"])) { DisplayName = TestName };
        }
    }

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="code"><inheritdoc cref="MemberDepartment.Code" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="MemberDepartment.Name" path="/summary"/></param>
    /// <param name="names"><inheritdoc cref="MemberDepartment.Names" path="/summary"/></param>
    [Test(TestName)]
    [Category("JSON Deserialize")]
    [MethodDataSource(nameof(TestData))]
    public async Task CanDeserializeJSON([StringSyntax(StringSyntaxAttribute.Json)] string json, string code, string? name, string[]? names)
    {
        // Arrange - Act
        var memberDepartment = JsonSerializer.Deserialize(json, JsonContext.Default.MemberDepartment);

        // Assert
        await Assert.That(memberDepartment).IsNotNull()
            .And.Member(sut => sut.Code, o => o.IsEqualTo<string>(code))
            .And.Member(sut => sut.Name!, o => name is null ? o.IsNull() : o.IsEqualTo<string>(name))
            .And.Member(sut => sut.Names!, o => names is null ? o.IsNull() : o.IsEquivalentTo(names));
    }
}
