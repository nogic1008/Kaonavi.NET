using System.Text.Json;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Sheet"/>の単体テスト</summary>
    [Category("API"), Category("シート情報")]
    public sealed class SheetTest
    {
        /// <summary>シート情報APIのリクエストPayload</summary>
        /*lang=json,strict*/
        private const string SheetDataJson = """
        [
          {
            "code": "A0002",
            "records": [
              {
                "custom_fields": [
                  { "id": 1000, "values": ["東京都港区x-x-x"] }
                ]
              }
            ]
          },
          {
            "code": "A0001",
            "records": [
              {
                "custom_fields": [
                  { "id": 1000, "values": ["大阪府大阪市y番y号"] },
                  { "id": 1001, "values": ["06-yyyy-yyyy"] }
                ]
              },
              {
                "custom_fields": [
                  { "id": 1000, "values": ["愛知県名古屋市z丁目z番z号"] },
                  { "id": 1001, "values": ["052-zzzz-zzzz"] }
                ]
              }
            ]
          }
        ]
        """;
        /// <summary>シート情報APIのリクエストPayload</summary>
        private static readonly IReadOnlyList<SheetData> _sheetDataPayload = JsonSerializer.Deserialize(SheetDataJson, JsonContext.Default.IReadOnlyListSheetData)!;

        /// <summary>シート情報 添付ファイルAPIのリクエストPayload</summary>
        /*lang=json,strict*/
        private const string AttachmentPayloadJson = """
        [
          {
            "code": "A0001",
            "records": [
              {
                "file_name": "sample.txt",
                "base64_content": "44GT44KM44Gv44K144Oz44OX44Or44OG44Kt44K544OI44Gn44GZ44CC"
              }
            ]
          }
        ]
        """;
        /// <summary>シート情報 添付ファイルAPIのリクエストPayload</summary>
        private static readonly IReadOnlyList<AttachmentPayload> _attachmentPayload
            = JsonSerializer.Deserialize(AttachmentPayloadJson, JsonContext.Default.IReadOnlyListAttachmentPayload)!;

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.ListAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.ListAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListAsync)} > ArgumentOutOfRangeException をスローする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task When_Id_IsNegative_Sheet_ListAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => _ = await sut.Sheet.ListAsync(-1, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.ListAsync"/>は、"/sheets/{sheetId}"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListAsync)} > GET /sheets/:sheetId をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task Sheet_ListAsync_Calls_GetApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int sheetId = 1;
            /*lang=json,strict*/
            const string responseJson = """
            {
              "id": 12,
              "name": "住所・連絡先",
              "record_type": 1,
              "updated_at": "2020-10-01 01:23:45",
              "member_data": [
                {
                  "code": "A0002",
                  "records": [
                    {
                      "custom_fields": [
                        {
                          "id": 1000,
                          "name": "住所",
                          "values": ["東京都港区x-x-x"]
                        }
                      ]
                    }
                  ]
                },
                {
                  "code": "A0001",
                  "records": [
                    {
                      "custom_fields": [
                        {
                          "id": 1000,
                          "name": "住所",
                          "values": ["大阪府大阪市y番y号"]
                        },
                        {
                          "id": 1001,
                          "name": "電話番号",
                          "values": ["06-yyyy-yyyy"]
                        }
                      ]
                    },
                    {
                      "custom_fields": [
                        {
                          "id": 1000,
                          "name": "住所",
                          "values": ["愛知県名古屋市z丁目z番z号"]
                        },
                        {
                          "id": 1001,
                          "name": "電話番号",
                          "values": ["052-zzzz-zzzz"]
                        }
                      ]
                    }
                  ]
                }
              ]
            }
            """;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet($"/sheets/{sheetId}").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var members = await sut.Sheet.ListAsync(sheetId, cancellationToken);

            // Assert
            await Assert.That(members).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path($"/sheets/{sheetId}"), Times.Once);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.ReplaceAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.ReplaceAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.ReplaceAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ReplaceAsync)} > ArgumentOutOfRangeException をスローする。")]
        [Category(nameof(HttpMethod.Put))]
        public async Task When_Id_IsNegative_Sheet_ReplaceAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client, "token");
            await Assert.That(async () => _ = await sut.Sheet.ReplaceAsync(-1, _sheetDataPayload, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.ReplaceAsync"/>は、"/sheets/{sheetId}"にPUTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.ReplaceAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ReplaceAsync)} > PUT /sheets/:sheetId をコールする。")]
        [Category(nameof(HttpMethod.Put))]
        public async Task Sheet_ReplaceAsync_Calls_PutApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int sheetId = 1;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPut($"/sheets/{sheetId}").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Sheet.ReplaceAsync(sheetId, _sheetDataPayload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Put).Path($"/sheets/{sheetId}"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsValidJsonObject().And.IsJsonEquals($$"""
            {"member_data": {{SheetDataJson}}}
            """);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.UpdateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.UpdateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.UpdateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateAsync)} > ArgumentOutOfRangeException をスローする。")]
        [Category(nameof(HttpMethod.Patch))]
        public async Task When_Id_IsNegative_Sheet_UpdateAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client, "token");
            await Assert.That(async () => _ = await sut.Sheet.UpdateAsync(-1, _sheetDataPayload, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.UpdateAsync"/>は、"/sheets/{sheetId}"にPATCHリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.UpdateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateAsync)} > PATCH /sheets/:sheetId をコールする。")]
        [Category(nameof(HttpMethod.Patch))]
        public async Task Sheet_UpdateAsync_Calls_PatchApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int sheetId = 1;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnRequest(req => req.Method(HttpMethod.Patch).Path($"/sheets/{sheetId}")).RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Sheet.UpdateAsync(sheetId, _sheetDataPayload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Patch).Path($"/sheets/{sheetId}"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsValidJsonObject().And.IsJsonEquals($$"""
            {"member_data": {{SheetDataJson}}}
            """);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.CreateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.CreateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.CreateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.CreateAsync)} > ArgumentOutOfRangeException をスローする。")]
        [Category(nameof(HttpMethod.Post))]
        public async Task When_Id_IsNegative_Sheet_CreateAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client, "token");
            await Assert.That(async () => _ = await sut.Sheet.CreateAsync(-1, _sheetDataPayload, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.CreateAsync"/>は、"/sheets/{sheetId}/add"にPOSTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.CreateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.CreateAsync)} > POST /sheets/:sheetId/add をコールする。")]
        [Category(nameof(HttpMethod.Post))]
        public async Task Sheet_CreateAsync_Calls_PostApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int sheetId = 1;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPost($"/sheets/{sheetId}/add").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Sheet.CreateAsync(sheetId, _sheetDataPayload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Post).Path($"/sheets/{sheetId}/add"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsValidJsonObject().And.IsJsonEquals($$"""
            {"member_data": {{SheetDataJson}}}
            """);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.ListFileAsync" path="/param[@name='id']"/>または<inheritdoc cref="KaonaviClient.ISheet.ListFileAsync" path="/param[@name='customFieldId']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.ListFileAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.ListFileAsync" path="/param[@name='id']"/>
        /// <inheritdoc cref="KaonaviClient.ISheet.ListFileAsync" path="/param[@name='customFieldId']"/>
        /// <param name="paramName">例外発生の原因となるパラメータ名</param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.ListFileAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListFileAsync)} > ArgumentOutOfRangeException をスローする。")]
        [Arguments(-1, 1, nameof(id), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListFileAsync)}(-1, 1, []) > ArgumentOutOfRangeException をスローする。")]
        [Arguments(1, -1, nameof(customFieldId), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListFileAsync)}(1, -1, []) > ArgumentOutOfRangeException をスローする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task When_Id_IsNegative_Sheet_ListFileAsync_Throws_ArgumentOutOfRangeException(int id, int customFieldId, string paramName, CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => _ = await sut.Sheet.ListFileAsync(id, customFieldId, cancellationToken: cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName(paramName);
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.ListFileAsync"/>は、<paramref name="expectedUri"/>にGETリクエストを行う。
        /// </summary>
        /// <param name="date">updatedSinceに渡す日付パラメータ(<c>null</c>指定時は<c>default</c>)</param>
        /// <param name="expectedUri">GETリクエスト先のURI</param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.ListFileAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListFileAsync)} > GET /sheets/:sheetId/custom_fields/:customFieldId/file をコールする。")]
        [Arguments(null, "/sheets/1/custom_fields/1/file", DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListFileAsync)}(1, 1) > GET /sheets/1/custom_fields/1/file をコールする。")]
        [Arguments("2020-10-25", "/sheets/1/custom_fields/1/file?updated_since=2020-10-25", DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListFileAsync)}(1, 1, 2020-10-25) > GET /sheets/1/custom_fields/1/file?updated_since=2020-10-25 をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task Sheet_ListFileAsync_Calls_GetApi(string? date, string expectedUri, CancellationToken cancellationToken = default)
        {
            // Arrange
            const int sheetId = 1;
            const int customFieldId = 1;
            /*lang=json,strict*/
            const string responseJson = """
            {
              "member_data": [
                {
                  "code": "A0001",
                  "records": [
                    {
                      "file_name": "A0001.jpg",
                      "download_url": "https://example.com/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID",
                      "updated_at": "2020-10-01 01:23:45"
                    },
                    {
                      "file_name": "A0001.txt",
                      "download_url": "https://example.com/image/xxxx.txt?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID",
                      "updated_at": "2020-10-01 01:23:45"
                    }
                  ]
                }
              ]
            }
            """;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet(expectedUri).RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var members = await sut.Sheet.ListFileAsync(sheetId, customFieldId, date is not null ? DateOnly.Parse(date) : default, cancellationToken);

            // Assert
            await Assert.That(members).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path(expectedUri), Times.Once);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='id']"/>または<inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='customFieldId']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.AddFileAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='id']"/>
        /// <inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='customFieldId']"/>
        /// <param name="paramName">例外発生の原因となるパラメータ名</param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.AddFileAsync)} > ArgumentOutOfRangeException をスローする。")]
        [Arguments(-1, 1, nameof(id), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.AddFileAsync)}(-1, 1, []) > ArgumentOutOfRangeException をスローする。")]
        [Arguments(1, -1, nameof(customFieldId), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.AddFileAsync)}(1, -1, []) > ArgumentOutOfRangeException をスローする。")]
        [Category(nameof(HttpMethod.Post))]
        public async Task When_Id_IsNegative_Sheet_AddFileAsync_Throws_ArgumentOutOfRangeException(int id, int customFieldId, string paramName, CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => _ = await sut.Sheet.AddFileAsync(id, customFieldId, _attachmentPayload, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName(paramName);
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.AddFileAsync"/>は、"/sheets/{sheetId}/custom_fields/{customFieldId}/file"にPOSTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.AddFileAsync)} > POST /sheets/:sheetId/custom_fields/:customFieldId/file をコールする。")]
        [Category(nameof(HttpMethod.Post))]
        public async Task Sheet_AddFileAsync_Calls_PostApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int sheetId = 1;
            const int customFieldId = 1;
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPost($"/sheets/{sheetId}/custom_fields/{customFieldId}/file").RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Sheet.AddFileAsync(sheetId, customFieldId, _attachmentPayload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Post).Path($"/sheets/{sheetId}/custom_fields/{customFieldId}/file"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsValidJsonObject().And.IsJsonEquals($$"""
            {"member_data": {{AttachmentPayloadJson}}}
            """);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='id']"/>または<inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='customFieldId']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.UpdateFileAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='id']"/>
        /// <inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='customFieldId']"/>
        /// <param name="paramName">例外発生の原因となるパラメータ名</param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateFileAsync)} > ArgumentOutOfRangeException をスローする。")]
        [Arguments(-1, 1, nameof(id), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateFileAsync)}(-1, 1, []) > ArgumentOutOfRangeException をスローする。")]
        [Arguments(1, -1, nameof(customFieldId), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateFileAsync)}(1, -1, []) > ArgumentOutOfRangeException をスローする。")]
        [Category(nameof(HttpMethod.Patch))]
        public async Task When_Id_IsNegative_Sheet_UpdateFileAsync_Throws_ArgumentOutOfRangeException(int id, int customFieldId, string paramName, CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => _ = await sut.Sheet.UpdateFileAsync(id, customFieldId, [], cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName(paramName);
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.UpdateFileAsync"/>は、"/sheets/{sheetId}/custom_fields/{customFieldId}/file"にPATCHリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateFileAsync)} > PATCH /sheets/:sheetId/custom_fields/:customFieldId/file をコールする。")]
        [Category(nameof(HttpMethod.Patch))]
        public async Task Sheet_UpdateFileAsync_Calls_PostApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int sheetId = 1;
            const int customFieldId = 1;
            string path = $"/sheets/{sheetId}/custom_fields/{customFieldId}/file";
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnRequest(req => req.Method(HttpMethod.Patch).Path(path)).RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.Sheet.UpdateFileAsync(sheetId, customFieldId, _attachmentPayload, cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Patch).Path(path), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsValidJsonObject().And.IsJsonEquals($$"""
            {"member_data": {{AttachmentPayloadJson}}}
            """);
        }
    }
}
