# Kaonavi.NET — コーディングガイド

カオナビ API v2 向けの非公式 .NET ライブラリです。このファイルはコード生成・編集時に従うべきルールをまとめています。

- **カオナビ API v2 ドキュメント**: https://developer.kaonavi.jp/api/v2.0/index.html
  新しいエンドポイントを実装する際は必ずこのドキュメントを参照してください。

---

## プロジェクト構成

| ディレクトリ | 内容 |
|---|---|
| `src/Kaonavi.NET` | メタパッケージ（Core + Generator を束ねるだけ） |
| `src/Kaonavi.NET.Core` | API クライアント本体、エンティティ、JSON 変換 |
| `src/Kaonavi.NET.Generator` | `[SheetSerializable]` 属性用ソースジェネレーター |
| `src/Kaonavi.NET.Server` | ASP.NET Core Webhook 受信ライブラリ |
| `test/` | TUnit による単体テスト（各 src に対応） |
| `sandbox/` | サンプル |
| `docs/analyzer/` | アナライザー診断コードのドキュメント |

---

## 技術スタック

- **言語**: C# 14（`LangVersion=14.0`）
- **SDK**: .NET 10 以上（`global.json` で固定）
- **ターゲット**: .NET 8 以上（Generator のみ .NET Standard 2.0）
- **HTTPクライアント**: `System.Net.Http.HttpClient`
- **JSON**: `System.Text.Json`
- **テスト**: TUnit + TUnit.Mocks.Http

---

## コーディング規約

### 基本スタイル

- `.editorconfig` の設定に従う。自動修正は `dotnet format` で行う。
  - ファイルエンコーディングは **UTF-8**、改行は **LF**。
  - `this.` 修飾子は不要。
  - `int` / `string` 等の組み込み型エイリアスを使う（`Int32` は使わない）。組み込み型以外の変数宣言は `var` を使う。
  - 変数名・メンバー名は以下の定義に従う。
    - 型名, 定数, publicなフィールド変数: `PascalCase`
    - privateなフィールド変数: `_camelCase`
    - その他のメンバー: `Capitalized` (先頭のみ大文字, `When_Something_A_Throws_Exception`のような単体テスト用の命名を許容)
    - 引数, ローカル変数: `camelCase`
- クラス・メソッド・プロパティには XML ドキュメントコメントを付ける。特に公開 API には必須。
- **変数名・メンバー名に非 ASCII 文字（日本語等）を使わない**。
  - XMLドキュメントコメントやコード内コメントには日本語を使って構わない。
- `Nullable` を有効にしている。null 許容/非許容を明示し、特に理由のない限り、`!`などで無効にしない。
- `ImplicitUsings` を有効にしている。不要な `using` は追加しない。

### 非同期処理

- 公開 API の非同期メソッドは **`ValueTask<T>`** を返す（低アロケーション）。
- すべての非同期メソッドに `CancellationToken cancellationToken = default` を末尾に追加する。
- `await` には必ず `.ConfigureAwait(false)` を付ける（ライブラリコード）。
- メソッド名は `Async` サフィックスを付ける。

```csharp
// 良い例
public async ValueTask<Token> AuthenticateAsync(CancellationToken cancellationToken = default)
{
    var response = await _client.PostAsync("token", content, cancellationToken).ConfigureAwait(false);
    await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);
    return token!;
}
```

### エラーハンドリング

- 引数検証には `ArgumentNullException.ThrowIfNull()` / `ArgumentException.ThrowIfNullOrEmpty()` 等のThrowHelperを使う。
- 破棄済みオブジェクトの検出には `ObjectDisposedException.ThrowIf()` を使う。
- API エラーは `ApplicationException` でラップする（`ValidateApiResponseAsync` パターンを踏襲）。

### リソース管理

- `IDisposable` を実装する場合は [仮想メソッドパターン](https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-dispose) に従う。
  - `SemaphoreSlim` 等のリソースは `Dispose(bool disposing)` 内で解放する。
  - `HttpClient` などDIで注入されるオブジェクトは、クラス内で直接 `Dispose` しないこと。
