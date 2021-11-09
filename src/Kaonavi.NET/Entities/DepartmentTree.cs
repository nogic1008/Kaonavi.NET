namespace Kaonavi.Net.Entities;

using System.Text.Json.Serialization;

/// <summary>所属ツリー</summary>
/// <param name="Code">所属コード</param>
/// <param name="Name">所属名</param>
/// <param name="ParentCode">
/// 親の<inheritdoc cref="DepartmentTree" path="/param[@name='Code']/text()"/>
/// (存在しない場合は<c>null</c>)
/// </param>
/// <param name="LeaderMemberCode">
/// 所属の責任者の<inheritdoc cref="MemberData" path="/param[@name='Code']/text()"/>
/// (存在しない場合は<c>null</c>)
/// </param>
/// <param name="Order">同階層内の並び順</param>
/// <param name="Memo">所属のメモ (存在しない場合は<c>null</c>)</param>
public record DepartmentTree(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("parent_code")] string? ParentCode,
    [property: JsonPropertyName("leader_member_code")] string? LeaderMemberCode,
    [property: JsonPropertyName("order")] int Order,
    [property: JsonPropertyName("memo")] string? Memo
);