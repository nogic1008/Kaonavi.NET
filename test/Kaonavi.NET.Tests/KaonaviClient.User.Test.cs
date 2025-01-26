using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.User"/>の単体テスト</summary>
    [TestClass]
    public sealed class UserTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.User.ListAsync"/>は、"/users"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.ListAsync)} > GET /users をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("ユーザー情報")]
        public async Task User_ListAsync_Calls_GetApi()
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/users")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var users = await sut.User.ListAsync();

            // Assert
            users.ShouldNotBeEmpty();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/users")
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.User.CreateAsync"/>は、"/users"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.CreateAsync)} > POST /users をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("ユーザー情報")]
        public async Task User_CreateAsync_Calls_PostApi()
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

            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/users")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var user = await sut.User.CreateAsync(payload);

            // Assert
            user.ShouldNotBeNull();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Post),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/users"),
                static req => req.Content!.ShouldHaveJsonBody("""
                {
                  "email": "user1@example.com",
                  "member_code": "00001",
                  "password": "password",
                  "role": { "id": 1 },
                  "is_active": true,
                  "password_locked": false,
                  "use_smartphone": false
                }
                """)
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IUser.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.User.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.ReadAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("ユーザー情報")]
        public async Task When_Id_IsNegative_User_ReadAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.User.ReadAsync(-1);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.User.ReadAsync"/>は、"/users/{userId}"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.ReadAsync)} > GET /users/:userId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("ユーザー情報")]
        public async Task User_ReadAsync_Calls_GetApi()
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

            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/users/{userId}")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var user = await sut.User.ReadAsync(userId);

            // Assert
            user.ShouldNotBeNull();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/users/{userId}")
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IUser.UpdateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.User.UpdateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.UpdateAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("ユーザー情報")]
        public async Task When_Id_IsNegative_User_UpdateAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.User.UpdateAsync(-1, null!);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.User.UpdateAsync"/>は、"/users/{userId}"にPATCHリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.UpdateAsync)} > PATCH /users/:userId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("ユーザー情報")]
        public async Task User_UpdateAsync_Calls_PatchApi()
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

            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/users/{userId}")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var user = await sut.User.UpdateAsync(userId, payload);

            // Assert
            user.ShouldNotBeNull();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Patch),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/users/{userId}"),
                static req => req.Content!.ShouldHaveJsonBody("""
                {
                  "email": "user1@example.com",
                  "member_code": "00001",
                  "password": "password",
                  "role": { "id": 1 },
                  "is_active": true,
                  "password_locked": false,
                  "use_smartphone": false
                }
                """)
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IUser.DeleteAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.User.DeleteAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.DeleteAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Delete)), TestCategory("ユーザー情報")]
        public async Task When_Id_IsNegative_User_DeleteAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => await sut.User.DeleteAsync(-1);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.User.DeleteAsync"/>は、"/users/{userId}"にDELETEリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.User)}.{nameof(KaonaviClient.User.DeleteAsync)} > DELETE /users/:userId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Delete)), TestCategory("ユーザー情報")]
        public async Task User_DeleteAsync_Calls_DeleteApi()
        {
            // Arrange
            const int userId = 1;
            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/users/{userId}")
                .ReturnsResponse(HttpStatusCode.NoContent);

            // Act
            var sut = CreateSut(handler, "token");
            await sut.User.DeleteAsync(userId);

            // Assert
            handler.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Delete),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/users/{userId}")
            );
        }
    }
}
