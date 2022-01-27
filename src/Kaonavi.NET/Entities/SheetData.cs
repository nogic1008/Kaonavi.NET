namespace Kaonavi.Net.Entities;

/// <summary>シート情報</summary>
public record SheetData
{
    /// <summary>
    /// 単一レコードシート向けに、SheetDataの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="code"><inheritdoc cref="Code" path="/summary/text()"/></param>
    /// <param name="customFields">設定値</param>
    public SheetData(string code, IReadOnlyList<CustomFieldValue> customFields)
        : this(code, new[] { customFields }) { }

    /// <summary>
    /// 複数レコードシート向けに、SheetDataの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="code"><inheritdoc cref="Code" path="/summary/text()"/></param>
    /// <param name="records"><inheritdoc cref="Records" path="/summary/text()"/></param>
    public SheetData(string code, params IReadOnlyList<CustomFieldValue>[] records)
        => (Code, Records) = (code, records);

    /// <inheritdoc cref="SheetData(string, IReadOnlyList{CustomFieldValue}[])"/>
    [JsonConstructor]
    public SheetData(string code, IReadOnlyList<IReadOnlyList<CustomFieldValue>> records)
        => (Code, Records) = (code, records);

    /// <summary>社員コード</summary>
    /// [JsonPropertyName("code")]
    public string Code { get; init; }

    /// <summary>メンバーが持つ設定値のリスト</summary>
    /// <remarks><see cref="RecordType.Multiple"/>の場合にのみ複数の値が返却されます。</remarks>
    [JsonPropertyName("records")]
    [JsonConverter(typeof(SheetRecordConverter))]
    public IReadOnlyList<IReadOnlyList<CustomFieldValue>> Records { get; init; }
}

internal class SheetRecordConverter : JsonConverter<IReadOnlyList<IReadOnlyList<CustomFieldValue>>>
{
    private const string PropertyName = "custom_fields";

    public override IReadOnlyList<IReadOnlyList<CustomFieldValue>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<IReadOnlyList<JsonElement>>(ref reader, options)!
            .Select(d =>
                d.GetProperty(PropertyName)
                    .EnumerateArray()
                    .Select(el => el.Deserialize<CustomFieldValue>(options)!)
                    .ToArray()
            )
            .ToArray();

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<IReadOnlyList<CustomFieldValue>> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var record in value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(PropertyName);
            JsonSerializer.Serialize(writer, record, options);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
