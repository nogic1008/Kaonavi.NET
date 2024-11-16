using Kaonavi.Net.Entities;
using Moq;
using Moq.Contrib.HttpClient;
using RandomFixtureKit;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.AdvancedPermission"/>の単体テスト</summary>
    [TestClass]
    public class AdvancedPermissionTest
    {
        /// <summary>
        /// <paramref name="type"/>が不正な値であるとき、<see cref="KaonaviClient.AdvancedPermission.ListAsync"/>は、ArgumentOutOfRangeExceptionをスローする。
        /// </summary>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        [TestMethod($"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("拡張アクセス設定")]
        [DataRow((AdvancedType)10, DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)}(({nameof(AdvancedType)})10) > ArgumentOutOfRangeExceptionをスローする。")]
        [DataRow((AdvancedType)(-1), DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)}(({nameof(AdvancedType)})-1) > ArgumentOutOfRangeExceptionをスローする。")]
        public async Task When_Type_IsInvalid_AdvancedPermission_ListAsync_Throws_ArgumentOutOfRangeException(AdvancedType type)
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            var act = async () => await sut.AdvancedPermission.ListAsync(type);

            // Assert
            _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>().WithParameterName(nameof(type));
            handler.VerifyAnyRequest(Times.Never());
        }

        /// <summary>
        /// <see cref="KaonaviClient.AdvancedPermission.ListAsync"/>は、"/advanced_permissions/:advancedType"にGETリクエストを行う。
        /// </summary>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        /// <param name="endpoint">呼ばれるAPIエンドポイント</param>
        [TestMethod($"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)} > GET /advanced_permissions/:advancedType をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("拡張アクセス設定")]
        [DataRow(AdvancedType.Member, "/advanced_permissions/member", DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Member)}) > GET /advanced_permissions/member をコールする。")]
        [DataRow(AdvancedType.Department, "/advanced_permissions/department", DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Department)}) > GET /advanced_permissions/department をコールする。")]
        public async Task AdvancedPermission_ListAsync_Calls_GetApi(AdvancedType type, string endpoint)
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "advanced_permission_data": [
                {
                  "user_id": 1,
                  "add_codes": [
                    "0001",
                    "0002",
                    "0003"
                  ],
                  "exclusion_codes": [
                    "0001",
                    "0002",
                    "0003"
                  ]
                },
                {
                  "user_id": 2,
                  "add_codes": [
                    "0001",
                    "0002",
                    "0003"
                  ],
                  "exclusion_codes": [
                    "0001",
                    "0002",
                    "0003"
                  ]
                }
              ]
            }
            """;
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            var permissions = await sut.AdvancedPermission.ListAsync(type);

            // Assert
            _ = permissions.Should().HaveCount(2).And.AllBeAssignableTo<AdvancedPermission>();

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, endpoint)
                    .And.HasToken(token);
                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <paramref name="type"/>が不正な値であるとき、<see cref="KaonaviClient.AdvancedPermission.ReplaceAsync"/>は、ArgumentOutOfRangeExceptionをスローする。
        /// </summary>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        [TestMethod($"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("拡張アクセス設定")]
        [DataRow((AdvancedType)10, DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)}(({nameof(AdvancedType)})10, []) > ArgumentOutOfRangeExceptionをスローする。")]
        [DataRow((AdvancedType)(-1), DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)}(({nameof(AdvancedType)})-1, []) > ArgumentOutOfRangeExceptionをスローする。")]
        public async Task When_Type_IsInvalid_AdvancedPermission_ReplaceAsync_Throws_ArgumentOutOfRangeException(AdvancedType type)
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            var act = async () => await sut.AdvancedPermission.ReplaceAsync(type, []);

            // Assert
            _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>().WithParameterName(nameof(type));
            handler.VerifyAnyRequest(Times.Never());
        }

        /// <summary>
        /// <see cref="KaonaviClient.AdvancedPermission.ReplaceAsync"/>は、"/advanced_permissions/:advancedType"にPUTリクエストを行う。
        /// </summary>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        /// <param name="endpoint">呼ばれるAPIエンドポイント</param>
        [TestMethod($"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)} > PUT /advanced_permissions/:advancedType をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("拡張アクセス設定")]
        [DataRow(AdvancedType.Member, "/advanced_permissions/member", DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Member)}) > PUT /advanced_permissions/member をコールする。")]
        [DataRow(AdvancedType.Department, "/advanced_permissions/department", DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Department)}) > PUT /advanced_permissions/department をコールする。")]
        public async Task AdvancedPermission_ReplaceAsync_Calls_PutApi(AdvancedType type, string endpoint)
        {
            // Arrange
            string token = FixtureFactory.Create<string>();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: token);
            int taskId = await sut.AdvancedPermission.ReplaceAsync(type,
            [
                new(1, ["1"], []),
                new(2, ["2"], ["1", "3"]),
            ]);

            // Assert
            _ = taskId.Should().Be(TaskId);

            handler.VerifyRequest(async req =>
            {
                _ = req.Should().SendTo(HttpMethod.Put, endpoint)
                    .And.HasToken(token);

                // Body
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = receivedJson.Should()
                    .Be(/*lang=json,strict*/ """{"advanced_permission_data":[{"user_id":1,"add_codes":["1"],"exclusion_codes":[]},{"user_id":2,"add_codes":["2"],"exclusion_codes":["1","3"]}]}""");

                return true;
            }, Times.Once());
        }
    }
}
