using System.Text.Json.Serialization.Metadata;

namespace Kaonavi.Net.Json;

/// <summary>
/// <c>""</c>を<see langword="null"/>とみなす<see cref="DateOnly"/>?用のJsonConverter実装。
/// </summary>
public class BlankDateOnlyConverter : JsonConverter<DateOnly?>
{
    /// <inheritdoc/>
    public override bool HandleNull => true;

    /// <inheritdoc/>
    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => string.IsNullOrEmpty(reader.GetString()) ? null : JsonMetadataServices.DateOnlyConverter.Read(ref reader, typeToConvert, options);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            JsonMetadataServices.DateOnlyConverter.Write(writer, value.GetValueOrDefault(), options);
        else
            writer.WriteStringValue(""u8);
    }
}
