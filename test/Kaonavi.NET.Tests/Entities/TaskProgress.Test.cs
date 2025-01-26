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
    /// JSONから<see cref="TaskState"/>にデシリアライズできる。
    /// </summary>
    [TestMethod($"{nameof(TaskState)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    [DataRow("\"OK\"", TaskState.OK, DisplayName = $"\"OK\" -> {nameof(TaskState)}.{nameof(TaskState.OK)} にデシリアライズできる。")]
    [DataRow("\"NG\"", TaskState.NG, DisplayName = $"\"NG\" -> {nameof(TaskState)}.{nameof(TaskState.NG)} にデシリアライズできる。")]
    [DataRow("\"ERROR\"", TaskState.Error, DisplayName = $"\"ERROR\" -> {nameof(TaskState)}.{nameof(TaskState.Error)} にデシリアライズできる。")]
    [DataRow("\"WAITING\"", TaskState.Waiting, DisplayName = $"\"WAITING\" -> {nameof(TaskState)}.{nameof(TaskState.Waiting)} にデシリアライズできる。")]
    [DataRow("\"RUNNING\"", TaskState.Running, DisplayName = $"\"RUNNING\" -> {nameof(TaskState)}.{nameof(TaskState.Running)} にデシリアライズできる。")]
    public void FieldType_Can_Deserialize_FromJSON(string json, TaskState expected)
        => JsonSerializer.Deserialize(json, Context.Default.TaskState).ShouldBe(expected);

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></param>
    /// <param name="status"><inheritdoc cref="TaskProgress" path="/param[@name='Status']"/></param>
    /// <param name="messages"><inheritdoc cref="TaskProgress" path="/param[@name='Messages']"/></param>
    [TestMethod(TestName), TestCategory("JSON Deserialize")]
    [DataRow(TaskOkJson, 1, TaskState.OK, (string[])[], DisplayName = TestName)]
    [DataRow(TaskRunningJson, 2, TaskState.Running, null, DisplayName = TestName)]
    [DataRow(TaskErrorJson, 3, TaskState.NG, (string[])["エラーメッセージ1", "エラーメッセージ2"], DisplayName = TestName)]
    public void TaskProgress_Can_Deserialize_FromJSON(string json, int id, TaskState status, string[]? messages)
    {
        // Arrange - Act
        var taskProgress = JsonSerializer.Deserialize(json, Context.Default.TaskProgress);

        // Assert
        taskProgress!.ShouldSatisfyAllConditions(
            static sut => sut.ShouldNotBeNull(),
            sut => sut.Id.ShouldBe(id),
            sut => sut.Status.ShouldBe(status),
            sut => sut.Messages.ShouldBe(messages)
        );
    }
}
