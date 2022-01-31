namespace Kaonavi.Net.Entities;

/// <summary>所属ツリー</summary>
/// <param name="Code">所属コード</param>
/// <param name="Name">所属名</param>
/// <param name="ParentCode">
/// 親の<inheritdoc cref="DepartmentTree" path="/param[@name='Code']/text()"/>
/// (存在しない場合は<see langword="null"/>)
/// </param>
/// <param name="LeaderMemberCode">
/// 所属の責任者の<inheritdoc cref="MemberData" path="/param[@name='Code']/text()"/>
/// (存在しない場合は<see langword="null"/>)
/// </param>
/// <param name="Order">同階層内の並び順</param>
/// <param name="Memo">所属のメモ (存在しない場合は<see langword="null"/>)</param>
public record DepartmentTree(
    string Code,
    string Name,
    string? ParentCode,
    string? LeaderMemberCode,
    int Order,
    string? Memo
);
