using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.User"/>の単体テスト</summary>
    [Category("API"), Category("ユーザー情報")]
    public sealed class UserTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.User.ListAsync"/>は、"/users"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IUser.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.ListAsync)} > GET /users をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task User_ListAsync_Calls_GetApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "user_data": [
                {
                  "id": 1,
                  "email": "taro@kaonavi.jp",
                  "member_code": "A0002",
                  "role": {
                    "id": 1,
                    "name": "システム管理者",
                    "type": "Adm"
                  },
                  "last_login_at": "2021-11-01 12:00:00",
                  "is_active": true,
                  "password_locked": false,
                  "use_smartphone": false
                },
                {
                  "id": 2,
                  "email": "hanako@kaonavi.jp",
                  "member_code": "A0001",
                  "role": {
                    "id": 2,
                    "name": "マネージャ",
                    "type": "一般"
                  },
                  "last_login_at": null,
                  "is_active": true,
                  "password_locked": false,
                  "use_smartphone": false
                }
              ]
            }
            """;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet("/users").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var users = await sut.User.ListAsync(cancellationToken);

            // Assert
            await Assert.That(users).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path("/users"), Times.Once);
        }

        /// <summary>
        /// <see cref="KaonaviClient.User.CreateAsync"/>は、"/users"にPOSTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IUser.CreateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.CreateAsync)} > POST /users をコールする。")]
        [Category(nameof(HttpMethod.Post))]
        public async Task User_CreateAsync_Calls_PostApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "id": 1,
              "email": "user1@example.com",
              "member_code": "00001",
              "role": {
                "id": 1,
                "name": "システム管理者",
                "type": "Adm"
              },
              "is_active": true,
              "password_locked": false,
              "use_smartphone": false
            }
            """;
            var payload = new UserPayload("user1@example.com", "00001", "password", 1);

            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPost("/users").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var user = await sut.User.CreateAsync(payload, cancellationToken);

            // Assert
            await Assert.That(user).IsNotNull();
            client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/users"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals("""
            {
              "email": "user1@example.com",
              "member_code": "00001",
              "password": "password",
              "role": { "id": 1 },
              "is_active": true,
              "password_locked": false,
              "use_smartphone": false
            }
            """);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IUser.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.User.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IUser.ReadAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.ReadAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task When_Id_IsNegative_User_ReadAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => _ = await sut.User.ReadAsync(-1, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.User.ReadAsync"/>は、"/users/{userId}"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IUser.ReadAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.ReadAsync)} > GET /users/:userId をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task User_ReadAsync_Calls_GetApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int userId = 1;
            /*lang=json,strict*/
            const string responseJson = """
            {
              "id": 1,
              "email": "user1@example.com",
              "member_code": "00001",
              "role": {
                "id": 1,
                "name": "システム管理者",
                "type": "Adm"
              },
              "is_active": true,
              "password_locked": false,
              "use_smartphone": false,
              "last_login_at": "2021-11-01 12:00:00"
            }
            """;

            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet($"/users/{userId}").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var user = await sut.User.ReadAsync(userId, cancellationToken);

            // Assert
            await Assert.That(user).IsNotNull();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path($"/users/{userId}"), Times.Once);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IUser.UpdateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.User.UpdateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IUser.UpdateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.UpdateAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [Category(nameof(HttpMethod.Patch))]
        public async Task When_Id_IsNegative_User_UpdateAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => _ = await sut.User.UpdateAsync(-1, null!, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.User.UpdateAsync"/>は、"/users/{userId}"にPATCHリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IUser.UpdateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.UpdateAsync)} > PATCH /users/:userId をコールする。")]
        [Category(nameof(HttpMethod.Patch))]
        public async Task User_UpdateAsync_Calls_PatchApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int userId = 1;
            /*lang=json,strict*/
            const string responseJson = """
            {
              "id": 1,
              "email": "user1@example.com",
              "member_code": "00001",
              "role": {
                "id": 1,
                "name": "システム管理者",
                "type": "Adm"
              },
              "is_active": true,
              "password_locked": false,
              "use_smartphone": false
            }
            """;
            var payload = new UserPayload("user1@example.com", "00001", "password", 1);

            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnRequest(req => req.Method(HttpMethod.Patch).Path($"/users/{userId}")).RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var user = await sut.User.UpdateAsync(userId, payload, cancellationToken);

            // Assert
            await Assert.That(user).IsNotNull();
            client.Handler.Verify(r => r.Method(HttpMethod.Patch).Path($"/users/{userId}"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsValidJsonObject().And.IsJsonEquals("""
            {
              "email": "user1@example.com",
              "member_code": "00001",
              "password": "password",
              "role": { "id": 1 },
              "is_active": true,
              "password_locked": false,
              "use_smartphone": false
            }
            """);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IUser.DeleteAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.User.DeleteAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IUser.DeleteAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.DeleteAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [Category(nameof(HttpMethod.Delete))]
        public async Task When_Id_IsNegative_User_DeleteAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => await sut.User.DeleteAsync(-1, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.User.DeleteAsync"/>は、"/users/{userId}"にDELETEリクエストを行う。
        /// </summary>
        [Test($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.DeleteAsync)} > DELETE /users/:userId をコールする。")]
        [Category(nameof(HttpMethod.Delete))]
        public async Task User_DeleteAsync_Calls_DeleteApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int userId = 1;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnDelete($"/users/{userId}").RespondWithString("", HttpStatusCode.NoContent);

            // Act
            var sut = CreateSut(client, "token");
            await sut.User.DeleteAsync(userId, cancellationToken);

            // Assert
            client.Handler.Verify(r => r.Method(HttpMethod.Delete).Path($"/users/{userId}"), Times.Once);
        }
    }
}
