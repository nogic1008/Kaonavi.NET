using Moq;
using Moq.Contrib.HttpClient;
using RandomFixtureKit;

using Kaonavi.Net.Tests.Assertions;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Task"/>の単体テスト</summary>
    [TestClass]
    public class TaskTest
    {
        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ITask.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Task.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Task)}.{nameof(KaonaviClient.Task.ReadAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("タスク進捗状況")]
        public async Task When_Id_IsNegative_Task_ReadAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            var act = async () => _ = await sut.Task.ReadAsync(-1);

            // Assert
            _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithParameterName("id");

            handler.VerifyAnyRequest(Times.Never());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Task.ReadAsync"/>は、"/tasks/{taskId}"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Task)}.{nameof(KaonaviClient.Task.ReadAsync)} > GET /tasks/:taskId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("タスク進捗状況")]
        public async Task Task_ReadAsync_Calls_GetApi()
        {
            // Arrange
            const int taskId = 1;
            /*lang=json,strict*/
            const string responseJson = """
            {
              "id": 1,
              "status": "NG",
              "messages": [
                "エラーメッセージ1",
                "エラーメッセージ2"
              ]
            }
            """;
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/tasks/{taskId}")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            var task = await sut.Task.ReadAsync(taskId);

            // Assert
            _ = task.Should().NotBeNull();

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, $"/tasks/{taskId}")
                    .And.HasToken(token);
                return true;
            }, Times.Once());
        }
    }
}
