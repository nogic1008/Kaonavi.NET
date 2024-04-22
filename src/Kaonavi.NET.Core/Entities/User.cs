using Kaonavi.Net.Json;

namespace Kaonavi.Net.Entities;

/// <summary>ログインユーザー情報</summary>
/// <param name="Id">ユーザーID</param>
/// <param name="Email">ログインメールアドレス</param>
/// <param name="MemberCode">
/// <inheritdoc cref="MemberData" path="/param[@name='Code']"/>
/// (紐付けメンバーが設定されていない場合は<see langword="null"/>)
/// </param>
/// <param name="Role"><inheritdoc cref="Entities.Role" path="/summary"/></param>
public record User(int Id, string Email, string? MemberCode, Role Role);

/// <inheritdoc cref="User"/>
/// <param name="Id"><inheritdoc cref="User" path="/param[@name='Id']"/></param>
/// <param name="Email"><inheritdoc cref="User" path="/param[@name='Email']"/></param>
/// <param name="MemberCode"><inheritdoc cref="User" path="/param[@name='MemberCode']"/></param>
/// <param name="Role"><inheritdoc cref="User" path="/param[@name='Role']"/></param>
/// <param name="LastLoginAt">最終ログイン日時(一度もログインしたことがない場合は<see langword="null"/>)</param>
public record UserWithLoginAt(
    int Id,
    string Email,
    string? MemberCode,
    Role Role,
    [property: JsonConverter(typeof(DateTimeConverter))] DateTime? LastLoginAt
) : User(Id, Email, MemberCode, Role);
