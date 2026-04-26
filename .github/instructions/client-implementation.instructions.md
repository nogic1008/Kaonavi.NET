---
applyTo: "{src/Kaonavi.NET.Core/KaonaviClient.*.cs,src/Kaonavi.NET.Core/IKaonaviClient.cs}"
---

# KaonaviClient 実装規約

`src/Kaonavi.NET.Core/KaonaviClient.*.cs` および `IKaonaviClient.cs` に適用されるルールです。

---

## ファイル分割の方針

`KaonaviClient` は **API エンドポイントのカテゴリ（タグ）単位でファイルを分割**する `partial class` 構成です。

| ファイル | 対応 API カテゴリ / エンドポイント |
|---|---|
| `KaonaviClient.cs` | 認証・基底クラス・共通処理 |
| `KaonaviClient.Member.cs` | `/members` |
| `KaonaviClient.Sheet.cs` | `/sheets` |
| `KaonaviClient.Layout.cs` | `/layouts` |
| `KaonaviClient.Task.cs` | `/tasks` |
| `KaonaviClient.Department.cs` | `/departments` |
| `KaonaviClient.User.cs` | `/users` |
| `KaonaviClient.Role.cs` | `/roles` |
| `KaonaviClient.AdvancedPermission.cs` | `/advanced_permissions` |
| `KaonaviClient.EnumOption.cs` | `/enum_options` |
| `KaonaviClient.Webhook.cs` | `/webhooks` |

新しいエンドポイントカテゴリを追加する場合は、対応する `KaonaviClient.{Category}.cs` ファイルを新規作成する。

また、各カテゴリファイルには次の委譲プロパティを必ず実装する。

```csharp
public I{Category} {Category} => this;
```

これにより `client.{Category}.XxxAsync(...)` 形式の呼び出しを可能にする。

メソッド本体はカテゴリインターフェースに対する**明示的実装**で記述する。

カテゴリ追加時は、次の 3 点を **必須** で同時に行う。

1. `KaonaviClient.{Category}.cs` を追加し、`public I{Category} {Category} => this;` を実装する。
2. `IKaonaviClient.cs` に `KaonaviClient.I{Category} {Category} { get; }` を追加する。
3. `PublicAPI.Unshipped.txt` に追加した公開 API シグネチャを記載する。

---

## partial class ファイルの構造

各ファイルは以下の構造を維持する：

```csharp
// 必要な using のみ（ImplicitUsings で追加済みのものは不要）
using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net;

// 1. クラス宣言に対応するインターフェースを列挙する
public partial class KaonaviClient : KaonaviClient.I{Category}
{
    // 2. client.{Category}.XxxAsync(...) のための委譲プロパティ
    public I{Category} {Category} => this;

    /// <summary>
    /// {カテゴリ名} API
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/{url-encoded-tag}"/>
    /// </summary>
    public interface I{Category}
    {
        // メソッド定義（XMLドキュメント + API リンク付き）
        public ValueTask<IReadOnlyList<{Category}>> ListAsync(CancellationToken cancellationToken = default);
    }

    // 3. メソッド実装は明示的インターフェース実装で記述する
    ValueTask<IReadOnlyList<{Category}>> I{Category}.ListAsync(CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, "{category}"), "{category}_data", Context.Default.IReadOnlyList{Category}, cancellationToken);
}
```

---

## インターフェース定義のルール

### 内部インターフェースとして定義する

各カテゴリのインターフェース（例: `IMember`）は `KaonaviClient` クラスの**内部に**定義する。`IKaonaviClient.cs` のような外部ファイルには書かない。

```csharp
public partial class KaonaviClient : KaonaviClient.IMember
{
    public interface IMember { ... }  // KaonaviClient の内側
}
```

### XML ドキュメントコメント

各メソッドに以下を付ける：
- `<summary>`: 処理の日本語説明
- `<see href="..."/>`: 対応する API ドキュメントの URL（`https://developer.kaonavi.jp/api/v2.0/...`）
- `<remarks>`: 制限事項・注意事項（更新リクエスト制限対象の場合は必ず記載）
- `<param name="cancellationToken">`: `<inheritdoc>` で統一
- `<param name="...">`: 引数の説明

