using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Services;

/// <summary><see cref="User"/>の追加/更新APIに用いるpayload</summary>
/// <param name="Email"><inheritdoc cref="User" path="/param[@name='EMail']"/></param>
/// <param name="MemberCode"><inheritdoc cref="User" path="/param[@name='MemberCode']"/></param>
/// <param name="Password">パスワード</param>
/// <param name="RoleId"><inheritdoc cref="Role" path="/param[@name='Id']"/></param>
public record UserPayload(string Email, string? MemberCode, string Password, int RoleId);
