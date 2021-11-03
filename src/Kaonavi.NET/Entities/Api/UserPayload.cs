namespace Kaonavi.Net.Entities.Api
{
    /// <summary><see cref="User"/>の追加/更新APIに用いるpayload</summary>
    /// <param name="EMail"><see cref="User.EMail"/></param>
    /// <param name="MemberCode"><see cref="User.MemberCode"/></param>
    /// <param name="Password">パスワード</param>
    /// <param name="RoleId"><see cref="Role.Id"/></param>
    public record UserPayload(string EMail, string? MemberCode, string Password, int RoleId);
}
