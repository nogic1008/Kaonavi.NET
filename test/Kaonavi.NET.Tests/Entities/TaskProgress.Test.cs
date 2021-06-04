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
        private const string TestName = nameof(TaskProgress) + " > ";

        /// <summary>
        /// JSONからデシリアライズできる。
        /// </summary>
        /// <param name="json">JSON文字列</param>
        /// <param name="id"><see cref="TaskProgress.Id"/></param>
        /// <param name="status"><see cref="TaskProgress.Status"/></param>
        /// <param name="messages"><see cref="TaskProgress.Messages"/>の文字列表現</param>
        [Theory(DisplayName = TestName + "JSONからデシリアライズできる。")]
        [InlineData("{\"id\": 1,\"status\": \"OK\",\"messages\": []}", 1, "OK", "")]
        [InlineData("{\"id\": 2,\"status\": \"RUNNING\"}", 2, "RUNNING", null)]
        [InlineData("{\"id\": 3,\"status\": \"NG\",\"messages\": [\"エラーメッセージ1\",\"エラーメッセージ2\"]}", 3, "NG", "エラーメッセージ1,エラーメッセージ2")]
        public void CanDeserializeJSON(string json, int id, string status, string? messages)
        {
            // Arrange - Act
            var task = JsonSerializer.Deserialize<TaskProgress>(json);

            // Assert
            task.Should().NotBeNull();
            task!.Id.Should().Be(id);
            task!.Status.Should().Be(status);
            if (messages is null)
                task.Messages.Should().BeNull();
            else
                string.Join(",", task.Messages!).Should().Be(messages);
        }
    }
}
