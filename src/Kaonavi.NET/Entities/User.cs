using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>ログインユーザー情報</summary>
    public record User
    {
        /// <summary>
        /// Userの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <param name="email">ログインメールアドレス</param>
        /// <param name="memberCode">社員番号 (紐付けメンバーが設定されていない場合は<c>null</c>)</param>
        /// <param name="role">ロール情報</param>
        public User(int id, string email, string? memberCode, Role role)
            => (Id, EMail, MemberCode, Role) = (id, email, memberCode, role);

        /// <summary>ユーザーID</summary>
        [JsonPropertyName("id")]
        public int Id { get; init; }

        /// <summary>ログインメールアドレス</summary>
        [JsonPropertyName("email")]
        public string EMail { get; init; }

        /// <summary>社員番号 (紐付けメンバーが設定されていない場合は<c>null</c>)</summary>
        [JsonPropertyName("member_code")]
        public string? MemberCode { get; init; }

        /// <summary>ロール情報</summary>
        [JsonPropertyName("role")]
        public Role Role { get; init; }
    }
}
