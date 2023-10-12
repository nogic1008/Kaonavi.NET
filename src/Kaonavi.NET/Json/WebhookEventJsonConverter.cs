using System.Diagnostics.CodeAnalysis;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Json;

/// <summary>
/// <see cref="JsonConverter"/> for <see cref="WebhookEvent"/>
/// </summary>
[ExcludeFromCodeCoverage]
internal class WebhookEventJsonConverter : JsonConverter<WebhookEvent>
{
    private const string MemberCreatedValue = "member_created";
    private const string MemberUpdatedValue = "member_updated";
    private const string MemberDeletedValue = "member_deleted";

    public override WebhookEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetString() switch
        {
            MemberCreatedValue => WebhookEvent.MemberCreated,
            MemberUpdatedValue => WebhookEvent.MemberUpdated,
            MemberDeletedValue => WebhookEvent.MemberDeleted,
            _ => throw new JsonException(),
        };

    public override void Write(Utf8JsonWriter writer, WebhookEvent value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case WebhookEvent.MemberCreated:
                writer.WriteStringValue(MemberCreatedValue);
                break;
            case WebhookEvent.MemberUpdated:
                writer.WriteStringValue(MemberUpdatedValue);
                break;
            case WebhookEvent.MemberDeleted:
                writer.WriteStringValue(MemberDeletedValue);
                break;
            default:
                throw new JsonException();
        }
    }
}
