using Kaonavi.Net.Json;

namespace Kaonavi.Net.Entities;

/// <summary>
/// カオナビAPIの返す、「配列をラップしたJSONオブジェクト」を表したエンティティ。
/// </summary>
/// <typeparam name="T">配列の中身を表すエンティティ</typeparam>
/// <param name="PropertyName">配列が格納されたJSONのプロパティ名</param>
/// <param name="Values">配列データ</param>
[JsonConverter(typeof(ApiListResultConverterFactory))]
internal record ApiListResult<T>(string PropertyName, IReadOnlyList<T> Values);

/// <inheritdoc/>
internal class ApiListResultConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(ApiListResult<>);

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter?)Activator.CreateInstance(typeof(ApiListResultConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()));

    /// <inheritdoc/>
    private class ApiListResultConverter<T> : JsonConverter<ApiListResult<T>>
    {
        /// <inheritdoc/>
        public override ApiListResult<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            var prop = json.EnumerateObject().First(d => d.Value.ValueKind == JsonValueKind.Array);
            return new(prop.Name, prop.Value.Deserialize<IReadOnlyList<T>>(options)!);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ApiListResult<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(value.PropertyName);
            JsonSerializer.Serialize(writer, value.Values, options);
            writer.WriteEndObject();
        }
    }
}
