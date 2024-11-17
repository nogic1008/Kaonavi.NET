using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;
using RandomFixtureKit;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.EnumOption"/>の単体テスト</summary>
    [TestClass]
    public class EnumOptionTest
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
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/enum_options")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            var entities = await sut.EnumOption.ListAsync();

            // Assert
            _ = entities.Should().HaveCount(3)
                .And.AllBeAssignableTo<EnumOption>();

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, "/enum_options")
                    .And.HasToken(token);
                return true;
            }, Times.Once());
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
            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            var act = async () => await sut.EnumOption.ReadAsync(-1);

            // Assert
            _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithParameterName("id");

            handler.VerifyAnyRequest(Times.Never());
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
            int id = JsonDocument.Parse(responseJson).RootElement.GetProperty("id"u8).GetInt32();
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/enum_options/{id}")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            var entity = await sut.EnumOption.ReadAsync(id);

            // Assert
            _ = entity.Should().NotBeNull();

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, $"/enum_options/{id}")
                    .And.HasToken(token);
                return true;
            }, Times.Once());
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
            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            var act = async () => await sut.EnumOption.UpdateAsync(-1, []);

            // Assert
            _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithParameterName("id");

            handler.VerifyAnyRequest(Times.Never());
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
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/enum_options/{id}")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            int taskId = await sut.EnumOption.UpdateAsync(id, [(1, "社長"), (null, "本部長"), (2, "マネージャー")]);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(async req =>
            {
                _ = req.Should().SendTo(HttpMethod.Put, $"/enum_options/{id}")
                    .And.HasToken(token);

                // Body
                using var doc = JsonDocument.Parse(req.Content!.ReadAsStream());
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = doc.RootElement.Should()
                    .BeSameJson("""
                    {
                      "enum_option_data": [
                        { "id": 1, "name": "社長" },
                        { "name": "本部長" },
                        { "id": 2, "name": "マネージャー" }
                      ]
                    }
                    """);

                return true;
            }, Times.Once());
        }
    }
}
