using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="User"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class UserTest
{
    /*lang=json,strict*/
    private const string AdminUserJson = """
    {
        "id": 1,
        "email":"taro@kaonavi.jp",
        "member_code":"A0002",
        "role":{
            "id":1,
            "name":"システム管理者",
            "type":"Adm"
        },
        "is_active": true,
        "password_locked": false,
        "use_smartphone": false,
        "last_login_at": "2021-11-01 12:00:00"
    }
    """;
    /*lang=json,strict*/
    private const string NonMemberJson = """
    {
        "id": 2,
        "email": "hanako@kaonavi.jp",
        "member_code": null,
        "role": {
            "id": 2,
            "name": "マネージャ",
            "type": "一般"
        },
        "is_active": true,
        "password_locked": true,
        "use_smartphone": true,
        "last_login_at": "2021-11-01 12:00:00"
    }
    """;

    private const string UserTestName = $"{nameof(User)} > JSONからデシリアライズできる。";

    /// <summary>
    /// JSONから<see cref="User"/>にデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><see cref="User.Id"/></param>
    /// <param name="email"><see cref="User.EMail"/></param>
    /// <param name="memberCode"><see cref="User.MemberCode"/></param>
    /// <param name="roleId"><see cref="Role.Id"/></param>
    /// <param name="roleName"><see cref="Role.Name"/></param>
    /// <param name="roleType"><see cref="Role.Type"/></param>
    /// <param name="isActive"><see cref="User.IsActive"/></param>
    /// <param name="passwordLocked"><see cref="User.PasswordLocked"/></param>
    /// <param name="useSmartphone"><see cref="User.UseSmartphone"/></param>
    [TestMethod(UserTestName), TestCategory("JSON Deserialize")]
    [DataRow(AdminUserJson, 1, "taro@kaonavi.jp", "A0002", 1, "システム管理者", "Adm", true, false, false, DisplayName = UserTestName)]
    [DataRow(NonMemberJson, 2, "hanako@kaonavi.jp", null, 2, "マネージャ", "一般", true, true, true, DisplayName = UserTestName)]
    public void User_CanDeserializeJSON(string json, int id, string email, string? memberCode, int roleId, string roleName, string roleType, bool isActive, bool passwordLocked, bool useSmartphone)
    {
        // Arrange - Act
        var user = JsonSerializer.Deserialize(json, Context.Default.User);

        // Assert
        _ = user.Should().NotBeNull();
        _ = user!.Id.Should().Be(id);
        _ = user.Email.Should().Be(email);
        _ = user.MemberCode.Should().Be(memberCode);
        _ = user.IsActive.Should().Be(isActive);
        _ = user.PasswordLocked.Should().Be(passwordLocked);
        _ = user.UseSmartphone.Should().Be(useSmartphone);
        _ = user.Role.Should().Be(new Role(roleId, roleName, roleType));
    }
}
