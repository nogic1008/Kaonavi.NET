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
    [Test, Category("JSON Deserialize")]
    [DisplayName($"{nameof(TaskState)} > $json から {nameof(TaskState)}.$expected にデシリアライズできる。")]
    [Arguments("\"OK\"", TaskState.OK)]
    [Arguments("\"NG\"", TaskState.NG)]
    [Arguments("\"ERROR\"", TaskState.Error)]
    [Arguments("\"WAITING\"", TaskState.Waiting)]
    [Arguments("\"RUNNING\"", TaskState.Running)]
    public async Task FieldType_Can_Deserialize_FromJSON(string json, TaskState expected)
        => await Assert.That(JsonSerializer.Deserialize(json, JsonContext.Default.TaskState))
            .IsEqualTo(expected);

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
            """, 1, TaskState.OK, []));
            yield return new((/*lang=json,strict*/ """
            {
              "id": 2,
              "status": "RUNNING"
            }
            """, 2, TaskState.Running, null));
            yield return new((/*lang=json,strict*/ """
            {
              "id": 3,
              "status": "NG",
              "messages": ["エラーメッセージ1", "エラーメッセージ2"]
            }
            """, 3, TaskState.NG, ["エラーメッセージ1", "エラーメッセージ2"]));
        }
    }

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></param>
    /// <param name="status"><inheritdoc cref="TaskProgress" path="/param[@name='Status']"/></param>
    /// <param name="messages"><inheritdoc cref="TaskProgress" path="/param[@name='Messages']"/></param>
    [Test, Category("JSON Deserialize")]
    [DisplayName($"{nameof(TaskProgress)} > $json からデシリアライズできる。")]
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
