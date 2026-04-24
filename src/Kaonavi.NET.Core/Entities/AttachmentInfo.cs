using Kaonavi.Net.Json;

namespace Kaonavi.Net.Entities;

/// <summary>添付ファイル情報のリスト</summary>
/// <param name="Code">社員番号</param>
/// <param name="Records">添付ファイルのリスト</param>
public record AttachmentInfo(string Code, params IReadOnlyList<AttachmentInfoRecord> Records);

/// <summary>添付ファイル情報</summary>
/// <param name="FileName">ファイル名</param>
/// <param name="DownloadUrl">ダウンロードするためのURL</param>
/// <param name="UpdatedAt">ファイルの最終更新日時</param>
public record AttachmentInfoRecord(string FileName, Uri DownloadUrl, [property: JsonConverter(typeof(DateTimeConverter))] DateTime UpdatedAt);
