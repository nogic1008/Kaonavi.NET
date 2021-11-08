namespace Kaonavi.Net.Entities;

/// <summary>ログインユーザー情報</summary>
/// <param name="Id">ユーザーID</param>
/// <param name="EMail">ログインメールアドレス</param>
/// <param name="MemberCode">
/// <inheritdoc cref="MemberData" path="/param[@name='Code']/text()"/>
/// (紐付けメンバーが設定されていない場合は<c>null</c>)
/// </param>
/// <param name="Role"><inheritdoc cref="Entities.Role" path="/summary/text()"/></param>
public record User(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("email")] string EMail,
    [property: JsonPropertyName("member_code")] string? MemberCode,
    [property: JsonPropertyName("role")] Role Role
);
