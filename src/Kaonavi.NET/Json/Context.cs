using Kaonavi.Net.Entities;
using static Kaonavi.Net.KaonaviClient;

namespace Kaonavi.Net.Json;

[JsonSerializable(typeof(ApiListResult<string>))]
[JsonSerializable(typeof(ApiListResult<AdvancedPermission>))]
[JsonSerializable(typeof(ApiListResult<DepartmentTree>))]
[JsonSerializable(typeof(ApiListResult<MemberData>))]
[JsonSerializable(typeof(ApiListResult<EnumOption>))]
[JsonSerializable(typeof(ApiListResult<EnumOptionPayloadData>))]
[JsonSerializable(typeof(ApiListResult<Role>))]
[JsonSerializable(typeof(ApiListResult<SheetData>))]
[JsonSerializable(typeof(ApiListResult<SheetLayout>))]
[JsonSerializable(typeof(ApiListResult<UserWithLoginAt>))]
[JsonSerializable(typeof(ApiListResult<WebhookConfig>))]
[JsonSerializable(typeof(IReadOnlyCollection<AdvancedPermission>))]
[JsonSerializable(typeof(IReadOnlyCollection<DepartmentTree>))]
[JsonSerializable(typeof(IReadOnlyCollection<EnumOption>))]
[JsonSerializable(typeof(IReadOnlyCollection<EnumOptionPayloadData>))]
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
[JsonSerializable(typeof(UserJsonPayload))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    UseStringEnumConverter = true
)]
internal partial class Context : JsonSerializerContext
{
}
