using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>所属情報</summary>
    public record DepartmentInfo
    {
        /// <summary>
        /// DepartmentInfoの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="code"><inheritdoc cref="Code" path="/summary/text()"/></param>
        /// <param name="name"><inheritdoc cref="Name" path="/summary/text()"/></param>
        /// <param name="parentCode"><inheritdoc cref="ParentCode" path="/summary/text()"/></param>
        /// <param name="leaderMemberCode"><inheritdoc cref="LeaderMemberCode" path="/summary/text()"/></param>
        /// <param name="order"><inheritdoc cref="Order" path="/summary/text()"/></param>
        /// <param name="memo"><inheritdoc cref="Memo" path="/summary/text()"/></param>
        public DepartmentInfo(string code, string name, string? parentCode, string? leaderMemberCode, int order, string? memo)
            => (Code, Name, ParentCode, LeaderMemberCode, Order, Memo) = (code, name, parentCode, leaderMemberCode, order, memo);

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
