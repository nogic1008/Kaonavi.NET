using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Role"/>の単体テスト</summary>
    [TestClass]
    public sealed class RoleTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Role.ListAsync"/>は、"/roles"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Role)}.{nameof(KaonaviClient.Role.ListAsync)} > GET /roles をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("ロール")]
        public async ValueTask Role_ListAsync_Calls_GetApi()
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "role_data": [
                {
                  "id": 1,
                  "name": "カオナビ管理者",
                  "type": "Adm"
                },
                {
                  "id": 2,
                  "name": "カオナビマネージャー",
                  "type": "一般"
                }
              ]
            }
            """;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/roles")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var roles = await sut.Role.ListAsync();

            // Assert
            roles.ShouldNotBeEmpty();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/roles")
            );
        }
    }
}
