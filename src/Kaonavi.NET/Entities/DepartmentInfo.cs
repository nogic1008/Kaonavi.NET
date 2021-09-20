using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>所属情報</summary>
    public record DepartmentInfo
    {
        /// <summary>
        /// DepartmentInfoの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="code">所属コード</param>
        /// <param name="name">所属名</param>
        /// <param name="order">同階層内の並び順</param>
        public DepartmentInfo(string code, string name, int order)
            => (Code, Name, Order) = (code, name, order);

        /// <summary>
        /// DepartmentInfoの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="code">所属コード</param>
        /// <param name="name">所属名</param>
        /// <param name="parentCode">親の所属コード (存在しない場合は<c>null</c>)</param>
        /// <param name="leaderMemberCode">所属の責任者の社員番号 (存在しない場合は<c>null</c>)</param>
        /// <param name="order">同階層内の並び順</param>
        /// <param name="memo">所属のメモ (存在しない場合は<c>null</c>)</param>
        [JsonConstructor]
        public DepartmentInfo(string code, string name, string? parentCode, string? leaderMemberCode, int order, string? memo) : this(code, name, order)
            => (ParentCode, LeaderMemberCode, Memo) = (parentCode, leaderMemberCode, memo);

        /// <summary>所属コード</summary>
        [JsonPropertyName("code")]
        public string Code { get; init; }

        /// <summary>所属名</summary>
        [JsonPropertyName("name")]
        public string Name { get; init; }

        /// <summary>親の所属コード (存在しない場合は<c>null</c>)</summary>
        [JsonPropertyName("parent_code")]
        public string? ParentCode { get; init; }

        /// <summary>所属の責任者の社員番号 (存在しない場合は<c>null</c>)</summary>
        [JsonPropertyName("leader_member_code")]
        public string? LeaderMemberCode { get; init; }

        /// <summary>同階層内の並び順</summary>
        [JsonPropertyName("order")]
        public int Order { get; init; }

        /// <summary>所属のメモ (存在しない場合は<c>null</c>)</summary>
        [JsonPropertyName("memo")]
        public string? Memo { get; init; }
    }
}
