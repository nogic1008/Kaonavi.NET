namespace Kaonavi.Net.Entities;

/// <summary>ログインユーザー情報</summary>
/// <param name="Id">ユーザーID</param>
/// <param name="EMail">ログインメールアドレス</param>
/// <param name="MemberCode">
/// <inheritdoc cref="MemberData" path="/param[@name='Code']/text()"/>
/// (紐付けメンバーが設定されていない場合は<see langword="null"/>)
/// </param>
/// <param name="Role"><inheritdoc cref="Entities.Role" path="/summary/text()"/></param>
public record User(int Id, string EMail, string? MemberCode, Role Role);

/// <inheritdoc cref="User"/>
/// <param name="LastLoginAt">最終ログイン日時(一度もログインしたことがない場合は<see langword="null"/>)</param>
public record UserWithLoginAt(
    int Id,
    string EMail,
    string? MemberCode,
    Role Role,
    DateTime? LastLoginAt
) : User(Id, EMail, MemberCode, Role);
