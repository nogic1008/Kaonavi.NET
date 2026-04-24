namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Role"/>の単体テスト</summary>
    [Category("API"), Category("ロール")]
    public sealed class RoleTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Role.ListAsync"/>は、"/roles"にGETリクエストを行う。
        /// </summary>
        [Test($"{nameof(KaonaviClient.Role)}.{nameof(KaonaviClient.Role.ListAsync)} > GET /roles をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task Role_ListAsync_Calls_GetApi(CancellationToken cancellationToken = default)
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
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet("/roles").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var roles = await sut.Role.ListAsync(cancellationToken);

            // Assert
            await Assert.That(roles).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path("/roles"), Times.Once);
        }
    }
}
