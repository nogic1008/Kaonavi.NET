using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="TaskProgress"/>の単体テスト</summary>
[Category("Entities")]
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

    /// <summary>
    /// JSONから<see cref="TaskState"/>にデシリアライズできる。
    /// </summary>
    [Test($"{nameof(TaskState)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    [Arguments("\"OK\"", TaskState.OK, DisplayName = $"OK -> {nameof(TaskState)}.{nameof(TaskState.OK)} にデシリアライズできる。")]
    [Arguments("\"NG\"", TaskState.NG, DisplayName = $"NG -> {nameof(TaskState)}.{nameof(TaskState.NG)} にデシリアライズできる。")]
    [Arguments("\"ERROR\"", TaskState.Error, DisplayName = $"ERROR -> {nameof(TaskState)}.{nameof(TaskState.Error)} にデシリアライズできる。")]
    [Arguments("\"WAITING\"", TaskState.Waiting, DisplayName = $"WAITING -> {nameof(TaskState)}.{nameof(TaskState.Waiting)} にデシリアライズできる。")]
    [Arguments("\"RUNNING\"", TaskState.Running, DisplayName = $"RUNNING -> {nameof(TaskState)}.{nameof(TaskState.Running)} にデシリアライズできる。")]
    public async Task FieldType_Can_Deserialize_FromJSON(string json, TaskState expected)
    {
        var result = JsonSerializer.Deserialize(json, JsonContext.Default.TaskState);
        await Assert.That(result).IsEqualTo(expected);
    }

    private const string TestName = $"{nameof(TaskProgress)} > JSONからデシリアライズできる。";

    /// <summary><see cref="TaskProgress_Can_Deserialize_FromJSON"/>のテストデータ</summary>
    public static IEnumerable<TestDataRow<(string json, int id, TaskState status, string[]? messages)>> TestData
    {
        get
        {
            yield return new((/*lang=json,strict*/ """
            {
              "id": 1,
              "status": "OK",
              "messages": []
            }
            """, 1, TaskState.OK, [])) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
            {
              "id": 2,
              "status": "RUNNING"
            }
            """, 2, TaskState.Running, null)) { DisplayName = TestName };
            yield return new((/*lang=json,strict*/ """
            {
              "id": 3,
              "status": "NG",
              "messages": ["エラーメッセージ1", "エラーメッセージ2"]
            }
            """, 3, TaskState.NG, ["エラーメッセージ1", "エラーメッセージ2"])) { DisplayName = TestName };
        }
    }

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></param>
    /// <param name="status"><inheritdoc cref="TaskProgress" path="/param[@name='Status']"/></param>
    /// <param name="messages"><inheritdoc cref="TaskProgress" path="/param[@name='Messages']"/></param>
    [Test(TestName)]
    [Category("JSON Deserialize")]
    [MethodDataSource(nameof(TestData))]
    public async Task TaskProgress_Can_Deserialize_FromJSON([StringSyntax(StringSyntaxAttribute.Json)] string json, int id, TaskState status, string[]? messages)
    {
        // Arrange - Act
        var taskProgress = JsonSerializer.Deserialize(json, JsonContext.Default.TaskProgress);

        // Assert
        await Assert.That(taskProgress).IsNotNull()
            .And.Member(sut => sut.Id, o => o.IsEqualTo(id))
            .And.Member(sut => sut.Status, o => o.IsEqualTo(status))
            .And.Member(sut => sut.Messages!, o => messages is null ? o.IsNull() : o.IsEquivalentTo(messages));
    }
}
