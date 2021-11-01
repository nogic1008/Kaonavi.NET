using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>ログインユーザー情報</summary>
    public record User
    {
        /// <summary>
        /// Userの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="email"><inheritdoc cref="Email" path="/summary"/></param>
        /// <param name="memberCode"><inheritdoc cref="MemberCode" path="/summary"/></param>
        /// <param name="role"><inheritdoc cref="Role" path="/summary"/></param>
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
