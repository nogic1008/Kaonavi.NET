using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="TaskProgress"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class TaskProgressTest
{
    /*lang=json,strict*/
    private const string TaskOkJson = """{ "id": 1, "status": "OK", "messages": [] }""";
    /*lang=json,strict*/
    private const string TaskRunningJson = """{ "id": 2, "status": "RUNNING" }""";
    /*lang=json,strict*/
    private const string TaskErrorJson = """
    { "id": 3, "status": "NG", "messages": ["エラーメッセージ1","エラーメッセージ2"] }
    """;

    private const string TestName = $"{nameof(TaskProgress)} > JSONからデシリアライズできる。";

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></param>
    /// <param name="status"><inheritdoc cref="TaskProgress" path="/param[@name='Status']"/></param>
    /// <param name="messages"><inheritdoc cref="TaskProgress" path="/param[@name='Messages']"/></param>
    [TestMethod(TestName), TestCategory("JSON Deserialize")]
    [DataRow(TaskOkJson, 1, "OK", new string[0], DisplayName = TestName)]
    [DataRow(TaskRunningJson, 2, "RUNNING", null, DisplayName = TestName)]
    [DataRow(TaskErrorJson, 3, "NG", new[] { "エラーメッセージ1", "エラーメッセージ2" }, DisplayName = TestName)]
    public void CanDeserializeJSON(string json, int id, string status, string[]? messages)
    {
        // Arrange - Act
        var task = JsonSerializer.Deserialize(json, Context.Default.TaskProgress);

        // Assert
        _ = task.Should().NotBeNull();
        _ = task!.Id.Should().Be(id);
        _ = task.Status.Should().Be(status);
        _ = task.Messages.Should().Equal(messages);
    }
}
