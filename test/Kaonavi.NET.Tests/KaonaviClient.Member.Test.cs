using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;
using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

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
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.ListAsync)} > GET /members をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("メンバー情報")]
        public async ValueTask Member_ListAsync_Calls_GetApi()
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var members = await sut.Member.ListAsync();

            // Assert
            members.ShouldNotBeEmpty();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/members")
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.CreateAsync"/>は、"/members"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.CreateAsync)} > POST /members をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("メンバー情報")]
        public async ValueTask Member_CreateAsync_Calls_PostApi()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Member.CreateAsync(_payload);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Post),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/members"),
                static req => req.Content!.ShouldHaveJsonBody("member_data"u8, PayloadJson)
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.ReplaceAsync"/>は、"/members"にPUTリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.ReplaceAsync)} > PUT /members をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("メンバー情報")]
        public async ValueTask Member_ReplaceAsync_Calls_PutApi()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Member.ReplaceAsync(_payload);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Put),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/members"),
                static req => req.Content!.ShouldHaveJsonBody("member_data"u8, PayloadJson)
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.UpdateAsync"/>は、"/members"にPATCHリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.UpdateAsync)} > PATCH /members をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("メンバー情報")]
        public async ValueTask Member_UpdateAsync_Calls_PatchApi()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Member.UpdateAsync(_payload);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Patch),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/members"),
                static req => req.Content!.ShouldHaveJsonBody("member_data"u8, PayloadJson)
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.OverWriteAsync"/>は、"/members/overwrite"にPUTリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.OverWriteAsync)} > PUT /members/overwrite をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("メンバー情報")]
        public async ValueTask Member_OverWriteAsync_Calls_PutApi()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/overwrite")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Member.OverWriteAsync(_payload);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Put),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/members/overwrite"),
                static req => req.Content!.ShouldHaveJsonBody("member_data"u8, PayloadJson)
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.DeleteAsync"/>は、"/members/delete"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.DeleteAsync)} > POST /members/delete をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("メンバー情報")]
        public async ValueTask Member_DeleteAsync_Calls_PostApi()
        {
            // Arrange
            string[] codes = _payload.Select(d => d.Code).ToArray();
            string expectedJson = $"{{\"codes\":{JsonSerializer.Serialize(codes, Context.Default.IReadOnlyListString)}}}";

            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/delete")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Member.DeleteAsync(codes);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Post),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/members/delete"),
                static req => req.Content!.ShouldHaveJsonBody("""{ "codes": ["A0002", "A0001"] }""")
            );
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
        private static readonly IReadOnlyList<FaceImagePayload> _faceImagePayload = JsonSerializer.Deserialize(FaceImagePayloadJson, Context.Default.IReadOnlyListFaceImagePayload)!;


        /// <summary>
        /// <see cref="KaonaviClient.Member.GetFaceImageListAsync"/>は、"/members/face_image"にGETリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.GetFaceImageListAsync)} > GET /members/face_image をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("メンバー情報")]
        public async ValueTask Member_GetFaceImageListAsync_Calls_GetApi()
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "member_data": [
                {
                  "code": "A0001",
                  "file_name": "A0001.jpg",
                  "download_url": "https://example.kaonavi.jp/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID",
                  "updated_at": "2020-10-01 01:23:45"
                },
                {
                  "code": "A0002",
                  "file_name": "A0002.jpg",
                  "download_url": "https://example.kaonavi.jp/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID",
                  "updated_at": "2020-10-01 01:23:45"
                }
              ]
            }
            """;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/face_image?updated_since=2020-10-01")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var faceImages = await sut.Member.GetFaceImageListAsync(new DateOnly(2020, 10, 1));

            // Assert
            faceImages.ShouldNotBeEmpty();

            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/members/face_image?updated_since=2020-10-01")
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.AddFaceImageAsync"/>は、"/members/face_image"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.AddFaceImageAsync)} > POST /members/face_image をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("メンバー情報")]
        [DataRow(true, /*lang=json,strict*/ $$"""{ "enable_trimming": true, "member_data": {{FaceImagePayloadJson}} }""")]
        [DataRow(false, /*lang=json,strict*/ $$"""{ "enable_trimming": false, "member_data": {{FaceImagePayloadJson}} }""")]
        public async ValueTask Member_AddFaceImageAsync_Calls_PostApi(bool enableTrimming, string expectedJson)
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/face_image")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Member.AddFaceImageAsync(_faceImagePayload, enableTrimming);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Post),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/members/face_image"),
                req => req.Content!.ShouldHaveJsonBody(expectedJson)
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.UpdateFaceImageAsync"/>は、"/members/face_image"にPATCHリクエストを行う。
        /// </summary>
        [TestMethod(DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.UpdateFaceImageAsync)} > PATCH /members/face_image をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("メンバー情報")]
        [DataRow(true, /*lang=json,strict*/ $$"""{ "enable_trimming": true, "member_data": {{FaceImagePayloadJson}} }""")]
        [DataRow(false, /*lang=json,strict*/ $$"""{ "enable_trimming": false, "member_data": {{FaceImagePayloadJson}} }""")]
        public async ValueTask Member_UpdateFaceImageAsync_Calls_PatchApi(bool enableTrimming, string expectedJson)
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/face_image")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Member.UpdateFaceImageAsync(_faceImagePayload, enableTrimming);

            // Assert
            taskId.ShouldBe(TaskId);

            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Patch),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/members/face_image"),
                req => req.Content!.ShouldHaveJsonBody(expectedJson)
            );
        }
    }
}
