using Kaonavi.Net.Json;

namespace Kaonavi.Net.Entities;

/// <summary>
/// メンバー情報 顔写真(ダウンロード用)
/// </summary>
/// <param name="Code">社員番号</param>
/// <param name="FileName">ファイル名</param>
/// <param name="DownloadUrl">ダウンロードするためのURL</param>
/// <param name="UpdatedAt">顔写真の最終更新日時</param>
public record FaceImageInfo(
    string Code,
    string FileName,
    Uri DownloadUrl,
    [property: JsonConverter(typeof(DateTimeConverter))] DateTime UpdatedAt
);
