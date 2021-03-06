namespace Kaonavi.Net.Entities;

/// <summary>ロール情報</summary>
/// <param name="Id">ロールID</param>
/// <param name="Name">ロール名</param>
/// <param name="Type">ロールの種別 ("Adm", "一般")</param>
public record Role(int Id, string Name, string Type);
