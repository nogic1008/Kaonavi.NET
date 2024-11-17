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
/// <param name="LastLoginAt">最終ログイン日時(一度もログインしたことがない場合は<see langword="null"/>)</param>
/// <param name="IsActive">アカウント状態(<see langword="false"/>:停止 <see langword="true"/>:利用)</param>
/// <param name="PasswordLocked">パスワードロック(<see langword="false"/>:解除 <see langword="true"/>:ロック)</param>
/// <param name="UseSmartphone">スマホオプションフラグ(<see langword="false"/>:停止 <see langword="true"/>:利用)</param>
public record User(
    int Id,
    string Email,
    string? MemberCode,
    Role Role,
    [property: JsonConverter(typeof(DateTimeConverter)), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] DateTime? LastLoginAt = null,
    bool IsActive = true,
    bool PasswordLocked = false,
    bool UseSmartphone = false
);
