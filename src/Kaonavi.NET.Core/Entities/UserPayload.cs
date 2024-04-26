namespace Kaonavi.Net.Entities;

/// <summary><see cref="User"/>の追加/更新APIに用いるpayload</summary>
/// <param name="Email"><inheritdoc cref="User" path="/param[@name='EMail']"/></param>
/// <param name="MemberCode"><inheritdoc cref="User" path="/param[@name='MemberCode']"/></param>
/// <param name="Password">パスワード</param>
/// <param name="RoleId"><inheritdoc cref="Role" path="/param[@name='Id']"/></param>
/// <param name="IsActive"><inheritdoc cref="User" path="/param[@name='IsActive']"/></param>
/// <param name="PasswordLocked"><inheritdoc cref="User" path="/param[@name='PasswordLocked']"/></param>
/// <param name="UseSmartphone"><inheritdoc cref="User" path="/param[@name='UseSmartphone']"/></param>
public record UserPayload(
    string Email,
    string? MemberCode,
    string Password,
    int RoleId,
    bool IsActive = true,
    bool PasswordLocked = false,
    bool UseSmartphone = false
);
