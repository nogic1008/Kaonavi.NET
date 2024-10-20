using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Department"/>の単体テスト</summary>
    [TestClass]
    public class DepartmentTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Department.ListAsync"/>は、"/departments"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Department)}.{nameof(KaonaviClient.Department.ListAsync)} > GET /departments をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("所属ツリー")]
        public async Task Department_ListAsync_Calls_GetApi()
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "department_data": [
                {
                  "code": "1000",
                  "name": "取締役会",
                  "parent_code": null,
                  "leader_member_code": "A0002",
                  "order": 1,
                  "memo": ""
                },
                {
                  "code": "1200",
                  "name": "営業本部",
                  "parent_code": null,
                  "leader_member_code": null,
                  "order": 2,
                  "memo": ""
                },
                {
                  "code": "1500",
                  "name": "第一営業部",
                  "parent_code": "1200",
                  "leader_member_code": null,
                  "order": 1,
                  "memo": ""
                },
                {
                  "code": "2000",
                  "name": "ITグループ",
                  "parent_code": "1500",
                  "leader_member_code": "A0001",
                  "order": 1,
                  "memo": "example"
                }
              ]
            }
            """;
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/departments")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var departments = await sut.Department.ListAsync();

            // Assert
            _ = departments.Should().HaveCount(4)
                .And.AllBeAssignableTo<DepartmentTree>();

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, "/departments")
                    .And.HasToken(tokenString);
                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Department.ReplaceAsync"/>は、"/departments"にPUTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Department)}.{nameof(KaonaviClient.Department.ReplaceAsync)} > PUT /departments をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("所属ツリー")]
        public async Task Department_ReplaceAsync_Calls_PutApi()
        {
            // Arrange
            var payload = new DepartmentTree[]
            {
                new("1000", "取締役会", null, "A0002", 1, ""),
                new("1200", "営業本部", null, null, 2, ""),
                new("1500", "第一営業部", "1200", null, 1, ""),
                new("2000", "ITグループ", "1500", "A0001", 1, "example"),
            };
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"department_data\":{JsonSerializer.Serialize(payload, Context.Default.IReadOnlyListDepartmentTree)}}}";

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/departments")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.Department.ReplaceAsync(payload);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(async (req) =>
            {
                _ = req.Should().SendTo(HttpMethod.Put, "/departments")
                    .And.HasToken(tokenString);

                // Body
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = receivedJson.Should().Be(expectedJson);

                return true;
            }, Times.Once());
        }
    }
}