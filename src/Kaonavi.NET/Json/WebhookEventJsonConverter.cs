using System.Diagnostics.CodeAnalysis;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Json;

/// <summary>
/// <see cref="JsonConverter"/> for <see cref="WebhookEvent"/>
/// </summary>
[ExcludeFromCodeCoverage]
internal class WebhookEventJsonConverter : JsonConverter<WebhookEvent>
{
    private static ReadOnlySpan<byte> MemberCreated => "member_created"u8;
    private static ReadOnlySpan<byte> MemberUpdated => "member_updated"u8;
    private static ReadOnlySpan<byte> MemberDeleted => "member_deleted"u8;

    public override WebhookEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.ValueSpan.SequenceEqual(MemberCreated) ? WebhookEvent.MemberCreated
        : reader.ValueSpan.SequenceEqual(MemberUpdated) ? WebhookEvent.MemberUpdated
        : reader.ValueSpan.SequenceEqual(MemberDeleted) ? WebhookEvent.MemberDeleted
        : throw new JsonException();

    public override void Write(Utf8JsonWriter writer, WebhookEvent value, JsonSerializerOptions options)
        => writer.WriteStringValue(value switch
        {
            WebhookEvent.MemberCreated => MemberCreated,
            WebhookEvent.MemberUpdated => MemberUpdated,
            WebhookEvent.MemberDeleted => MemberDeleted,
            _ => throw new JsonException(),
        });
}
