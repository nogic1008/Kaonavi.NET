namespace Kaonavi.Net.Services;

using System.Reflection;

internal record ApiResult<T>(string PropertyName, IReadOnlyList<T> Data);

internal class JsonConverterFactoryForApiResult : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(ApiResult<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];

        return (JsonConverter)Activator.CreateInstance(
            typeof(ApiResultJsonConverter<>)
                .MakeGenericType(new Type[] { elementType }),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;
    }
}

/// <inheritdoc/>
internal class ApiResultJsonConverter<T> : JsonConverter<ApiResult<T>>
{
    /// <inheritdoc/>
    public override ApiResult<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        IReadOnlyList<T>? data = null;
        string? propertyName = null;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString()?.Contains("_data") == true)
            {
                propertyName = reader.GetString()!;
                data = JsonSerializer.Deserialize<IReadOnlyList<T>>(ref reader, options);
            }
        }
        return data is not null ? new(propertyName!, data) : throw new JsonException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ApiResult<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(value.PropertyName);
        JsonSerializer.Serialize(writer, value.Data, options);
        writer.WriteEndObject();
    }
}
