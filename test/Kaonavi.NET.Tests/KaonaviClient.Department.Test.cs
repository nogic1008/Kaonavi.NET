using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Department"/>の単体テスト</summary>
    [TestClass]
    public sealed class DepartmentTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Department.ListAsync"/>は、"/departments"にGETリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Department)}.{nameof(KaonaviClient.Department.ListAsync)} > GET /departments をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("所属ツリー")]
        public async ValueTask Department_ListAsync_Calls_GetApi()
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/departments")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var departments = await sut.Department.ListAsync();

            // Assert
            departments.ShouldNotBeEmpty();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/departments")
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Department.ReplaceAsync"/>は、"/departments"にPUTリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Department)}.{nameof(KaonaviClient.Department.ReplaceAsync)} > PUT /departments をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("所属ツリー")]
        public async ValueTask Department_ReplaceAsync_Calls_PutApi()
        {
            // Arrange
            var payload = new DepartmentTree[]
            {
                new("1000", "取締役会", null, "A0002", 1, ""),
                new("1200", "営業本部", null, null, 2, ""),
                new("1500", "第一営業部", "1200", null, 1, ""),
                new("2000", "ITグループ", "1500", "A0001", 1, "example"),
            };
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/departments")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Department.ReplaceAsync(payload);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Put),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/departments"),
                static async req => await req.Content!.ShouldHaveJsonBodyAsync("""
                {
                  "department_data": [
                    {
                      "code": "1000",
                      "name": "取締役会",
                      "leader_member_code": "A0002",
                      "order": 1,
                      "memo": ""
                    },
                    {
                      "code": "1200",
                      "name": "営業本部",
                      "order": 2,
                      "memo": ""
                    },
                    {
                      "code": "1500",
                      "name": "第一営業部",
                      "parent_code": "1200",
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
                """)
            );
        }
    }
}
