using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Task"/>の単体テスト</summary>
    [TestClass]
    public sealed class TaskTest
    {
        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ITask.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Task.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Task)}.{nameof(KaonaviClient.Task.ReadAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("タスク進捗状況")]
        public async ValueTask When_Id_IsNegative_Task_ReadAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.Task.ReadAsync(-1);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Task.ReadAsync"/>は、"/tasks/{taskId}"にGETリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Task)}.{nameof(KaonaviClient.Task.ReadAsync)} > GET /tasks/:taskId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("タスク進捗状況")]
        public async ValueTask Task_ReadAsync_Calls_GetApi()
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

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/tasks/{taskId}")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, "token");
            var task = await sut.Task.ReadAsync(taskId);

            // Assert
            task.ShouldNotBeNull();
            handler.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/tasks/{taskId}")
            );
        }
    }
}
