using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>ログインユーザー情報</summary>
    public record User(
        /// <summary>ユーザーID</summary>
        [property: JsonPropertyName("id")] int Id,
        /// <summary>ログインメールアドレス</summary>
        [property: JsonPropertyName("email")] string EMail,
        /// <summary>社員番号 (紐付けメンバーが設定されていない場合は<c>null</c>)</summary>
        [property: JsonPropertyName("member_code")] string? MemberCode,
        /// <summary>ロール情報</summary>
        [property: JsonPropertyName("role")] Role Role
    );
}
