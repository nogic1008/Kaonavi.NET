using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>所属情報</summary>
    public record DepartmentInfo(
        /// <summary>所属コード</summary>
        [property: JsonPropertyName("code")] string Code,
        /// <summary>所属名</summary>
        [property: JsonPropertyName("name")] string Name,
        /// <summary>親の所属コード (存在しない場合は<c>null</c>)</summary>
        [property: JsonPropertyName("parent_code")] string? ParentCode,
        /// <summary>所属の責任者の社員番号 (存在しない場合は<c>null</c>)</summary>
        [property: JsonPropertyName("leader_member_code")] string? LeaderMemberCode,
        /// <summary>同階層内の並び順</summary>
        [property: JsonPropertyName("order")] int Order,
        /// <summary>所属のメモ (存在しない場合は<c>null</c>)</summary>
        [property: JsonPropertyName("memo")] string? Memo
    );
}
