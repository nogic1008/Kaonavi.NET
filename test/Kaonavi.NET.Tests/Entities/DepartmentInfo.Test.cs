using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// <see cref="DepartmentInfo"/>の単体テスト
    /// </summary>
    public class DepartmentInfoTest
    {
        private const string SimpleJson = "{\"code\": \"1000\",\"name\": \"取締役会\",\"parent_code\": null,\"leader_member_code\": \"A0002\",\"order\": 1,\"memo\": \"\"}";
        private const string NoLeaderJson = "{\"code\": \"1200\",\"name\": \"営業本部\",\"parent_code\": null,\"leader_member_code\": null,\"order\": 2,\"memo\": null}";
        private const string ChildJson = "{\"code\": \"2000\",\"name\": \"ITグループ\",\"parent_code\": \"1500\",\"leader_member_code\": \"A0001\",\"order\": 1,\"memo\": \"example\"}";

        /// <summary>
        /// JSONからデシリアライズできる。
        /// </summary>
        /// <param name="json">JSON文字列</param>
        /// <param name="code"><see cref="DepartmentInfo.Code"/></param>
        /// <param name="name"><see cref="DepartmentInfo.Name"/></param>
        /// <param name="parentCode"><see cref="DepartmentInfo.ParentCode"/></param>
        /// <param name="leaderMemberCode"><see cref="DepartmentInfo.LeaderMemberCode"/></param>
        /// <param name="order"><see cref="DepartmentInfo.Order"/></param>
        /// <param name="memo"><see cref="DepartmentInfo.Memo"/></param>
        [Theory(DisplayName = nameof(DepartmentInfo) + " > JSONからデシリアライズできる。")]
        [InlineData(SimpleJson, "1000", "取締役会", null, "A0002", 1, "")]
        [InlineData(NoLeaderJson, "1200", "営業本部", null, null, 2, null)]
        [InlineData(ChildJson, "2000", "ITグループ", "1500", "A0001", 1, "example")]
        public void CanDeserializeJSON(string json, string code, string name, string? parentCode, string? leaderMemberCode, int order, string? memo)
        {
            // Arrange - Act
            var department = JsonSerializer.Deserialize<DepartmentInfo>(json);

            // Assert
            department.Should().Be(new DepartmentInfo(code, name, parentCode, leaderMemberCode, order, memo));
        }
    }
}
