using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;
using Moq;
using Moq.Contrib.HttpClient;

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
            _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            var act = async () => _ = await sut.Task.ReadAsync(-1);

            // Assert
            _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithParameterName("id");

            handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
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
            string tokenString = GenerateRandomString();
            var response = new TaskProgress(taskId, "NG", ["エラーメッセージ1", "エラーメッセージ2"]);

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/tasks/{taskId}")
                .ReturnsJsonResponse(HttpStatusCode.OK, response, Context.Default.Options);

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var task = await sut.Task.ReadAsync(taskId);

            // Assert
            _ = task.Should().NotBeNull();
            _ = task.Id.Should().Be(taskId);
            _ = task.Status.Should().Be("NG");
            _ = task.Messages.Should().Equal("エラーメッセージ1", "エラーメッセージ2");

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, $"/tasks/{taskId}")
                    .And.HasToken(tokenString);
                return true;
            }, Times.Once());
        }
    }
}
