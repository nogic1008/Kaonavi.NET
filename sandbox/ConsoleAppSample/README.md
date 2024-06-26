# ConsoleAppSample

[System.CommandLine](https://github.com/dotnet/command-line-api)を利用した、`Kaonavi.Net`のサンプルプログラムです。

## Setup

1. カオナビにログインします。
1. 管理者機能トップ > [公開API v2 情報](https://service.kaonavi.jp/setup/public_api_v2_information) > 認証情報 を開きます。
1. `Consumer Key` と `Consumer Secret` を確認します。
1. [appsettings.json](./appsettings.json)を以下のように編集します。

    ```jsonc
    {
      "KaonaviOptions": {
        "ConsumerKey": "<3.で確認したConsumer Key>",
        "ConsumerSecret": "<3.で確認したConsumer Secret>",
        "UseDryRun": true // 実際に更新処理を行う場合はfalseにする
      }
    }
    ```

## Usage

```shell
# メンバー情報のレイアウトを取得
> ConsoleAppSample layout
# メンバー情報を全取得
> ConsoleAppSample download
# メンバー情報を更新
> ConsoleAppSample upload
# タスクの進捗状況を取得
> ConsoleAppSample progress -t 13
```

## See also

- [System.CommandLine の概要](https://learn.microsoft.com/dotnet/standard/commandline/)
- [.NET での汎用ホスト](https://learn.microsoft.com/dotnet/core/extensions/generic-host)
