using Microsoft.AspNetCore.Http;

namespace Kaonavi.Net.Server
{
    /// <summary>
    /// <see cref="HttpRequest"/>の拡張メソッド
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// kaonavi Webhookのリクエストかどうかを判定します。
        /// </summary>
        /// <param name="request">リクエスト情報</param>
        /// <param name="token">検証用トークン</param>
        public static bool IsKaonaviWebhookRequest(this HttpRequest request, string token)
            => request.ContentType == "application/json"
            && request.Headers.UserAgent.Any(s => s is not null && s.Contains("Kaonavi-Webhook"))
            && request.Headers.TryGetValue("Kaonavi-Token", out var values) && values.Contains(token);
    }
}
