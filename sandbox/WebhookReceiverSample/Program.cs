using Kaonavi.Net.Server;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/webhook", ([FromHeader(Name = "Kaonavi-Token")] string token, KaonaviWebhook webhook, IConfiguration configuration) =>
{
    // Kaonavi-Token ヘッダーがカオナビで設定したものと一致しているか検証する
    if (string.IsNullOrEmpty(token) || token != configuration["KaonaviToken"])
        return Results.Unauthorized();

    // webhook データを利用した何かしらの処理 (10秒以内に完了することを求めているため、重い処理は別途非同期で行うこと)
    Console.WriteLine(webhook);

    // ステータスコード 200 系を返す
    return Results.Ok();
});

app.MapPost("/webhook2", (HttpRequest request, KaonaviWebhook webhook, IConfiguration configuration) =>
{
    // リクエストがカオナビの webhook であるか厳密に検証する
    // (Kaonavi-Token のほか、User-AgentとContent-Typeも検証する)
    if (!request.IsKaonaviWebhookRequest(configuration["KaonaviToken"]!))
        return Results.Unauthorized();

    // webhook データを利用した何かしらの処理 (10秒以内に完了することを求めているため、重い処理は別途非同期で行うこと)
    Console.WriteLine(webhook);

    // ステータスコード 200 系を返す
    return Results.Ok();
});

app.Run();
