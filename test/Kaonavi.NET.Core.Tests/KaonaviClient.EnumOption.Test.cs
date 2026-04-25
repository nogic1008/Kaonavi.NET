using Kaonavi.Net.Tests.Assertions;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.EnumOption"/>の単体テスト</summary>
    [Category("API"), Category("マスター管理")]
    public sealed class EnumOptionTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.EnumOption.ListAsync"/>は、"/enum_options"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IEnumOption.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.ListAsync)} > GET /enum_options をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task EnumOption_ListAsync_Calls_GetApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "custom_field_data": [
                {
                  "sheet_name": "役職情報",
                  "id": 10,
                  "name": "役職",
                  "enum_option_data": [
                    { "id": 1, "name": "社長" },
                    { "id": 2, "name": "部長" },
                    { "id": 3, "name": "課長" }
                  ]
                },
                {
                  "sheet_name": "家族情報",
                  "id": 20,
                  "name": "続柄区分",
                  "enum_option_data": [
                    { "id": 4, "name": "父" },
                    { "id": 5, "name": "母" },
                    { "id": 6, "name": "兄" },
                    { "id": 7, "name": "姉" }
                  ]
                },
                {
                  "sheet_name": "学歴情報",
                  "id": 30,
                  "name": "学歴区分",
                  "enum_option_data": [
                    { "id": 8, "name": "高校" },
                    { "id": 9, "name": "大学" },
                    { "id": 10, "name": "大学院" }
                  ]
                }
              ]
            }
            """;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet("/enum_options").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var entities = await sut.EnumOption.ListAsync(cancellationToken);

            // Assert
            await Assert.That(entities).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path("/enum_options"), Times.Once);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IEnumOption.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.EnumOption.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IEnumOption.ReadAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.ReadAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task When_Id_IsNegative_EnumOption_ReadAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => await sut.EnumOption.ReadAsync(-1, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.EnumOption.ReadAsync"/>は、"/enum_options/{id}"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IEnumOption.ReadAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.ReadAsync)} > GET /enum_options/:id をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task EnumOption_ReadAsync_Calls_GetApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "sheet_name": "役職情報",
              "id": 10,
              "name": "役職",
              "enum_option_data": [
                { "id": 1, "name": "社長" },
                { "id": 2, "name": "部長" },
                { "id": 3, "name": "課長" }
              ]
            }
            """;
            const int id = 10;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet($"/enum_options/{id}").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var entity = await sut.EnumOption.ReadAsync(id, cancellationToken);

            // Assert
            await Assert.That(entity).IsNotNull();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path($"/enum_options/{id}"), Times.Once);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IEnumOption.UpdateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.EnumOption.UpdateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IEnumOption.UpdateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.UpdateAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [Category(nameof(HttpMethod.Put))]
        public async Task When_Id_IsNegative_EnumOption_UpdateAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => await sut.EnumOption.UpdateAsync(-1, [], cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.EnumOption.UpdateAsync"/>は、"/enum_options/{id}"にPUTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IEnumOption.UpdateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.UpdateAsync)} > PUT /enum_options/:id をコールする。")]
        [Category(nameof(HttpMethod.Put))]
        public async Task EnumOption_UpdateAsync_Calls_PutApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int id = 10;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPut($"/enum_options/{id}").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.EnumOption.UpdateAsync(id, [(1, "社長"), (null, "本部長"), (2, "マネージャー")], cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Put).Path($"/enum_options/{id}"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals("""
            {
              "enum_option_data": [
                { "id": 1, "name": "社長" },
                { "name": "本部長" },
                { "id": 2, "name": "マネージャー" }
              ]
            }
            """u8);
        }
    }
}
