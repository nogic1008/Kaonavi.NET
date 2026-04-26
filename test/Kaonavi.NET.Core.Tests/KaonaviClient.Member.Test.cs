using System.Text.Json;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Member"/>の単体テスト</summary>
    [Category("API"), Category("メンバー情報")]
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
        private static readonly IReadOnlyList<MemberData> _payload = JsonSerializer.Deserialize(PayloadJson, JsonContext.Default.IReadOnlyListMemberData)!;

        /// <summary>
        /// <see cref="KaonaviClient.Member.ListAsync"/>は、"/members"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.ListAsync)} > GET /members をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task Member_ListAsync_Calls_GetApi(CancellationToken cancellationToken = default)
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
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet("/members").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var members = await sut.Member.ListAsync(cancellationToken);

            // Assert
            await Assert.That(members).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path("/members"), Times.Once);
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.CreateAsync"/>は、"/members"にPOSTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.CreateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.CreateAsync)} > POST /members をコールする。")]
        [Category(nameof(HttpMethod.Post))]
        public async Task Member_CreateAsync_Calls_PostApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPost("/members").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Member.CreateAsync(_payload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/members"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals($$"""{"member_data": {{PayloadJson}}}""");
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.ReplaceAsync"/>は、"/members"にPUTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.ReplaceAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.ReplaceAsync)} > PUT /members をコールする。")]
        [Category(nameof(HttpMethod.Put))]
        public async Task Member_ReplaceAsync_Calls_PutApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPut("/members").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Member.ReplaceAsync(_payload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Put).Path("/members"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals($$"""{"member_data": {{PayloadJson}}}""");
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.UpdateAsync"/>は、"/members"にPATCHリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.UpdateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.UpdateAsync)} > PATCH /members をコールする。")]
        [Category(nameof(HttpMethod.Patch))]
        public async Task Member_UpdateAsync_Calls_PatchApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnRequest(req => req.Method(HttpMethod.Patch).Path("/members")).RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Member.UpdateAsync(_payload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Patch).Path("/members"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals($$"""{"member_data": {{PayloadJson}}}""");
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.OverWriteAsync"/>は、"/members/overwrite"にPUTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.OverWriteAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.OverWriteAsync)} > PUT /members/overwrite をコールする。")]
        [Category(nameof(HttpMethod.Put))]
        public async Task Member_OverWriteAsync_Calls_PutApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPut("/members/overwrite").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Member.OverWriteAsync(_payload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Put).Path("/members/overwrite"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals($$"""{"member_data": {{PayloadJson}}}""");
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.DeleteAsync"/>は、"/members/delete"にPOSTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.DeleteAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.DeleteAsync)} > POST /members/delete をコールする。")]
        [Category(nameof(HttpMethod.Post))]
        public async Task Member_DeleteAsync_Calls_PostApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            string[] codes = [.. _payload.Select(d => d.Code)];

            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPost("/members/delete").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Member.DeleteAsync(codes, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/members/delete"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals("""{ "codes": ["A0002", "A0001"] }"""u8);
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
        private static readonly IReadOnlyList<FaceImagePayload> _faceImagePayload = JsonSerializer.Deserialize(FaceImagePayloadJson, JsonContext.Default.IReadOnlyListFaceImagePayload)!;

        /// <summary>
        /// <see cref="KaonaviClient.Member.GetFaceImageListAsync"/>は、"/members/face_image"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.GetFaceImageListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.GetFaceImageListAsync)} > GET /members/face_image をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        [Arguments(null, "/members/face_image", DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.GetFaceImageListAsync)}() > GET /members/face_image をコールする。")]
        [Arguments("2020-10-25", "/members/face_image?updated_since=2020-10-25", DisplayName = $"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.GetFaceImageListAsync)}(2020-10-25) > GET /members/face_image?updated_since=2020-10-25 をコールする。")]
        public async Task Member_GetFaceImageListAsync_Calls_GetApi(string? date, string expectedUri, CancellationToken cancellationToken = default)
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
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet(expectedUri).RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var faceImages = await sut.Member.GetFaceImageListAsync(date is not null ? DateOnly.Parse(date) : default, cancellationToken);

            // Assert
            await Assert.That(faceImages).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path(expectedUri), Times.Once);
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.AddFaceImageAsync"/>は、"/members/face_image"にPOSTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.AddFaceImageAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.AddFaceImageAsync)} > POST /members/face_image をコールする。")]
        [Category(nameof(HttpMethod.Post))]
        [Arguments(true, /*lang=json,strict*/ $$"""{ "enable_trimming": true, "member_data": {{FaceImagePayloadJson}} }""")]
        [Arguments(false, /*lang=json,strict*/ $$"""{ "enable_trimming": false, "member_data": {{FaceImagePayloadJson}} }""")]
        public async Task Member_AddFaceImageAsync_Calls_PostApi(bool enableTrimming, string expectedJson, CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPost("/members/face_image").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Member.AddFaceImageAsync(_faceImagePayload, enableTrimming, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/members/face_image"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals(expectedJson);
        }

        /// <summary>
        /// <see cref="KaonaviClient.Member.UpdateFaceImageAsync"/>は、"/members/face_image"にPATCHリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.UpdateFaceImageAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Member)}.{nameof(KaonaviClient.Member.UpdateFaceImageAsync)} > PATCH /members/face_image をコールする。")]
        [Category(nameof(HttpMethod.Patch))]
        [Arguments(true, /*lang=json,strict*/ $$"""{ "enable_trimming": true, "member_data": {{FaceImagePayloadJson}} }""")]
        [Arguments(false, /*lang=json,strict*/ $$"""{ "enable_trimming": false, "member_data": {{FaceImagePayloadJson}} }""")]
        public async Task Member_UpdateFaceImageAsync_Calls_PatchApi(bool enableTrimming, string expectedJson, CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnRequest(req => req.Method(HttpMethod.Patch).Path("/members/face_image")).RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Member.UpdateFaceImageAsync(_faceImagePayload, enableTrimming, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Patch).Path("/members/face_image"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals(expectedJson);
        }
    }
}
