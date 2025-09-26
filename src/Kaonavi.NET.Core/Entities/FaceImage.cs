namespace Kaonavi.Net.Entities;

/// <summary>
/// メンバー情報 顔写真(アップロード用)
/// </summary>
/// <param name="Code">社員番号</param>
/// <param name="Content">顔写真画像(5MBまで)</param>
public record FaceImage(string Code, [property: JsonPropertyName("base64_face_image")] byte[] Content);
