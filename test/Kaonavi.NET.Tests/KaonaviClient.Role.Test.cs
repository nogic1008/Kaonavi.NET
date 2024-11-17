using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;
using RandomFixtureKit;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Role"/>の単体テスト</summary>
    [TestClass]
    public class RoleTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Role.ListAsync"/>は、"/roles"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Role)}.{nameof(KaonaviClient.Role.ListAsync)} > GET /roles をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("ロール")]
        public async Task Role_ListAsync_Calls_GetApi()
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
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/roles")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            var roles = await sut.Role.ListAsync();

            // Assert
            _ = roles.Should().HaveCount(2);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, "/roles")
                    .And.HasToken(token);
                return true;
            }, Times.Once());
        }
    }
}
