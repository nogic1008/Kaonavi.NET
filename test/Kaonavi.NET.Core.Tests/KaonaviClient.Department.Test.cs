using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Department"/>の単体テスト</summary>
    [Category("API"), Category("所属ツリー")]
    public sealed class DepartmentTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Department.ListAsync"/>は、"/departments"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IDepartment.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Department)}.{nameof(KaonaviClient.Department.ListAsync)} > GET /departments をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task Department_ListAsync_Calls_GetApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "department_data": [
                { "code": "1000", "name": "取締役会", "parent_code": null, "leader_member_code": "A0002", "order": 1, "memo": "" },
                { "code": "1200", "name": "営業本部", "parent_code": null, "leader_member_code": null, "order": 2, "memo": "" },
                { "code": "1500", "name": "第一営業部", "parent_code": "1200", "leader_member_code": null, "order": 1, "memo": "" },
                { "code": "2000", "name": "ITグループ", "parent_code": "1500", "leader_member_code": "A0001", "order": 1, "memo": "example" }
              ]
            }
            """;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet("/departments").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var departments = await sut.Department.ListAsync(cancellationToken);

            // Assert
            await Assert.That(departments).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path("/departments"), Times.Once);
        }

        /// <summary>
        /// <see cref="KaonaviClient.Department.ReplaceAsync"/>は、"/departments"にPUTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IDepartment.ReplaceAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Department)}.{nameof(KaonaviClient.Department.ReplaceAsync)} > PUT /departments をコールする。")]
        [Category(nameof(HttpMethod.Put))]
        public async Task Department_ReplaceAsync_Calls_PutApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            var payload = new DepartmentTree[]
            {
                new("1000", "取締役会", null, "A0002", 1, ""),
                new("1200", "営業本部", null, null, 2, ""),
                new("1500", "第一営業部", "1200", null, 1, ""),
                new("2000", "ITグループ", "1500", "A0001", 1, "example"),
            };
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPut("/departments").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Department.ReplaceAsync(payload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Put).Path("/departments"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals("""
            {
              "department_data": [
                { "code": "1000", "name": "取締役会", "leader_member_code": "A0002", "order": 1, "memo": "" },
                { "code": "1200", "name": "営業本部", "order": 2, "memo": "" },
                { "code": "1500", "name": "第一営業部", "parent_code": "1200", "order": 1, "memo": "" },
                { "code": "2000", "name": "ITグループ", "parent_code": "1500", "leader_member_code": "A0001", "order": 1, "memo": "example" }
              ]
            }
            """u8);
        }
    }
}
