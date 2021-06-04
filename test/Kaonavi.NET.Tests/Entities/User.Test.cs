using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// <see cref="User"/>の単体テスト
    /// </summary>
    public class UserTest
    {
        private const string TestName = nameof(User) + " > ";

        /// <summary>
        /// JSONからデシリアライズできる。
        /// </summary>
        /// <param name="json">JSON文字列</param>
        /// <param name="id"><see cref="User.Id"/></param>
        /// <param name="email"><see cref="User.EMail"/></param>
        /// <param name="memberCode"><see cref="User.MemberCode"/></param>
        [Theory(DisplayName = TestName + "JSONからデシリアライズできる。")]
        [InlineData(
            "{\"id\": 1,\"email\":\"taro@kaonavi.jp\",\"member_code\":\"A0002\",\"role\":{\"id\":1,\"name\":\"システム管理者\",\"type\":\"Adm\"}}",
            1, "taro@kaonavi.jp", "A0002"
        )]
        [InlineData(
            "{\"id\": 2,\"email\": \"hanako@kaonavi.jp\",\"member_code\": null,\"role\": {\"id\": 2,\"name\": \"マネージャ\",\"type\": \"一般\"}}",
            2, "hanako@kaonavi.jp", null
        )]
        public void CanDeserializeJSON(string json, int id, string email, string? memberCode)
        {
            // Arrange - Act
            var user = JsonSerializer.Deserialize<User>(json);

            // Assert
            user.Should().NotBeNull();
            user!.Id.Should().Be(id);
            user.EMail.Should().Be(email);
            user.MemberCode.Should().Be(memberCode);

            user.Role.Should().BeAssignableTo<Role>();
            user.Role.Id.Should().NotBe(0);
            user.Role.Name.Should().NotBeNullOrEmpty();
            user.Role.Type.Should().NotBeNullOrEmpty();
        }
    }
}