- `TimeProvider` を使って時刻を抽象化し、テスト可能にする。

### AOT 互換性

- `src/Kaonavi.NET.Core` と `src/Kaonavi.NET.Server` は AOT 互換性を有効にしている。
  - AOT 互換性のある API のみを使用すること。特にリフレクションや動的コード生成は避ける。
- `System.Text.Json` のシリアライゼーションは `JsonSerializerContext`（ソースジェネレーター）経由で行う。

---

## 公開 API の管理

- 公開 API は `Microsoft.CodeAnalysis.PublicApiAnalyzers` の診断に従って管理する（手動で列挙しない）。
- 公開 API の追加・変更時は `dotnet build` を実行し、診断が要求する内容を `PublicAPI.Unshipped.txt` に反映する。
- リリース時に `PublicAPI.Shipped.txt` へ移動する。
- `Microsoft.CodeAnalysis.PublicApiAnalyzers` が破壊的変更を検出するため、メジャーバージョンを上げない限り、既存シグネチャは変更しない。

---

## APIメソッドの命名規則

| 操作 | メソッド名 |
|---|---|
| 一覧取得 | `ListAsync()` |
| 単一取得 | `ReadAsync()` |
| 新規作成 | `CreateAsync()` |
| 更新 | `UpdateAsync()` |
| 削除 | `DeleteAsync()` |
| 追加（リスト操作） | `AddAsync()` |
| 削除（リスト操作） | `RemoveAsync()` |

---

## コード改修時の必須手順

コードを変更した後は、**必ず以下の順で実行**してください。すべてが通ることを確認してからコミットしてください。

1. **エディター上のエラーがないことを確認する**（赤波線・コンパイルエラーがゼロであること）
2. **フォーマット修正**: `dotnet format`
3. **テスト実行**: `dotnet test`

機能追加・バグ修正・リファクタリングにかかわらず、変更内容に対応するテストを**必ず**追加または更新してください。テストのないコード変更は原則受け入れません。

---

## ビルドとテスト

```powershell
# フォーマットチェック（CI でも実行される）
dotnet format --verify-no-changes

# フォーマット自動修正
dotnet format

# ビルド
dotnet build

# テスト実行
dotnet test

# カバレッジ付きテスト
dotnet test --coverage --coverage-output-format cobertura
```

CI は Ubuntu / Windows / macOS × x64 / ARM のマトリクスで実行される。ローカルでは `dotnet test` が通れば十分。

---

## コミットメッセージ

[Angular.js の規約](https://github.com/angular/angular.js/blob/master/DEVELOPERS.md#type)に準拠：

```
<修飾子>(<スコープ>): <概要（英語推奨）>
<空行>
<詳細（任意）>
```

修飾子一覧: `feat` / `fix` / `docs` / `style` / `refactor` / `perf` / `test` / `build` / `ci` / `chore`

---

## 個別ルール（`.github/instructions/`）

詳細なルールは用途別に切り出されています。該当ファイルを編集する際は自動的に適用されます。

| ファイル | 適用対象 |
|---|---|
| [testing.instructions.md](.github/instructions/testing.instructions.md) | `test/**` — TUnit テストコード |
| [source-generator.instructions.md](.github/instructions/source-generator.instructions.md) | `src/Kaonavi.NET.Generator/**` — ソースジェネレーター |
| [entity-design.instructions.md](.github/instructions/entity-design.instructions.md) | `src/Kaonavi.NET.Core/Entities/**` |
| [public-api.instructions.md](.github/instructions/public-api.instructions.md) | `src/Kaonavi.NET.Core/PublicAPI.*.txt` |
| [client-implementation.instructions.md](.github/instructions/client-implementation.instructions.md) | `src/Kaonavi.NET.Core/KaonaviClient.*.cs`, `src/Kaonavi.NET.Core/IKaonaviClient.cs` |
