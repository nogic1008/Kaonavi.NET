# HOW TO CONTRIBUTE

当プロジェクトに関心を寄せていただき、ありがとうございます。このプロジェクトはオープン ソースであるため、誰でもプロジェクトに貢献することができます。
皆様がプロジェクトへの貢献を円滑に行えるよう、以下の事項を守っていただけますようお願いします。

## Getting Started

- このプロジェクトに貢献する為には[GitHub アカウント](https://github.com/signup/free)が必要です。
- ソースコードの変更を伴う場合は、[For Developer](#for-developer)の手順に従って開発環境をセットアップしてください。

## For Developer

### Requirements

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (9.0.200以降) および [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) が必要です。
- ソース ジェネレーター部分(`Kaonavi.NET.Generator`)のデバッグには Visual Studio 2022 (バージョン 17.3)以降が必要です。

#### Recommended

下記のいずれかの環境を利用することを推奨します。[Rider](https://www.jetbrains.com/rider/)やその他のエディタを利用する場合は、各エディタの設定に従ってください。

- Visual Studio 2022 (バージョン 17.3)以降
  - 当プロジェクトはオープンソースのため、[Visual Studio 2022 Community](https://visualstudio.microsoft.com/downloads/)が利用可能です。
  - .NET 9.0 (9.0.200以降) および .NET 8.0 SDK がインストール内容に含まれているか確認してください。
  - ソース ジェネレーター部分をデバッグするためには、.NET Compiler Platform SDK を追加でインストールしてください。
- [Visual Studio Code](https://code.visualstudio.com/) + [EditorConfig 拡張機能](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig) + [C# Dev Kit](https://learn.microsoft.com/visualstudio/subscriptions/vs-c-sharp-dev-kit)
  - .NET 9.0 および .NET 8.0 SDK は個別にインストールしてください。
  - Gitのインストールが必要です。(Visual Studio Codeの初回実行時、インストールを促されます)
  - C# Dev Kitがライセンス上利用できない場合は、[C# 拡張機能](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)を代わりに使用できます。
- Dev Container
  - Visual Studio Codeの[Dev Containers 拡張機能](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)を利用して、Docker コンテナー内で開発を行うことができます。
    - Dockerを利用できる環境が必要です。
  - もしくは、[GitHub Codespaces](https://github.com/features/codespaces)を利用することもできます。

### Development Commands

[.NET CLI](https://docs.microsoft.com/dotnet/core/tools/)(SDKに付属)を利用して、下記のコマンドを実行できます。

```powershell
# 必要なパッケージの復元(test, buildコマンド内で行われるため、これらの呼び出し時には不要)
> dotnet restore

# コードフォーマットのチェックと修正
> dotnet format

# 単体テストの実行
> dotnet test

# ビルド
> dotnet build
```

## Send issue

- 重複する Issue がないかどうか、はじめに検索してください。
- 機能要望(新機能の追加、既存機能の変更など)には、**必ず** Issue を作成してください。
  - 小さなバグ修正やリファクタリングなどは、Issue を作成せずに直接 Pull Request を送っても構いません。ただし、規模が大きい場合は、事前に Issue を作成し、了解を取ってから作業を始めてください。
- Issue を送るのに、事前の連絡は必要ありません。
- Issue のタイトルと本文はできるだけ英語で書いてください。
  - 英語に慣れていない場合は日本語を使ってください。不正確な英語では、英語話者・日本語話者のどちらにも伝わりません。
- バグを Issue で報告する場合、バグを再現する為の説明、エラーの情報、環境を書いてください。
- 本文は明確に記述し、1 行のみの Issue を送ることは避けてください。

## Making Changes

- コードやドキュメントを変更する場合は、`main`ブランチから、トピック・ブランチを作ってください。(`issue_999`, `hotfix/some_bug`など)
- 変更の為にテストが必要ならば、そのテストも追加または変更してください。
- commit は合理的(ロジック単位)に分けてください。また目的と関係のないコードの変更は含めないでください(コードフォーマットの変更、不要コードの削除など)。
- commit メッセージが正しいフォーマットであることを確認してください。commit メッセージはできるだけ英語でお願いします。
- commit メッセージには、下記の修飾子を先頭につけてください。([angular.js/DEVELOPERS.md](https://github.com/angular/angular.js/blob/master/DEVELOPERS.md#type)に準じます)
  - **feat**: 新機能
  - **fix**: バグ修正
  - **docs**: ドキュメントの修正のみ
  - **style**: コードの意味を変更しない修正 (スペース・フォーマット・セミコロンのフォーマットなど)
  - **refactor**: バグや機能追加ではないコード修正
  - **perf**: コードの高速化に寄与する修正
  - **test**: テストの追加、または修正
  - **build**: ビルド構成の変更
  - **ci**: CI/CD構成の変更
  - **chore**: その他の変更 (ライブラリの更新など)

Git コミットメッセージの例:
```text
修飾子(サブカテゴリ): コミットの概要
<ここは空行>
3行目以降に、このコミットの詳細を記述します。
```

### Coding Style

- Lint ルールか、すでにあるコードのスタイルに準じます。
  - [EditorConfig](https://editorconfig.org/) を適用しているため、それに対応したエディタを使うことを推奨します。
  - コードのフォーマットは `dotnet format` コマンドで自動修正できます。
- ファイルのエンコーディングは UTF-8 とします。
- 非 ASCII 文字(日本語など)を変数名,メンバー名に使用しないでください。
  - XMLドキュメンテーションコメントを含む、コードのコメント部分に非 ASCII 文字を使うのは構いません。
- リソースファイルのデフォルト言語は英語とし、日本語のリソースファイルは `Resources.ja-jp.resx` として作成してください。

### CI/CD

- このプロジェクトは、[GitHub Actions](https://github.com/features/actions)を利用して CI/CD を行っています。
- **すべての** Pull Request と`main`ブランチへの push に対して、自動的に下記チェック処理が実行されます。
  - Pull Requestのマージには、チェック処理に合格している必要があります。

| Job Name | Description |
| --- | --- |
|[Lint](./.github/workflows/dotnet.yml#L16)|コードフォーマットのチェック|
|[Validate NuGet Lock Files](./.github/workflows/dotnet.yml#L28)|NuGet パッケージのロックファイル(`packages.lock.json`)の検証|
|[Debug Build & Test](./.github/workflows/dotnet.yml#L43)|デバッグ ビルドと単体テストの実行(Windows, MacOS, Linuxと.NET 8.0/9.0の各環境で実施)|

## Thanks

このガイドは、[MMP/CONTRIBUTING.md · sn0w75/MMP](https://github.com/sn0w75/MMP/blob/master/CONTRIBUTING.md)(現在リンク切れ)を参考にして作成しました。
