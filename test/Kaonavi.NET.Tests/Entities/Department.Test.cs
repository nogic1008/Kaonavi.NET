using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// Unit test for <see cref="Department"/>
    /// </summary>
    public class DepartmentTest
    {
        [Theory]
        [InlineData(
            "{\"code\":\"所属コード\"}",
            "所属コード", null, null
        )]
        [InlineData(
            "{\"code\": \"1000\",\"name\":\"取締役会\",\"names\":[\"取締役会\"]}",
            "1000", "取締役会", "取締役会"
        )]
        [InlineData(
            "{\"code\":\"2000\",\"name\":\"営業本部 第一営業部 ITグループ\",\"names\":[\"営業本部\",\"第一営業部\",\"ITグループ\"]}",
            "2000", "営業本部 第一営業部 ITグループ", "営業本部,第一営業部,ITグループ"
        )]
        public void CanDeserializeJSON(string json, string code, string? name, string? names)
        {
            // Arrange - Act
            var department = JsonSerializer.Deserialize<Department>(json);

            // Assert
            department.Should().NotBeNull();
            department!.Code.Should().Be(code);
            department.Name.Should().Be(name);
            if (names is null)
            {
                department.Names.Should().BeNull();
            }
            else
            {
                department.Names.Should().NotBeNull();
                string.Join(",", department.Names!).Should().Be(names);
            }
        }
    }
}
