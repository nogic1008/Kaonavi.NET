using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Layout"/>の単体テスト</summary>
    [TestClass]
    public class LayoutTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Layout.ReadMemberLayoutAsync"/>は、"/member_layouts"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadMemberLayoutAsync)} > GET /member_layouts をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("レイアウト設定")]
        public async Task Layout_ReadMemberLayoutAsync_Calls_GetApi()
        {
            string tokenString = GenerateRandomString();
            var response = new MemberLayout(
                new("社員番号", true, FieldType.String, 50, []),
                new("氏名", false, FieldType.String, 100, []),
                new("フリガナ", false, FieldType.String, 100, []),
                new("メールアドレス", false, FieldType.String, 100, []),
                new("入社日", false, FieldType.Date, null, []),
                new("退職日", false, FieldType.Date, null, []),
                new("性別", false, FieldType.Enum, null, ["男性", "女性"]),
                new("生年月日", false, FieldType.Date, null, []),
                new("所属", false, FieldType.Department, null, []),
                new("兼務情報", false, FieldType.DepartmentArray, null, []),
                [
                    new(100, "血液型", false, FieldType.Enum, null, ["A", "B", "O", "AB"]),
                    new(200, "役職", false, FieldType.Enum, null, ["部長", "課長", "マネージャー", null]),
                ]
            );

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/member_layouts")
                .ReturnsJsonResponse(HttpStatusCode.OK, response, Context.Default.Options);

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var layout = await sut.Layout.ReadMemberLayoutAsync();

            // Assert
            _ = layout.Should().NotBeNull();
            _ = layout.Code.Name.Should().Be("社員番号");
            _ = layout.Name.Name.Should().Be("氏名");
            _ = layout.NameKana.Name.Should().Be("フリガナ");
            _ = layout.Mail.Name.Should().Be("メールアドレス");
            _ = layout.EnteredDate.Name.Should().Be("入社日");
            _ = layout.RetiredDate.Name.Should().Be("退職日");
            _ = layout.Gender.Name.Should().Be("性別");
            _ = layout.Birthday.Name.Should().Be("生年月日");
            _ = layout.Department.Name.Should().Be("所属");
            _ = layout.SubDepartments.Name.Should().Be("兼務情報");
            _ = layout.CustomFields.Should().HaveCount(2);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, "/member_layouts")
                    .And.HasToken(tokenString);
                return true;
            }, Times.Once());
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
        public async Task Layout_ListAsync_Calls_GetApi(bool getCalcType, string expectedEndpoint)
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
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupAnyRequest()
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var layouts = await sut.Layout.ListAsync(getCalcType);

            // Assert
            _ = layouts.Should().HaveCount(1);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, expectedEndpoint)
                    .And.HasToken(tokenString);
                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.ILayout.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Layout.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Layout)}.{nameof(KaonaviClient.Layout.ReadAsync)}(-1) > ArgumentOutOfRangeException をスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("レイアウト設定")]
        public async Task When_Id_IsNegative_Layout_ReadAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            var act = async () => _ = await sut.Layout.ReadAsync(-1);

            // Assert
            _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithParameterName("id");

            handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
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
        public async Task Layout_ReadAsync_Calls_GetApi(bool getCalcType, string expectedEndpoint)
        {
            // Arrange
            const int sheetId = 12;
            string tokenString = GenerateRandomString();
            var response = new SheetLayout(
                sheetId,
                "住所・連絡先",
                RecordType.Multiple,
                [new(1000, "住所", false, FieldType.String, 250, []), new(1001, "電話番号", false, FieldType.String, 50, [])]
            );

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupAnyRequest()
                .ReturnsJsonResponse(HttpStatusCode.OK, response, Context.Default.Options);

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var layout = await sut.Layout.ReadAsync(12, getCalcType);

            // Assert
            _ = layout.Should().NotBeNull();
            _ = layout.Id.Should().Be(sheetId);
            _ = layout.Name.Should().Be("住所・連絡先");
            _ = layout.RecordType.Should().Be(RecordType.Multiple);
            _ = layout.CustomFields.Should().HaveCount(2);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, expectedEndpoint)
                    .And.HasToken(tokenString);
                return true;
            }, Times.Once());
        }
    }
}
