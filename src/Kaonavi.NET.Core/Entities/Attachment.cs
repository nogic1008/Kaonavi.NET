using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaonavi.Net.Entities;

/// <summary>添付ファイル</summary>
public record Attachment
{
    /// <summary>
    /// 単一レコードシート向けに、Attachmentの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="code"><inheritdoc cref="Code" path="/summary"/></param>
    /// <param name="record">設定値</param>
    public Attachment(string code, Record record)
        : this(code, [record]) { }

    /// <summary>
    /// 複数レコードシート向けに、Attachmentの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="code"><inheritdoc cref="Code" path="/summary"/></param>
    /// <param name="records"><inheritdoc cref="Records" path="/summary"/></param>
    public Attachment(string code, params Record[] records)
        => (Code, Records) = (code, records);

    /// <inheritdoc cref="Attachment(string, Record[])"/>
    [JsonConstructor]
    public Attachment(string code, IReadOnlyList<Record> records)
        => (Code, Records) = (code, records);

    /// <summary>社員番号</summary>
    public string Code { get; init; }

    /// <summary>メンバーが持つ設定値のリスト</summary>
    /// <remarks><see cref="RecordType.Multiple"/>の場合にのみ複数の値が返却されます。</remarks>
    public IReadOnlyList<Record> Records { get; init; }

    /// <summary>添付ファイル情報</summary>
    /// <param name="FileName">ファイル名</param>
    /// <param name="Content">ファイルデータ</param>
    public record Record(string FileName, [property: JsonPropertyName("base64_content")] byte[] Content);
}
