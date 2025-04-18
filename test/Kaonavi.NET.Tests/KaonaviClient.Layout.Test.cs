using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Layout"/>の単体テスト</summary>
    [TestClass]
    public sealed class LayoutTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Layout.ReadMemberLayoutAsync"/>は、"/member_layouts"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadMemberLayoutAsync)} > GET /member_layouts をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("レイアウト設定")]
        [DataRow(false, "/member_layouts", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadMemberLayoutAsync)}(false) > GET /member_layouts をコールする。")]
        [DataRow(true, "/member_layouts?get_calc_type=true", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadMemberLayoutAsync)}(true) > GET /member_layouts?get_calc_type=true をコールする。")]
        public async ValueTask Layout_ReadMemberLayoutAsync_Calls_GetApi(bool getCalcType, string expectedEndpoint)
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "code": {
                "name": "社員番号",
                "required": true,
                "type": "string",
                "max_length": 50,
                "enum": []
              },
              "name": {
                "name": "氏名",
                "required": false,
                "type": "string",
                "max_length": 100,
                "enum": []
              },
              "name_kana": {
                "name": "フリガナ",
                "required": false,
                "type": "string",
                "max_length": 100,
                "enum": []
              },
              "mail": {
                "name": "メールアドレス",
                "required": false,
                "type": "string",
                "max_length": 100,
                "enum": []
              },
              "entered_date": {
                "name": "入社日",
                "required": false,
                "type": "date",
                "max_length": null,
                "enum": []
              },
              "retired_date": {
                "name": "退職日",
                "required": false,
                "type": "date",
                "max_length": null,
                "enum": []
              },
              "gender": {
                "name": "性別",
                "required": false,
                "type": "enum",
                "max_length": null,
                "enum": ["男性", "女性"]
              },
              "birthday": {
                "name": "生年月日",
                "required": false,
                "type": "date",
                "max_length": null,
                "enum": []
              },
              "department": {
                "name": "所属",
                "required": false,
                "type": "department",
                "max_length": null,
                "enum": []
              },
              "sub_departments": {
                "name": "兼務情報",
                "required": false,
                "type": "department[]",
                "max_length": null,
                "enum": []
              },
              "custom_fields": [
                {
                  "id": 100,
                  "name": "血液型",
                  "required": false,
                  "type": "enum",
                  "max_length": null,
                  "enum": ["A", "B", "O", "AB"]
                },
                {
                  "id": 200,
                  "name": "役職",
                  "required": false,
                  "type": "enum",
                  "max_length": null,
                  "enum": ["部長", "課長", "マネージャー"]
                },
                {
                  "id": 300,
                  "name": "計算式",
                  "required": false,
                  "type": "calc",
                  "max_length": null,
                  "enum": null,
                  "read_only": true
                }
              ]
            }
            """;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == expectedEndpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var layout = await sut.Layout.ReadMemberLayoutAsync(getCalcType);

            // Assert
            layout.ShouldNotBeNull();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                req => req.RequestUri?.PathAndQuery.ShouldBe(expectedEndpoint)
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Layout.ListAsync"/>は、<paramref name="expectedEndpoint"/>にGETリクエストを行う。
        /// </summary>
        /// <param name="getCalcType"><inheritdoc cref="KaonaviClient.ILayout.ListAsync" path="/param[@name='getCalcType']"/></param>
        /// <param name="expectedEndpoint">呼び出されるAPIエンドポイント</param>
        [TestMethod($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ListAsync)} > GET /sheet_layouts をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("レイアウト設定")]
        [DataRow(false, "/sheet_layouts", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ListAsync)}(false) > GET /sheet_layouts をコールする。")]
        [DataRow(true, "/sheet_layouts?get_calc_type=true", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ListAsync)}(true) > GET /sheet_layouts?get_calc_type=true をコールする。")]
        public async ValueTask Layout_ListAsync_Calls_GetApi(bool getCalcType, string expectedEndpoint)
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "sheets": [
                {
                  "id": 12,
                  "name": "住所・連絡先",
                  "record_type": 1,
                  "custom_fields": [
                    {
                      "id": 1000,
                      "name": "住所",
                      "required": false,
                      "type": "string",
                      "max_length": 250,
                      "enum": []
                    },
                    {
                      "id": 1001,
                      "name": "電話番号",
                      "required": false,
                      "type": "string",
                      "max_length": 50,
                      "enum": []
                    }
                  ]
                }
              ]
            }
            """;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == expectedEndpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var layouts = await sut.Layout.ListAsync(getCalcType);

            // Assert
            layouts.ShouldNotBeEmpty();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                req => req.RequestUri?.PathAndQuery.ShouldBe(expectedEndpoint)
            );
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ILayout.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Layout.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadAsync)}(-1) > ArgumentOutOfRangeException をスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("レイアウト設定")]
        public async ValueTask When_Id_IsNegative_Layout_ReadAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => _ = await sut.Layout.ReadAsync(-1);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Layout.ReadAsync"/>は、<paramref name="expectedEndpoint"/>にGETリクエストを行う。
        /// </summary>
        /// <param name="getCalcType"><inheritdoc cref="KaonaviClient.ILayout.ReadAsync" path="/param[@name='getCalcType']"/></param>
        /// <param name="expectedEndpoint">呼び出されるAPIエンドポイント</param>
        [TestMethod($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadAsync)} > GET /sheet_layouts/:id をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("レイアウト設定")]
        [DataRow(false, "/sheet_layouts/12", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadAsync)}(12, false) > GET /sheet_layouts/12 をコールする。")]
        [DataRow(true, "/sheet_layouts/12?get_calc_type=true", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadAsync)}(12, true) > GET /sheet_layouts/12?get_calc_type=true をコールする。")]
        public async ValueTask Layout_ReadAsync_Calls_GetApi(bool getCalcType, string expectedEndpoint)
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "id": 12,
              "name": "住所・連絡先",
              "record_type": 1,
              "custom_fields": [
                {
                  "id": 1000,
                  "name": "住所",
                  "required": false,
                  "type": "string",
                  "max_length": 100,
                  "enum": [],
                  "type_detail": "text_box"
                },
                {
                  "id": 1001,
                  "name": "電話番号",
                  "required": false,
                  "type": "string",
                  "max_length": 100,
                  "enum": [],
                  "type_detail": "text_box"
                },
                {
                  "id": 1002,
                  "name": "計算式",
                  "required": false,
                  "type": "calc",
                  "max_length": null,
                  "enum": [],
                  "read_only": true,
                  "type_detail": "text_box"
                }
              ]
            }
            """;
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == expectedEndpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var layout = await sut.Layout.ReadAsync(12, getCalcType);

            // Assert
            layout.ShouldNotBeNull();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                req => req.RequestUri?.PathAndQuery.ShouldBe(expectedEndpoint)
            );
        }
    }
}
