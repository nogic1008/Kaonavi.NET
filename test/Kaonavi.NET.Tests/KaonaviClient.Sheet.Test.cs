using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;
using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Sheet"/>の単体テスト</summary>
    [TestClass]
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
        private static readonly IReadOnlyList<SheetData> _sheetDataPayload = JsonSerializer.Deserialize(SheetDataJson, Context.Default.IReadOnlyListSheetData)!;

        /// <summary>シート情報 添付ファイルAPIのリクエストPayload</summary>
        /*lang=json,strict*/
        private const string AttachmentJson = """
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
        private static readonly IReadOnlyList<Attachment> _attachmentPayload = JsonSerializer.Deserialize(AttachmentJson, Context.Default.IReadOnlyListAttachment)!;

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.ListAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.ListAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListAsync)} > ArgumentOutOfRangeException をスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("シート情報")]
        public async Task When_Id_IsNegative_Sheet_ListAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.Sheet.ListAsync(-1);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.ListAsync"/>は、"/sheets/{sheetId}"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ListAsync)} > GET /sheets/:sheetId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("シート情報")]
        public async Task Sheet_ListAsync_Calls_GetApi()
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var members = await sut.Sheet.ListAsync(sheetId);

            // Assert
            members.ShouldNotBeEmpty();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/sheets/{sheetId}")
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.ReplaceAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.ReplaceAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ReplaceAsync)} > ArgumentOutOfRangeException をスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("シート情報")]
        public async Task When_Id_IsNegative_Sheet_ReplaceAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.Sheet.ReplaceAsync(-1, _sheetDataPayload);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.ReplaceAsync"/>は、"/sheets/{sheetId}"にPUTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.ReplaceAsync)} > PUT /sheets/:sheetId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("シート情報")]
        public async Task Sheet_ReplaceAsync_Calls_PutApi()
        {
            // Arrange
            const int sheetId = 1;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Sheet.ReplaceAsync(sheetId, _sheetDataPayload);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Put),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/sheets/{sheetId}"),
                static req => req.Content!.ShouldHaveJsonBody("member_data"u8, SheetDataJson)
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.UpdateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.UpdateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateAsync)} > ArgumentOutOfRangeException をスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("シート情報")]
        public async Task When_Id_IsNegative_Sheet_UpdateAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.Sheet.UpdateAsync(-1, _sheetDataPayload);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.UpdateAsync"/>は、"/sheets/{sheetId}"にPATCHリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateAsync)} > PATCH /sheets/:sheetId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("シート情報")]
        public async Task Sheet_UpdateAsync_Calls_PatchApi()
        {
            // Arrange
            const int sheetId = 1;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Sheet.UpdateAsync(sheetId, _sheetDataPayload);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Patch),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/sheets/{sheetId}"),
                static req => req.Content!.ShouldHaveJsonBody("member_data"u8, SheetDataJson)
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.CreateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.CreateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.CreateAsync)} > ArgumentOutOfRangeException をスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("シート情報")]
        public async Task When_Id_IsNegative_Sheet_CreateAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.Sheet.CreateAsync(-1, _sheetDataPayload);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.CreateAsync"/>は、"/sheets/{sheetId}/add"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.CreateAsync)} > POST /sheets/:sheetId/add をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("シート情報")]
        public async Task Sheet_CreateAsync_Calls_PostApi()
        {
            // Arrange
            const int sheetId = 1;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}/add")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Sheet.CreateAsync(sheetId, _sheetDataPayload);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Post),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/sheets/{sheetId}/add"),
                static req => req.Content!.ShouldHaveJsonBody("member_data"u8, SheetDataJson)
            );
        }


        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='id']"/>または<inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='customFieldId']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.AddFileAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='id']"/>
        /// <inheritdoc cref="KaonaviClient.ISheet.AddFileAsync" path="/param[@name='customFieldId']"/>
        /// <param name="paramName">例外発生の原因となるパラメータ名</param>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.AddFileAsync)} > ArgumentOutOfRangeException をスローする。")]
        [DataRow(-1, 1, nameof(id), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.AddFileAsync)}(-1, 1, []) > ArgumentOutOfRangeException をスローする。")]
        [DataRow(1, -1, nameof(customFieldId), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.AddFileAsync)}(1, -1, []) > ArgumentOutOfRangeException をスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("シート情報")]
        public async Task When_Id_IsNegative_Sheet_AddFileAsync_Throws_ArgumentOutOfRangeException(int id, int customFieldId, string paramName)
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.Sheet.AddFileAsync(id, customFieldId, _attachmentPayload);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe(paramName);
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.AddFileAsync"/>は、"/sheets/{sheetId}/custom_fields/{customFieldId}/file"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.AddFileAsync)} > POST /sheets/:sheetId/custom_fields/:customFieldId/file をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("シート情報")]
        public async Task Sheet_AddFileAsync_Calls_PostApi()
        {
            // Arrange
            const int sheetId = 1;
            const int customFieldId = 1;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}/custom_fields/{customFieldId}/file")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.Sheet.AddFileAsync(sheetId, customFieldId, _attachmentPayload);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Post),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/sheets/{sheetId}/custom_fields/{customFieldId}/file"),
                static req => req.Content!.ShouldHaveJsonBody("member_data"u8, AttachmentJson)
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='id']"/>または<inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='customFieldId']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Sheet.UpdateFileAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='id']"/>
        /// <inheritdoc cref="KaonaviClient.ISheet.UpdateFileAsync" path="/param[@name='customFieldId']"/>
        /// <param name="paramName">例外発生の原因となるパラメータ名</param>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateFileAsync)} > ArgumentOutOfRangeException をスローする。")]
        [DataRow(-1, 1, nameof(id), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateFileAsync)}(-1, 1, []) > ArgumentOutOfRangeException をスローする。")]
        [DataRow(1, -1, nameof(customFieldId), DisplayName = $"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateFileAsync)}(1, -1, []) > ArgumentOutOfRangeException をスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("シート情報")]
        public async Task When_Id_IsNegative_Sheet_UpdateFileAsync_Throws_ArgumentOutOfRangeException(int id, int customFieldId, string paramName)
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.Sheet.UpdateFileAsync(id, customFieldId, []);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe(paramName);
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Sheet.UpdateFileAsync"/>は、"/sheets/{sheetId}/custom_fields/{customFieldId}/file"にPATCHリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Sheet)}.{nameof(KaonaviClient.Sheet.UpdateFileAsync)} > PATCH /sheets/:sheetId/custom_fields/:customFieldId/file をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("シート情報")]
        public async Task Sheet_UpdateFileAsync_Calls_PostApi()
        {
            // Arrange
            const int sheetId = 1;
            const int customFieldId = 1;
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_attachmentPayload, Context.Default.IReadOnlyListAttachment)}}}";

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}/custom_fields/{customFieldId}/file")
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, "token");
            int taskId = await sut.Sheet.UpdateFileAsync(sheetId, customFieldId, _attachmentPayload);

            // Assert
            taskId.ShouldBe(TaskId);
            handler.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Patch),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/sheets/{sheetId}/custom_fields/{customFieldId}/file"),
                static req => req.Content!.ShouldHaveJsonBody("member_data"u8, AttachmentJson)
            );
        }
    }
}
