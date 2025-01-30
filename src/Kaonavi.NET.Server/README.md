# Kaonavi.NET.Server

[![NuGet (stable version)](https://img.shields.io/nuget/v/Kaonavi.NET.Server?logo=nuget)](https://www.nuget.org/packages/Kaonavi.NET.Server/)
[![GitHub releases (including pre-releases)](https://img.shields.io/github/v/release/nogic1008/Kaonavi.NET?include_prereleases&sort=semver)](https://github.com/nogic1008/Kaonavi.NET/releases)
[![.NET CI](https://github.com/nogic1008/Kaonavi.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/nogic1008/Kaonavi.NET/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/nogic1008/Kaonavi.NET/branch/main/graph/badge.svg?token=DK9S9TJtgj)](https://codecov.io/gh/nogic1008/Kaonavi.NET)
[![CodeFactor](https://www.codefactor.io/repository/github/nogic1008/Kaonavi.NET/badge)](https://www.codefactor.io/repository/github/nogic1008/Kaonavi.NET)
[![License](https://img.shields.io/github/license/nogic1008/Kaonavi.NET)](https://github.com/nogic1008/Kaonavi.NET/blob/main/LICENSE)

Server side parser for Kaonavi Webhook

> [!WARNING]
> 現状、メンテナーがカオナビAPIの利用権を持っていないため、実際の動作確認ができていません。
> ご利用の際は自己責任でお願いいたします。

## Install

```powershell
# Package Manager
> Install-Package Kaonavi.NET.Server

# .NET CLI
> dotnet add package Kaonavi.NET.Server
```

## Usage

事前に[公式APIドキュメント](https://developer.kaonavi.jp/api/v2.0/index.html#section/Webhookb)の手順に従い、Webhookの利用設定を行ってください。

```csharp
// ASP.NET Minimal APIでの利用例
using Kaonavi.Net.Server;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/webhook", ([FromHeader(Name = "Kaonavi-Token")] string token, KaonaviWebhook webhook) =>
{
    // Kaonavi-Token ヘッダーの検証
    // ("YOUR_KAONAVI_TOKEN" は事前に設定したトークン。実際の値は環境変数などから取得することを推奨)
    if (string.IsNullOrEmpty(token) || token != "YOUR_KAONAVI_TOKEN")
        return Results.Unauthorized();

    // もしくは、引数でHttpRequestを受け取り、User-AgentとContent-Typeも検証する
    // if (!request.IsKaonaviWebhookRequest("YOUR_KAONAVI_TOKEN"))
    //     return Results.Unauthorized();

    // webhook データを利用した処理 (10秒以内に完了すること)
    Console.WriteLine(webhook);

    // ステータスコード 200 系を返す
    return Results.Ok();
});
```

> [!TIP]
> 完全なサンプルは[WebhookReceiverSample](https://github.com/nogic1008/Kaonavi.NET/tree/main/sandbox/WebhookReceiverSample)を参照してください。

## Development & Contributing

[CONTRIBUTING.md](https://github.com/nogic1008/Kaonavi.NET/blob/main/CONTRIBUTING.md)を参照してください。
