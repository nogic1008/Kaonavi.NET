using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.AdvancedPermission"/>の単体テスト</summary>
    [Category("API"), Category("拡張アクセス設定")]
    public sealed class AdvancedPermissionTest
    {
        /// <summary>
        /// <paramref name="type"/>が不正な値であるとき、<see cref="KaonaviClient.AdvancedPermission.ListAsync"/>は、ArgumentOutOfRangeExceptionをスローする。
        /// </summary>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IAdvancedPermission.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [Category(nameof(HttpMethod.Get))]
        [Arguments((AdvancedType)10, DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)}(({nameof(AdvancedType)})10) > ArgumentOutOfRangeExceptionをスローする。")]
        [Arguments((AdvancedType)(-1), DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)}(({nameof(AdvancedType)})-1) > ArgumentOutOfRangeExceptionをスローする。")]
        public async Task When_Type_IsInvalid_AdvancedPermission_ListAsync_Throws_ArgumentOutOfRangeException(AdvancedType type, CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => await sut.AdvancedPermission.ListAsync(type, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName(nameof(type));
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.AdvancedPermission.ListAsync"/>は、"/advanced_permissions/:advancedType"にGETリクエストを行う。
        /// </summary>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        /// <param name="endpoint">呼ばれるAPIエンドポイント</param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IAdvancedPermission.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)} > GET /advanced_permissions/:advancedType をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        [Arguments(AdvancedType.Member, "/advanced_permissions/member", DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Member)}) > GET /advanced_permissions/member をコールする。")]
        [Arguments(AdvancedType.Department, "/advanced_permissions/department", DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ListAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Department)}) > GET /advanced_permissions/department をコールする。")]
        public async Task AdvancedPermission_ListAsync_Calls_GetApi(AdvancedType type, string endpoint, CancellationToken cancellationToken = default)
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
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet(endpoint).RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var permissions = await sut.AdvancedPermission.ListAsync(type, cancellationToken);

            // Assert
            await Assert.That(permissions).IsNotEmpty();
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path(endpoint), Times.Once);
        }

        /// <summary>
        /// <paramref name="type"/>が不正な値であるとき、<see cref="KaonaviClient.AdvancedPermission.ReplaceAsync"/>は、ArgumentOutOfRangeExceptionをスローする。
        /// </summary>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IAdvancedPermission.ReplaceAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [Category(nameof(HttpMethod.Put))]
        [Arguments((AdvancedType)10, DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)}(({nameof(AdvancedType)})10, []) > ArgumentOutOfRangeExceptionをスローする。")]
        [Arguments((AdvancedType)(-1), DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)}(({nameof(AdvancedType)})-1, []) > ArgumentOutOfRangeExceptionをスローする。")]
        public async Task When_Type_IsInvalid_AdvancedPermission_ReplaceAsync_Throws_ArgumentOutOfRangeException(AdvancedType type, CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => await sut.AdvancedPermission.ReplaceAsync(type, [], cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName(nameof(type));
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.AdvancedPermission.ReplaceAsync"/>は、"/advanced_permissions/:advancedType"にPUTリクエストを行う。
        /// </summary>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        /// <param name="endpoint">呼ばれるAPIエンドポイント</param>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IAdvancedPermission.ReplaceAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)} > PUT /advanced_permissions/:advancedType をコールする。")]
        [Category(nameof(HttpMethod.Put))]
        [Arguments(AdvancedType.Member, "/advanced_permissions/member", DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Member)}) > PUT /advanced_permissions/member をコールする。")]
        [Arguments(AdvancedType.Department, "/advanced_permissions/department", DisplayName = $"{nameof(KaonaviClient.AdvancedPermission)}.{nameof(KaonaviClient.AdvancedPermission.ReplaceAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Department)}) > PUT /advanced_permissions/department をコールする。")]
        public async Task AdvancedPermission_ReplaceAsync_Calls_PutApi(AdvancedType type, string endpoint, CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPut(endpoint).RespondWithJson(TaskJson);

            // Act
            var sut = CreateSut(client, "token");
            int taskId = await sut.AdvancedPermission.ReplaceAsync(type,
            [
                new(1, ["1"], []),
                new(2, ["2"], ["1", "3"]),
            ], cancellationToken);

            // Assert
            await Assert.That(taskId).IsEqualTo(TaskId);
            client.Handler.Verify(r => r.Method(HttpMethod.Put).Path(endpoint), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsJsonEquals("""
            {
              "advanced_permission_data": [
                { "user_id": 1, "add_codes": ["1"], "exclusion_codes": [] },
                { "user_id": 2, "add_codes": ["2"], "exclusion_codes": ["1", "3"] }
              ]
            }
            """u8);
        }
    }
}
