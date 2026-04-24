namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Task"/>の単体テスト</summary>
    [Category("API"), Category("タスク進捗状況")]
    public sealed class TaskTest
    {
        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ITask.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Task.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ITask.ReadAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Task)}.{nameof(KaonaviClient.Task.ReadAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task When_Id_IsNegative_Task_ReadAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => _ = await sut.Task.ReadAsync(-1, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Task.ReadAsync"/>は、"/tasks/{taskId}"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ITask.ReadAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Task)}.{nameof(KaonaviClient.Task.ReadAsync)} > GET /tasks/:taskId をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task Task_ReadAsync_Calls_GetApi(CancellationToken cancellationToken = default)
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

            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet($"/tasks/{taskId}").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var task = await sut.Task.ReadAsync(taskId, cancellationToken);

            // Assert
            await Assert.That(task).IsNotNull();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path($"/tasks/{taskId}"), Times.Once);
        }
    }
}

