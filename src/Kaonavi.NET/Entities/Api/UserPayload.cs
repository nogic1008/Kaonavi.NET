namespace Kaonavi.Net.Entities.Api
{
    /// <summary><see cref="User"/>の追加/更新APIに用いるpayload</summary>
    public record UserPayload
    {
        /// <summary>
        /// UserPayloadの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="email"><see cref="User.EMail"/></param>
        /// <param name="memberCode"><see cref="User.MemberCode"/></param>
        /// <param name="password">パスワード</param>
        /// <param name="roleId"><see cref="Role.Id"/></param>
        public UserPayload(string email, string? memberCode, string password, int roleId)
            => (EMail, MemberCode, Password, RoleId) = (email, memberCode, password, roleId);

        /// <summary><see cref="User.EMail"/></summary>
        public string EMail { get; init; }

        /// <summary><see cref="User.MemberCode"/></summary>
        public string? MemberCode { get; init; }

        /// <summary>パスワード</summary>
        public string Password { get; init; }

        /// <summary><see cref="Role.Id"/></summary>
        public int RoleId { get; init; }
    }
}
