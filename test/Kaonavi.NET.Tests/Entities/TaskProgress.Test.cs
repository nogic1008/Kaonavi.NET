using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="TaskProgress"/>の単体テスト
/// </summary>
public class TaskProgressTest
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
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></param>
    /// <param name="status"><inheritdoc cref="TaskProgress" path="/param[@name='Status']"/></param>
    /// <param name="messages"><inheritdoc cref="TaskProgress" path="/param[@name='Messages']"/></param>
    [Theory(DisplayName = $"{nameof(TaskProgress)} > JSONからデシリアライズできる。")]
    [InlineData(TaskOkJson, 1, "OK")]
    [InlineData(TaskRunningJson, 2, "RUNNING", null)]
    [InlineData(TaskErrorJson, 3, "NG", "エラーメッセージ1", "エラーメッセージ2")]
    public void CanDeserializeJSON(string json, int id, string status, params string[] messages)
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
