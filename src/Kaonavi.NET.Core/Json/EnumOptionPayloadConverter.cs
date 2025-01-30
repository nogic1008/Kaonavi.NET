using System.Diagnostics.CodeAnalysis;

namespace Kaonavi.Net.Json;

/// <summary>
/// (id, name) -> { id: 1, name: "name" }変換
/// </summary>
internal class EnumOptionPayloadConverter : JsonConverter<IReadOnlyList<(int? id, string name)>>
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override IReadOnlyList<(int? id, string name)>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, IReadOnlyList<(int? id, string name)> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var (id, name) in value)
        {
            writer.WriteStartObject();
            if (id.HasValue)
                writer.WriteNumber("id"u8, id.GetValueOrDefault());
            writer.WriteString("name"u8, name);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
