using Kaonavi.Net.Entities;
using static Kaonavi.Net.KaonaviClient;

namespace Kaonavi.Net.Json;

[JsonSerializable(typeof(IReadOnlyList<AdvancedPermission>))]
[JsonSerializable(typeof(IReadOnlyList<Attachment>))]
[JsonSerializable(typeof(IReadOnlyList<DepartmentTree>))]
[JsonSerializable(typeof(IReadOnlyList<EnumOption>))]
[JsonSerializable(typeof(IReadOnlyList<EnumOptionPayloadData>))]
[JsonSerializable(typeof(IReadOnlyList<FaceImage>))]
[JsonSerializable(typeof(IReadOnlyList<JsonElement>))]
[JsonSerializable(typeof(IReadOnlyList<MemberData>))]
[JsonSerializable(typeof(IReadOnlyList<Role>))]
[JsonSerializable(typeof(IReadOnlyList<SheetData>))]
[JsonSerializable(typeof(IReadOnlyList<SheetLayout>))]
[JsonSerializable(typeof(IReadOnlyList<User>))]
[JsonSerializable(typeof(IReadOnlyList<WebhookConfig>))]
[JsonSerializable(typeof(MemberLayout))]
[JsonSerializable(typeof(TaskProgress))]
[JsonSerializable(typeof(Token))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(UserPayload))]
[JsonSerializable(typeof(WebhookConfigPayload))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    UseStringEnumConverter = true
)]
internal partial class Context : JsonSerializerContext;
