namespace Kaonavi.Net.Json;

/// <summary>
/// <c>""</c>を<c>null</c>とみなす<see cref="Nullable{T}"/>用のJsonConverter実装。
/// </summary>
/// <typeparam name="T">変換対象となる値型。</typeparam>
/// <param name="converter">値型の変換に使用するJsonConverter。</param>
internal class BlankNullableConverter<T>(JsonConverter<T> converter) : JsonConverter<T?>
    where T : struct
{
    /// <inheritdoc/>
    public override bool HandleNull => true;

    /// <inheritdoc/>
    public override T? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) =>
        reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String when reader.ValueTextEquals(""u8) => null,
            _ => converter.Read(ref reader, typeof(T), options),
        };

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            converter.Write(writer, value.GetValueOrDefault(), options);
        else
            writer.WriteStringValue(""u8);
    }
}