```csharp
/// <summary>
/// 全てのメンバーの基本情報・所属(主務)・兼務情報を取得します。
/// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/..."/>
/// </summary>
/// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
public ValueTask<IReadOnlyList<MemberData>> ListAsync(CancellationToken cancellationToken = default);
```

---

## メソッド実装のルール

### 明示的インターフェース実装（必須）

カテゴリ API の公開メソッドは、`I{Category}` に対する明示的実装で記述する。

```csharp
public interface IEnumOption
{
    ValueTask<IReadOnlyList<EnumOption>> ListAsync(CancellationToken cancellationToken = default);
}

public IEnumOption EnumOption => this;

ValueTask<IReadOnlyList<EnumOption>> IEnumOption.ListAsync(CancellationToken cancellationToken)
    => CallApiAsync(new(HttpMethod.Get, "enum_options"), "custom_field_data", Context.Default.IReadOnlyListEnumOption, cancellationToken);
```

この設計の意図:

1. 利用者は `client.Example.ListAsync(...)` のようにカテゴリ起点で API を探索できる。
2. `client.ListAsync(...)` / `client.XXXAsync(...)` のようなメソッドが Intellisense に直接並ばず、候補が整理される。
3. カテゴリ境界が明確になり、同名メソッド衝突や API 肥大化を防げる。

### 戻り値

- **一覧取得** → `ValueTask<IReadOnlyList<T>>`
- **単一取得・作成・更新** → `ValueTask<T>`（タスク ID の場合は `ValueTask<int>`）
- **削除** → `ValueTask`

タスク ID を返す場合は `TaskProgress` から `Id` プロパティを取得して返す。

### 配列ラップ応答の扱い

カオナビ API が次のような「配列を一意のプロパティでラップしたオブジェクト」を返す場合は、
クライアントの公開戻り値ではラップを外し、`IReadOnlyList<T>` として返す。

```json
{ "member_data": [ ... ] }
```

この場合は `CallApiAsync(request, propertyName, typeInfo, cancellationToken)` オーバーロードを使い、
`propertyName` にラップされた配列プロパティ名（例: `member_data`, `role_data`, `custom_field_data`）を渡す。

### 更新リクエスト制限

`更新リクエスト制限の対象API` の場合は、`_semaphore` を使って同時実行数を制限する：

```csharp
ValueTask<int> IMember.CreateAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken)
    => CallTaskApiAsync(HttpMethod.Post, "members", payload, "member_data"u8, Context.Default.IReadOnlyListMemberData, cancellationToken);
```

### 共通 API 呼び出しの利用

- 認証（トークン付与）・共通エラーハンドリング・更新リクエスト制限を統一するため、`KaonaviClient.cs` の共通メソッドを必ず経由する。
    - 読み取り系: `CallApiAsync(...)`
    - タスクIDを返す更新系: `CallTaskApiAsync(...)`
    - 制限付き更新の低レベル呼び出し: `CallRequestLimitApiAsync(...)`
- `PostAsJsonAsync` / `PutAsJsonAsync` / `PatchAsJsonAsync` をカテゴリ実装から直接呼ばない。

### JSON シリアライゼーション

- `System.Text.Json` の **ソースジェネレーター**（`Context.Default.*`）を必ず使う。直接 `JsonSerializer` を呼ばない。
- レスポンスの読み取りは共通メソッド内部の `ReadFromJsonAsync(Context.Default.T, cancellationToken)` を利用する。

### `ConfigureAwait(false)`

すべての `await` に `.ConfigureAwait(false)` を付ける。

---

## `IKaonaviClient.cs` のルール

`IKaonaviClient` はプロパティとして各カテゴリインターフェースを公開する：

```csharp
public interface IKaonaviClient
{
    KaonaviClient.I{Category} {Category} { get; }
    // ...
}
```

新しいカテゴリを追加した場合、`IKaonaviClient` への対応プロパティ追記は **必須**。`KaonaviClient.{Category}.cs` だけを追加して `IKaonaviClient` を更新しない変更は受け入れない。
