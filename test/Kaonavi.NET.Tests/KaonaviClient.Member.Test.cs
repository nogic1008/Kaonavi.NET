using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Member"/>の単体テスト</summary>
    [TestClass]
    public class MemberTest
    {
        /// <summary>Member APIのリクエストPayload</summary>
        private static readonly MemberData[] _memberDataPayload =
        [
            new(
                Code: "A0002",
                Name: "カオナビ 太郎",
                NameKana: "カオナビ タロウ",
                Mail: "taro@example.com",
                EnteredDate: new(2005, 9, 20),
                Gender: "男性",
                Birthday: new(1984, 5, 15),
                Department: new("1000"),
                SubDepartments: [],
                CustomFields: [new(100, "A")]
            ),
            new(
                Code: "A0001",
                Name: "カオナビ 花子",
                NameKana: "カオナビ ハナコ",
                Mail: "hanako@kaonavi.jp",
                EnteredDate: new(2013, 5, 7),
                Gender: "女性",
                Birthday: new(1986, 5, 16),
                Department: new("2000"),
                SubDepartments: [new("3000"), new("4000")],
                CustomFields: [new(100, "O"), new(200, ["部長", "マネージャー"])]
            )
        ];

        /// <summary>
        /// <see cref="KaonaviClient.Member.ListAsync"/>は、"/members"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.ListAsync)} > GET /members をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("メンバー情報")]
        public async Task Member_ListAsync_Calls_GetApi()
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "updated_at": "2020-10-01 01:23:45",
              "member_data": [
                {
                  "code": "A0002",
                  "name": "カオナビ 太郎",
                  "name_kana": "カオナビ タロウ",
                  "mail": "taro@kaonavi.jp",
                  "entered_date": "2005-09-20",
                  "retired_date": "",
                  "gender": "男性",
                  "birthday": "1984-05-15",
                  "age": 36,
                  "years_of_service": "15年5ヵ月",
                  "department": {
                    "code": "1000",
                    "name": "取締役会",
                    "names": ["取締役会"]
                  },
                  "sub_departments": [],
                  "custom_fields": [
                    {
                      "id": 100,
                      "name": "血液型",
                      "values": ["A"]
                    }
                  ]
                },
                {
                  "code": "A0001",
                  "name": "カオナビ 花子",
                  "name_kana": "カオナビ ハナコ",
                  "mail": "hanako@kaonavi.jp",
                  "entered_date": "2013-05-07",
                  "retired_date": "",
                  "gender": "女性",
                  "birthday": "1986-05-16",
                  "age": 36,
                  "years_of_service": "7年9ヵ月",
                  "department": {
                    "code": "2000",
                    "name": "営業本部 第一営業部 ITグループ",
                    "names": ["営業本部", "第一営業部", "ITグループ"]
                  },
                  "sub_departments": [
                    {
                      "code": "3000",
                      "name": "企画部",
                      "names": ["企画部"]
                    },
                    {
                      "code": "4000",
                      "name": "管理部",
                      "names": ["管理部"]
                    }
                  ],
                  "custom_fields": [
                    {
                      "id": 100,
                      "name": "血液型",
                      "values": ["O"]
                    },
                    {
                      "id": 200,
                      "name": "役職",
                      "values": ["部長", "マネージャー"]
                    }
                  ]
                }
              ]
            }
            """;
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var members = await sut.Member.ListAsync();

            // Assert
            _ = members.Should().HaveCount(2);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, "/members")
                    .And.HasToken(tokenString);
                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.CreateAsync"/>は、"/members"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.CreateAsync)} > POST /members をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("メンバー情報")]
        public async Task Member_CreateAsync_Calls_PostApi()
        {
            // Arrange
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, Context.Default.IReadOnlyListMemberData)}}}";

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.Member.CreateAsync(_memberDataPayload);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(async req =>
            {
                _ = req.Should().SendTo(HttpMethod.Post, "/members")
                    .And.HasToken(tokenString);

                // Body
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = receivedJson.Should().Be(expectedJson);

                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.ReplaceAsync"/>は、"/members"にPUTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.ReplaceAsync)} > PUT /members をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("メンバー情報")]
        public async Task Member_ReplaceAsync_Calls_PutApi()
        {
            // Arrange
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, Context.Default.IReadOnlyListMemberData)}}}";

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.Member.ReplaceAsync(_memberDataPayload);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(async req =>
            {
                _ = req.Should().SendTo(HttpMethod.Put, "/members")
                    .And.HasToken(tokenString);

                // Body
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = receivedJson.Should().Be(expectedJson);

                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.UpdateAsync"/>は、"/members"にPATCHリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.UpdateAsync)} > PATCH /members をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("メンバー情報")]
        public async Task Member_UpdateAsync_Calls_PatchApi()
        {
            // Arrange
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, Context.Default.IReadOnlyListMemberData)}}}";

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.Member.UpdateAsync(_memberDataPayload);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(async req =>
            {
                _ = req.Should().SendTo(HttpMethod.Patch, "/members")
                    .And.HasToken(tokenString);

                // Body
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = receivedJson.Should().Be(expectedJson);

                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.OverWriteAsync"/>は、"/members/overwrite"にPUTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.OverWriteAsync)} > PUT /members/overwrite をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("メンバー情報")]
        public async Task Member_OverWriteAsync_Calls_PutApi()
        {
            // Arrange
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, Context.Default.IReadOnlyListMemberData)}}}";

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/overwrite")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.Member.OverWriteAsync(_memberDataPayload);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(async req =>
            {
                _ = req.Should().SendTo(HttpMethod.Put, "/members/overwrite")
                    .And.HasToken(tokenString);

                // Body
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = receivedJson.Should().Be(expectedJson);

                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.DeleteAsync"/>は、"/members/delete"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.DeleteAsync)} > POST /members/delete をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("メンバー情報")]
        public async Task Member_DeleteAsync_Calls_PostApi()
        {
            // Arrange
            string[] codes = _memberDataPayload.Select(d => d.Code).ToArray();
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"codes\":{JsonSerializer.Serialize(codes, Context.Default.IReadOnlyListString)}}}";

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/delete")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.Member.DeleteAsync(codes);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(async req =>
            {
                _ = req.Should().SendTo(HttpMethod.Post, "/members/delete")
                    .And.HasToken(tokenString);

                // Body
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = receivedJson.Should().Be(expectedJson);

                return true;
            }, Times.Once());
        }
    }
}
