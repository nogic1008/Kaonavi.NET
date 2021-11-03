using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// <see cref="TaskProgress"/>の単体テスト
    /// </summary>
    public class TaskProgressTest
    {
        private const string TaskOkJson = "{\"id\": 1,\"status\": \"OK\",\"messages\": []}";
        private const string TaskRunningJson = "{\"id\": 2,\"status\": \"RUNNING\"}";
        private const string TaskErrorJson = "{\"id\": 3,\"status\": \"NG\",\"messages\": [\"エラーメッセージ1\",\"エラーメッセージ2\"]}";

        /// <summary>
        /// JSONからデシリアライズできる。
        /// </summary>
        /// <param name="json">JSON文字列</param>
        /// <param name="id"><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></param>
        /// <param name="status"><inheritdoc cref="TaskProgress" path="/param[@name='Status']/text()"/></param>
        /// <param name="messages"><inheritdoc cref="TaskProgress" path="/param[@name='Messages']/text()"/></param>
        [Theory(DisplayName = nameof(TaskProgress) + " > JSONからデシリアライズできる。")]
        [InlineData(TaskOkJson, 1, "OK")]
        [InlineData(TaskRunningJson, 2, "RUNNING", null)]
        [InlineData(TaskErrorJson, 3, "NG", "エラーメッセージ1", "エラーメッセージ2")]
        public void CanDeserializeJSON(string json, int id, string status, params string[] messages)
        {
            // Arrange - Act
            var task = JsonSerializer.Deserialize<TaskProgress>(json);

            // Assert
            task.Should().NotBeNull();
            task!.Id.Should().Be(id);
            task.Status.Should().Be(status);
            task.Messages.Should().Equal(messages);
        }
    }
}
