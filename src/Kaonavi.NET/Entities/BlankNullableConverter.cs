namespace Kaonavi.Net;

/// <summary>
/// <c>""</c>を<c>null</c>とみなす<see cref="Nullable{T}"/>用のJsonConverter実装。
/// </summary>
public class BlankNullableConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>)
        && typeToConvert.GetGenericArguments()[0].IsValueType;

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter?)Activator.CreateInstance(typeof(BlankNullableConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()));

    private class BlankNullableConverter<T> : JsonConverter<T?> where T : struct
    {
        private static readonly JsonConverter<T> _defaultConverter
            = (JsonConverter<T>)JsonSerializerOptions.Default.GetConverter(typeof(T));

        /// <inheritdoc/>
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => string.IsNullOrEmpty(reader.GetString()) ? null : _defaultConverter.Read(ref reader, typeToConvert, options);

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                _defaultConverter.Write(writer, value.GetValueOrDefault(), options);
            else
                writer.WriteStringValue(""u8);
        }
    }
}
