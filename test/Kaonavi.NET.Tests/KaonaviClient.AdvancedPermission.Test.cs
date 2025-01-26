using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.AdvancedPermission"/>の単体テスト</summary>
    [TestClass]
    public sealed class AdvancedPermissionTest
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => await sut.AdvancedPermission.ListAsync(type);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe(nameof(type));
            mockedApi.ShouldNotBeCalled();
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var permissions = await sut.AdvancedPermission.ListAsync(type);

            // Assert
            permissions.ShouldNotBeEmpty();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                req => req.RequestUri?.PathAndQuery.ShouldBe(endpoint)
            );
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => await sut.AdvancedPermission.ReplaceAsync(type, []);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe(nameof(type));
            mockedApi.ShouldNotBeCalled();
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            int taskId = await sut.AdvancedPermission.ReplaceAsync(type,
            [
                new(1, ["1"], []),
                new(2, ["2"], ["1", "3"]),
            ]);

            // Assert
            taskId.ShouldBe(TaskId);
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Put),
                req => req.RequestUri?.PathAndQuery.ShouldBe(endpoint),
                static req => req.Content!.ShouldHaveJsonBody("""
                {
                  "advanced_permission_data": [
                    {
                      "user_id": 1,
                      "add_codes": ["1"],
                      "exclusion_codes": []
                    },
                    {
                      "user_id": 2,
                      "add_codes": ["2"],
                      "exclusion_codes": ["1", "3"]
                    }
                  ]
                }
                """)
            );
        }
    }
}
