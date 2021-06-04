namespace Kaonavi.Net.Entities.Api
{
    /// <summary><see cref="User"/>の追加/更新APIに用いるpayload</summary>
    public record UserPayload(
        /// <summary><see cref="User.EMail"/></summary>
        string EMail,
        /// <summary><see cref="User.MemberCode"/></summary>
        string? MemberCode,
        /// <summary>パスワード</summary>
        string Password,
        /// <summary><see cref="Role.Id"/></summary>
        int RoleId
    );
}
