namespace Kaonavi.Net.Entities;

/// <summary>添付ファイル</summary>
/// <param name="Code">社員番号</param>
/// <param name="Records">
/// メンバーが持つ設定値のリスト
/// <para><see cref="RecordType.Multiple"/>の場合にのみ複数の値が返却されます。</para>
/// </param>
[method: JsonConstructor]
public record AttachmentPayload(string Code, params IReadOnlyList<AttachmentPayload.Record> Records)
{
    /// <summary>
    /// 単一レコードシート向けに、Attachmentの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="code"><inheritdoc cref="Code" path="/summary"/></param>
    /// <param name="record">設定値</param>
    public AttachmentPayload(string code, Record record)
        : this(code, [record]) { }

    /// <summary>添付ファイル情報</summary>
    /// <param name="FileName">ファイル名</param>
    /// <param name="Content">ファイルデータ</param>
    public record Record(string FileName, [property: JsonPropertyName("base64_content")] byte[] Content);
}
