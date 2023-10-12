using Kaonavi.Net.Services;

namespace Kaonavi.Net.Entities;

[JsonSerializable(typeof(IReadOnlyCollection<DepartmentTree>))]
[JsonSerializable(typeof(IReadOnlyCollection<EnumOption>))]
[JsonSerializable(typeof(IReadOnlyCollection<JsonElement>))]
[JsonSerializable(typeof(IReadOnlyCollection<MemberData>))]
[JsonSerializable(typeof(IReadOnlyCollection<Role>))]
[JsonSerializable(typeof(IReadOnlyCollection<SheetData>))]
[JsonSerializable(typeof(IReadOnlyCollection<SheetLayout>))]
[JsonSerializable(typeof(IReadOnlyCollection<UserWithLoginAt>))]
[JsonSerializable(typeof(IReadOnlyCollection<WebhookConfig>))]
[JsonSerializable(typeof(MemberLayout))]
[JsonSerializable(typeof(TaskProgress))]
[JsonSerializable(typeof(Token))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(WebhookConfigPayload))]
[JsonSerializable(typeof(KaonaviV2Service.ApiResult<MemberData>))]
[JsonSerializable(typeof(KaonaviV2Service.ApiResult<SheetData>))]
[JsonSerializable(typeof(KaonaviV2Service.DeleteMemberDataPayload))]
[JsonSerializable(typeof(KaonaviV2Service.DepartmentsResult))]
[JsonSerializable(typeof(KaonaviV2Service.EnumOptionPayload))]
[JsonSerializable(typeof(KaonaviV2Service.UserJsonPayload))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower
)]
internal partial class Context : JsonSerializerContext
{
}
