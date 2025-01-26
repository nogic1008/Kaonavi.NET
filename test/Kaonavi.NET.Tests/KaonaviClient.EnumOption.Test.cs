using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.EnumOption"/>の単体テスト</summary>
    [TestClass]
    public sealed class EnumOptionTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.EnumOption.ListAsync"/>は、"/enum_options"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.ListAsync)} > GET /enum_options をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("マスター管理")]
        public async Task EnumOption_ListAsync_Calls_GetApi()
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/enum_options")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var entities = await sut.EnumOption.ListAsync();

            // Assert
            entities.ShouldNotBeEmpty();

            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/enum_options")
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IEnumOption.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.EnumOption.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.ReadAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("マスター管理")]
        public async Task When_Id_IsNegative_EnumOption_ReadAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => await sut.EnumOption.ReadAsync(-1);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");

            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.EnumOption.ReadAsync"/>は、"/enum_options/{id}"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.ReadAsync)} > GET /enum_options/:id をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("マスター管理")]
        public async Task EnumOption_ReadAsync_Calls_GetApi()
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/enum_options/{id}")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var entity = await sut.EnumOption.ReadAsync(id);

            // Assert
            entity.ShouldNotBeNull();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/enum_options/{id}")
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IEnumOption.UpdateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.EnumOption.UpdateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.UpdateAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("マスター管理")]
        public async Task When_Id_IsNegative_EnumOption_UpdateAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => await sut.EnumOption.UpdateAsync(-1, []);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.EnumOption.UpdateAsync"/>は、"/enum_options/{id}"にPUTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.EnumOption)}.{nameof(KaonaviClient.EnumOption.UpdateAsync)} > PUT /enum_options/:id をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("マスター管理")]
        public async Task EnumOption_UpdateAsync_Calls_PutApi()
        {
            // Arrange
            const int id = 10;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/enum_options/{id}")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.EnumOption.UpdateAsync(id, [(1, "社長"), (null, "本部長"), (2, "マネージャー")]);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Put),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/enum_options/{id}"),
                static req => req.Content!.ShouldHaveJsonBody("""
                {
                  "enum_option_data": [
                    { "id": 1, "name": "社長" },
                    { "name": "本部長" },
                    { "id": 2, "name": "マネージャー" }
                  ]
                }
                """)
            );
        }
    }
}
