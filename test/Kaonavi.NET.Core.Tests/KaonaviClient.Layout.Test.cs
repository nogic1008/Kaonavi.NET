namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Layout"/>の単体テスト</summary>
    [Category("API"), Category("レイアウト設定")]
    public sealed class LayoutTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Layout.ReadMemberLayoutAsync"/>は、"/member_layouts"にGETリクエストを行う。
        /// </summary>
        /// <param name="getCalcType"><inheritdoc cref="KaonaviClient.ILayout.ReadMemberLayoutAsync" path="/param[@name='getCalcType']"/></param>
        /// <param name="expectedEndpoint">呼び出されるAPIエンドポイント</param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ILayout.ReadMemberLayoutAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadMemberLayoutAsync)} > GET /member_layouts をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        [Arguments(false, "/member_layouts", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadMemberLayoutAsync)}(false) > GET /member_layouts をコールする。")]
        [Arguments(true, "/member_layouts?get_calc_type=true", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadMemberLayoutAsync)}(true) > GET /member_layouts?get_calc_type=true をコールする。")]
        public async Task Layout_ReadMemberLayoutAsync_Calls_GetApi(bool getCalcType, string expectedEndpoint, CancellationToken cancellationToken = default)
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
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet(expectedEndpoint).RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var layout = await sut.Layout.ReadMemberLayoutAsync(getCalcType, cancellationToken);

            // Assert
            await Assert.That(layout).IsNotNull();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path(expectedEndpoint), Times.Once);
        }

        /// <summary>
        /// <see cref="KaonaviClient.Layout.ListAsync"/>は、<paramref name="expectedEndpoint"/>にGETリクエストを行う。
        /// </summary>
        /// <param name="getCalcType"><inheritdoc cref="KaonaviClient.ILayout.ListAsync" path="/param[@name='getCalcType']"/></param>
        /// <param name="expectedEndpoint">呼び出されるAPIエンドポイント</param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ILayout.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ListAsync)} > GET /sheet_layouts をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        [Arguments(false, "/sheet_layouts", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ListAsync)}(false) > GET /sheet_layouts をコールする。")]
        [Arguments(true, "/sheet_layouts?get_calc_type=true", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ListAsync)}(true) > GET /sheet_layouts?get_calc_type=true をコールする。")]
        public async Task Layout_ListAsync_Calls_GetApi(bool getCalcType, string expectedEndpoint, CancellationToken cancellationToken = default)
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
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet(expectedEndpoint).RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var layouts = await sut.Layout.ListAsync(getCalcType, cancellationToken);

            // Assert
            await Assert.That(layouts).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path(expectedEndpoint), Times.Once);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ILayout.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Layout.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ILayout.ReadAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadAsync)}(-1) > ArgumentOutOfRangeException をスローする。")]
        [Category(nameof(HttpMethod.Get))]
        public async Task When_Id_IsNegative_Layout_ReadAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client, "token");
            await Assert.That(async () => _ = await sut.Layout.ReadAsync(-1, cancellationToken: cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Layout.ReadAsync"/>は、<paramref name="expectedEndpoint"/>にGETリクエストを行う。
        /// </summary>
        /// <param name="getCalcType"><inheritdoc cref="KaonaviClient.ILayout.ReadAsync" path="/param[@name='getCalcType']"/></param>
        /// <param name="expectedEndpoint">呼び出されるAPIエンドポイント</param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.ILayout.ReadAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadAsync)} > GET /sheet_layouts/:id をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        [Arguments(false, "/sheet_layouts/12", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadAsync)}(12, false) > GET /sheet_layouts/12 をコールする。")]
        [Arguments(true, "/sheet_layouts/12?get_calc_type=true", DisplayName = $"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadAsync)}(12, true) > GET /sheet_layouts/12?get_calc_type=true をコールする。")]
        public async Task Layout_ReadAsync_Calls_GetApi(bool getCalcType, string expectedEndpoint, CancellationToken cancellationToken = default)
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
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet(expectedEndpoint).RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var layout = await sut.Layout.ReadAsync(12, getCalcType, cancellationToken);

            // Assert
            await Assert.That(layout).IsNotNull();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path(expectedEndpoint), Times.Once);
        }
    }
}
