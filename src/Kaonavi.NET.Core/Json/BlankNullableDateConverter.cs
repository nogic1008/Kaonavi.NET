namespace Kaonavi.Net.Json;

/// <summary>
/// <c>""</c>を<c>null</c>とみなす<see cref="Nullable{DateOnly}"/>用のJsonConverter実装。
/// </summary>
public class BlankNullableDateConverter : JsonConverter<DateOnly?>
{
    private readonly BlankNullableConverter<DateOnly> _converter = new(new DateOnlyConverter());

    /// <inheritdoc/>
    public override bool HandleNull => _converter.HandleNull;

    /// <inheritdoc/>
    public override DateOnly? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) => _converter.Read(ref reader, typeToConvert, options);

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        DateOnly? value,
        JsonSerializerOptions options
    ) => _converter.Write(writer, value, options);
}
