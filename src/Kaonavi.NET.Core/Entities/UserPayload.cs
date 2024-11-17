namespace Kaonavi.Net.Entities;

/// <summary><see cref="User"/>の追加/更新APIに用いるpayload</summary>
/// <param name="Email"><inheritdoc cref="User" path="/param[@name='Email']"/></param>
/// <param name="MemberCode"><inheritdoc cref="User" path="/param[@name='MemberCode']"/></param>
/// <param name="Password">パスワード</param>
/// <param name="Role"><inheritdoc cref="Entities.Role"/></param>
/// <param name="IsActive"><inheritdoc cref="User" path="/param[@name='IsActive']"/></param>
/// <param name="PasswordLocked"><inheritdoc cref="User" path="/param[@name='PasswordLocked']"/></param>
/// <param name="UseSmartphone"><inheritdoc cref="User" path="/param[@name='UseSmartphone']"/></param>
[method: JsonConstructor]
public record UserPayload(
    string Email,
    string? MemberCode,
    string Password,
    Role Role,
    bool IsActive = true,
    bool PasswordLocked = false,
    bool UseSmartphone = false
)
{
    /// <summary>新しい<see cref="UserPayload"/>を生成します。</summary>
    /// <param name="email"><inheritdoc cref="User" path="/param[@name='Email']"/></param>
    /// <param name="memberCode"><inheritdoc cref="User" path="/param[@name='MemberCode']"/></param>
    /// <param name="password"><inheritdoc cref="UserPayload(string, string?, string, Role, bool, bool, bool)" path="/param[@name='Password']"/></param>
    /// <param name="roleId"><inheritdoc cref="Entities.Role" path="/param[@name='Id']"/></param>
    /// <param name="isActive"><inheritdoc cref="User" path="/param[@name='IsActive']"/></param>
    /// <param name="passwordLocked"><inheritdoc cref="User" path="/param[@name='PasswordLocked']"/></param>
    /// <param name="useSmartphone"><inheritdoc cref="User" path="/param[@name='UseSmartphone']"/></param>
    public UserPayload(string email, string? memberCode, string password, int roleId, bool isActive = true, bool passwordLocked = false, bool useSmartphone = false)
        : this(email, memberCode, password, new Role(roleId, null!, null!), isActive, passwordLocked, useSmartphone) { }
}
