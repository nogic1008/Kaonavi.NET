namespace Kaonavi.Net.Entities;

using System.Text.Json.Serialization;

/// <summary>ロール情報</summary>
/// <param name="Id">ロールID</param>
/// <param name="Name">ロール名</param>
/// <param name="Type">ロールの種別 ("Adm", "一般")</param>
public record Role(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type
);
