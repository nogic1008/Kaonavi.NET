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
[JsonSerializable(typeof(IReadOnlyList<AdvancedPermission>))]
[JsonSerializable(typeof(IReadOnlyList<Attachment>))]
[JsonSerializable(typeof(IReadOnlyList<DepartmentTree>))]
[JsonSerializable(typeof(IReadOnlyList<EnumOption>))]
[JsonSerializable(typeof(IReadOnlyList<EnumOptionPayloadData>))]
[JsonSerializable(typeof(IReadOnlyList<JsonElement>))]
[JsonSerializable(typeof(IReadOnlyList<MemberData>))]
[JsonSerializable(typeof(IReadOnlyList<Role>))]
[JsonSerializable(typeof(IReadOnlyList<SheetData>))]
[JsonSerializable(typeof(IReadOnlyList<SheetLayout>))]
[JsonSerializable(typeof(IReadOnlyList<UserWithLoginAt>))]
[JsonSerializable(typeof(IReadOnlyList<WebhookConfig>))]
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
internal partial class Context : JsonSerializerContext;
