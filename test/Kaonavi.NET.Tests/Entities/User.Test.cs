using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="User"/>の単体テスト
/// </summary>
public class UserTest
{
    /*lang=json,strict*/
    private const string MemberJson = "{\"id\": 1,\"email\":\"taro@kaonavi.jp\",\"member_code\":\"A0002\",\"role\":{\"id\":1,\"name\":\"システム管理者\",\"type\":\"Adm\"}}";
    /*lang=json,strict*/
    private const string NonMemberJson = "{\"id\": 2,\"email\": \"hanako@kaonavi.jp\",\"member_code\": null,\"role\": {\"id\": 2,\"name\": \"マネージャ\",\"type\": \"一般\"}}";

    /// <summary>
    /// JSONから<see cref="User"/>にデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><see cref="User.Id"/></param>
    /// <param name="email"><see cref="User.EMail"/></param>
    /// <param name="memberCode"><see cref="User.MemberCode"/></param>
    [Theory(DisplayName = $"{nameof(User)} > JSONからデシリアライズできる。")]
    [InlineData(MemberJson, 1, "taro@kaonavi.jp", "A0002")]
    [InlineData(NonMemberJson, 2, "hanako@kaonavi.jp", null)]
    public void User_CanDeserializeJSON(string json, int id, string email, string? memberCode)
    {
        // Arrange - Act
        var user = JsonSerializer.Deserialize<User>(json, JsonConfig.Default);

        // Assert
        _ = user.Should().NotBeNull();
        _ = user!.Id.Should().Be(id);
        _ = user.EMail.Should().Be(email);
        _ = user.MemberCode.Should().Be(memberCode);

        _ = user.Role.Should().BeAssignableTo<Role>();
        _ = user.Role.Id.Should().NotBe(0);
        _ = user.Role.Name.Should().NotBeNullOrEmpty();
        _ = user.Role.Type.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// JSONから<see cref="UserWithLoginAt"/>にデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(UserWithLoginAt)} > JSONからデシリアライズできる。")]
    public void UserWithLoginAt_CanDeserializeJSON()
    {
        // Arrange
        const string json = "{"
        + "\"id\": 1,"
        + "\"email\": \"example@kaonavi.jp\","
        + "\"member_code\": \"12345\","
        + "\"role\": {"
        + "  \"id\": 1,"
        + "  \"name\": \"システム管理者\","
        + "  \"type\": \"Adm\""
        + "},"
        + "\"last_login_at\": \"2021-11-01 12:00:00Z\""
        + "}";

        // Act
        var user = JsonSerializer.Deserialize<UserWithLoginAt>(json, JsonConfig.Default);

        // Assert
        _ = user.Should().Be(new UserWithLoginAt(
            Id: 1,
            EMail: "example@kaonavi.jp",
            MemberCode: "12345",
            Role: new(1, "システム管理者", "Adm"),
            LastLoginAt: new DateTimeOffset(2021, 11, 1, 12, 0, 0, TimeSpan.Zero).LocalDateTime
        ));
    }
}
