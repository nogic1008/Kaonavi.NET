namespace Kaonavi.Net.Entities;

/// <summary>アクセストークン</summary>
/// <param name="AccessToken">アクセストークン</param>
/// <param name="TokenType">トークン種別(<c>"Bearer"</c>固定)</param>
/// <param name="ExpiresIn">トークンの有効期限 (秒)</param>
public record Token(string AccessToken, string TokenType, int ExpiresIn);
