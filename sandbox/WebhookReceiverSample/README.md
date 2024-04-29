# WebhookReceiverSample

`Kaonavi.NET.Server`を使用して、カオナビからのWebhookを受信するサンプルプログラムです。

## Setup

> [!IMPORTANT]
> Webhookを受信するためには、インターネットに公開されたWebサーバーが必要です。

1. [Webhook 設定 API](https://developer.kaonavi.jp/api/v2.0/index.html#tag/Webhook) または 管理者機能トップ > [公開API v2 情報](https://service.kaonavi.jp/setup/public_api_v2_information) > Webhook 設定で、任意のアクセス トークンを登録します。

2. `appsettings.json`の`KaonaviToken`に、1.で設定したものと同一のアクセス トークンを設定します。

```jsonc
{
  //...,
  "KaonaviToken": "YOUR_KAONAVI_TOKEN",
  //...
}
```

## Testing

Webhookを受信したときの処理をシミュレートすることができます。

1. このファイルがあるディレクトリに移動します。
2. `dotnet run`を実行します。
3. `http://localhost:5247/webhook`または`http://localhost:5247/webhook2`に対して、任意のRESTクライアントからPOSTリクエストを送信します。
  - [サンプル リクエスト](./WebhookReceiverSample.http)が準備されています。[Visual Studio Code](https://code.visualstudio.com/)の[REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)拡張機能などで利用可能です。

## See also

- [カオナビの公式ドキュメント](https://developer.kaonavi.jp/api/v2.0/index.html#section/Webhookb)
