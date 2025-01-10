using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;
using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;
using RandomFixtureKit;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Member"/>の単体テスト</summary>
    [TestClass]
    public sealed class MemberTest
    {
        /// <summary><inheritdoc cref="KaonaviClient.Member" path="/summary/text()"/>のリクエストPayload</summary>
        /*lang=json,strict*/
        private const string PayloadJson = """
        [
          {
            "code": "A0002",
            "name": "カオナビ 太郎",
            "name_kana": "カオナビ タロウ",
            "mail": "taro@kaonavi.jp",
            "entered_date": "2005-09-20",
            "gender": "男性",
            "birthday": "1984-05-15",
            "department": { "code": "1000" },
            "sub_departments": [],
            "custom_fields": [
              { "id": 100, "values": ["A"] }
            ]
          },
          {
            "code": "A0001",
            "name": "カオナビ 花子",
            "name_kana": "カオナビ ハナコ",
            "mail": "hanako@kaonavi.jp",
            "entered_date": "2013-05-07",
            "gender": "女性",
            "birthday": "1986-05-16",
            "department": { "code": "2000" },
            "sub_departments": [
              { "code": "3000" },
              { "code": "4000" }
            ],
            "custom_fields": [
              { "id": 100, "values": ["O"] },
              { "id": 200, "values": ["部長", "マネージャー"] }
            ]
          }
        ]
        """;
        /// <summary><inheritdoc cref="KaonaviClient.Member" path="/summary/text()"/>のリクエストPayload</summary>
        private static readonly IReadOnlyList<MemberData> _payload = JsonSerializer.Deserialize(PayloadJson, Context.Default.IReadOnlyListMemberData)!;

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
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            var members = await sut.Member.ListAsync();

            // Assert
            _ = members.Should().HaveCount(2);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, "/members")
                    .And.HasToken(token);
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
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            int taskId = await sut.Member.CreateAsync(_payload);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Post, "/members")
                    .And.HasToken(token);

                // Body
                using var doc = JsonDocument.Parse(req.Content!.ReadAsStream());
                _ = doc.RootElement.GetProperty("member_data"u8).Should().BeSameJson(PayloadJson);

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
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            int taskId = await sut.Member.ReplaceAsync(_payload);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Put, "/members")
                    .And.HasToken(token);

                // Body
                using var doc = JsonDocument.Parse(req.Content!.ReadAsStream());
                _ = doc.RootElement.GetProperty("member_data"u8).Should().BeSameJson(PayloadJson);

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
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            int taskId = await sut.Member.UpdateAsync(_payload);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Patch, "/members")
                    .And.HasToken(token);

                // Body
                using var doc = JsonDocument.Parse(req.Content!.ReadAsStream());
                _ = doc.RootElement.GetProperty("member_data"u8).Should().BeSameJson(PayloadJson);

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
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/overwrite")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            int taskId = await sut.Member.OverWriteAsync(_payload);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Put, "/members/overwrite")
                    .And.HasToken(token);

                // Body
                using var doc = JsonDocument.Parse(req.Content!.ReadAsStream());
                _ = doc.RootElement.GetProperty("member_data"u8).Should().BeSameJson(PayloadJson);

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
            string[] codes = _payload.Select(d => d.Code).ToArray();
            string token = FixtureFactory.Create<string>();
            string expectedJson = $"{{\"codes\":{JsonSerializer.Serialize(codes, Context.Default.IReadOnlyListString)}}}";

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/delete")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            int taskId = await sut.Member.DeleteAsync(codes);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Post, "/members/delete")
                    .And.HasToken(token);

                // Body
                using var doc = JsonDocument.Parse(req.Content!.ReadAsStream());
                _ = doc.RootElement.Should().BeSameJson("""{ "codes": ["A0002", "A0001"]}""");

                return true;
            }, Times.Once());
        }

        /// <summary>メンバー情報 顔写真 APIのリクエストPayload</summary>
        /*lang=json,strict*/
        private const string FaceImagePayloadJson = """
        [
          {
            "code": "A0001",
            "base64_face_image": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAA1JREFUGFdj+P///38ACfsD/QVDRcoAAAAASUVORK5CYII="
          }
        ]
        """;
        /// <summary>メンバー情報 顔写真 APIのリクエストPayload</summary>
        private static readonly IReadOnlyList<FaceImage> _faceImagePayload = JsonSerializer.Deserialize(FaceImagePayloadJson, Context.Default.IReadOnlyListFaceImage)!;


        /// <summary>
        /// <see cref="KaonaviClient.Member.AddFaceImageAsync"/>は、"/members/face_image"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.AddFaceImageAsync)} > POST /members/face_image をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("メンバー情報")]
        [DataRow(true, /*lang=json,strict*/ $$"""{ "enable_trimming": true, "member_data": {{FaceImagePayloadJson}} }""")]
        [DataRow(false, /*lang=json,strict*/ $$"""{ "enable_trimming": false, "member_data": {{FaceImagePayloadJson}} }""")]
        public async Task Member_AddFaceImageAsync_Calls_PostApi(bool enableTrimming, string expectedJson)
        {
            // Arrange
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/face_image")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            int taskId = await sut.Member.AddFaceImageAsync(_faceImagePayload, enableTrimming);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Post, "/members/face_image")
                    .And.HasToken(token);

                // Body
                using var doc = JsonDocument.Parse(req.Content!.ReadAsStream());
                _ = doc.RootElement.Should().BeSameJson(expectedJson);

                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.UpdateFaceImageAsync"/>は、"/members/face_image"にPATCHリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.UpdateFaceImageAsync)} > PATCH /members/face_image をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("メンバー情報")]
        [DataRow(true, /*lang=json,strict*/ $$"""{ "enable_trimming": true, "member_data": {{FaceImagePayloadJson}} }""")]
        [DataRow(false, /*lang=json,strict*/ $$"""{ "enable_trimming": false, "member_data": {{FaceImagePayloadJson}} }""")]
        public async Task Member_UpdateFaceImageAsync_Calls_PatchApi(bool enableTrimming, string expectedJson)
        {
            // Arrange
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/face_image")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            int taskId = await sut.Member.UpdateFaceImageAsync(_faceImagePayload, enableTrimming);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Patch, "/members/face_image")
                    .And.HasToken(token);

                // Body
                using var doc = JsonDocument.Parse(req.Content!.ReadAsStream());
                _ = doc.RootElement.Should().BeSameJson(expectedJson);

                return true;
            }, Times.Once());
        }
    }
}
