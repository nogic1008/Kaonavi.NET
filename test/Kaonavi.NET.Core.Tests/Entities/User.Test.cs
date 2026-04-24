using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="User"/>の単体テスト</summary>
[Category("Entities")]
public sealed class UserTest
{
    private const string TestName = $"{nameof(User)} > JSONからデシリアライズできる。";

    /// <summary><see cref="CanDeserializeJSON"/>のテストデータ</summary>
    public static IEnumerable<TestDataRow<(string json, int id, string email, string? memberCode, int roleId, string roleName, string roleType, bool isActive, bool passwordLocked, bool useSmartphone)>> TestData
    {
        get
        {
            yield return new((/*lang=json,strict*/ """
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
            """, 1, "taro@kaonavi.jp", "A0002", 1, "システム管理者", "Adm", true, false, false)) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
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
            """, 2, "hanako@kaonavi.jp", null, 2, "マネージャ", "一般", true, true, true)) { DisplayName = TestName };
        }
    }

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
    [Test(TestName)]
    [Category("JSON Deserialize")]
    [MethodDataSource(nameof(TestData))]
    public async Task CanDeserializeJSON(string json, int id, string email, string? memberCode, int roleId, string roleName, string roleType, bool isActive, bool passwordLocked, bool useSmartphone)
    {
        // Arrange - Act
        var user = JsonSerializer.Deserialize(json, JsonContext.Default.User);

        // Assert
        await Assert.That(user).IsNotNull()
            .And.Member(sut => sut.Id, o => o.IsEqualTo(id))
            .And.Member(sut => sut.Email, o => o.IsEqualTo<string>(email))
            .And.Member(sut => sut.MemberCode, o => o.IsEqualTo<string>(memberCode))
            .And.Member(sut => sut.Role, o => o.IsEqualTo(new(roleId, roleName, roleType)))
            .And.Member(sut => sut.IsActive, o => o.IsEqualTo(isActive))
            .And.Member(sut => sut.PasswordLocked, o => o.IsEqualTo(passwordLocked))
            .And.Member(sut => sut.UseSmartphone, o => o.IsEqualTo(useSmartphone));
    }
}
