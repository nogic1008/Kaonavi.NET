namespace Kaonavi.Net.Entities;

/// <summary>シート情報</summary>
/// <param name="Code">社員コード</param>
/// <param name="Records">
/// メンバーが持つ設定値のリスト
/// <para><see cref="RecordType.Multiple"/>の場合にのみ複数の値が返却されます。</para>
/// </param>
[method: JsonConstructor]
public record SheetData(
    string Code,
    [property: JsonConverter(typeof(SheetRecordConverter))] params IReadOnlyList<IReadOnlyList<CustomFieldValue>> Records
)
{
    /// <summary>
    /// 単一レコードシート向けに、SheetDataの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="code"><inheritdoc cref="Code" path="/summary/text()"/></param>
    /// <param name="customFields">設定値</param>
    public SheetData(string code, IReadOnlyList<CustomFieldValue> customFields)
        : this(code, [customFields]) { }
}

internal class SheetRecordConverter : JsonConverter<IReadOnlyList<IReadOnlyList<CustomFieldValue>>>
{
    private static ReadOnlySpan<byte> PropertyName => "custom_fields"u8;

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
