using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Json;

[JsonSerializable(typeof(IReadOnlyList<AdvancedPermission>))]
[JsonSerializable(typeof(IReadOnlyList<Attachment>))]
[JsonSerializable(typeof(IReadOnlyList<DepartmentTree>))]
[JsonSerializable(typeof(IReadOnlyList<EnumOption>))]
[JsonSerializable(typeof(IReadOnlyList<FaceImage>))]
[JsonSerializable(typeof(IReadOnlyList<FaceImageInfo>))]
[JsonSerializable(typeof(IReadOnlyList<JsonElement>))]
[JsonSerializable(typeof(IReadOnlyList<MemberData>))]
[JsonSerializable(typeof(IReadOnlyList<Role>))]
[JsonSerializable(typeof(IReadOnlyList<SheetData>))]
[JsonSerializable(typeof(IReadOnlyList<SheetLayout>))]
[JsonSerializable(typeof(IReadOnlyList<User>))]
[JsonSerializable(typeof(IReadOnlyList<WebhookConfig>))]
[JsonSerializable(typeof(IReadOnlyList<(int? id, string name)>), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MemberLayout))]
[JsonSerializable(typeof(TaskProgress))]
[JsonSerializable(typeof(Token))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(UserPayload))]
[JsonSerializable(typeof(WebhookConfigPayload))]
#pragma warning disable CS3016 // CLS Compliant
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    UseStringEnumConverter = true,
    Converters = [typeof(EnumOptionPayloadConverter)]
)]
#pragma warning restore CS3016 // CLS Compliant
internal partial class Context : JsonSerializerContext;
