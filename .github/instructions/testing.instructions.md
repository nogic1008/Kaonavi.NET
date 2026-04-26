---
applyTo: "test/**"
---

# テストコード規約（TUnit）

`test/` 配下のすべてのテストコードに適用されるルールです。

---

## テストフレームワーク

- **TUnit** を使用する（MSTest / xUnit などは使わない）。
- HTTP モックは **TUnit.Mocks.Http** の `MockHttpClient` を使う。
- 時刻制御は **Microsoft.Extensions.TimeProvider.Testing** の `FakeTimeProvider` を使う。
- ランダムな値の生成は **RandomFixtureKit** を使う（必要に応じて）。

---

## テストクラスの構造

```csharp
/// <summary><see cref="TargetClass"/>の単体テスト</summary>
public sealed class TargetClassTest
{
    // 対応する本体クラス単位でテストクラスを分ける
}
```

- テストファイルは「本体のクラスファイル単位」で分ける（例: `KaonaviClient.Member.cs` に対して `KaonaviClient.Member.Test.cs`）。
- テストクラスを `partial` にする規約は、元コードや既存テストが `partial` 構成の場合にのみ適用する。
- `CreateSut` などのヘルパーメソッドは、対象テスト群で共通化メリットがある場合のみ導入する。

---

## テストメソッドの書き方

### テストメソッド名

テストメソッド名は **英語 + PascalCase + `_` 区切り** で記述し、次のパターンに従う。

```csharp
// 正常系（API呼び出しの検証）
{対象メソッド}_Calls_{HttpVerb}Api

// 異常系（ガード節・引数検証）
When_{条件}_{対象メソッド}_Throws_{例外型}
```

例:

```csharp
public async Task ListAsync_Calls_GetApi(...)
public async Task UpdateAsync_Calls_PatchApi(...)
public async Task When_Id_IsNegative_DeleteAsync_Throws_ArgumentOutOfRangeException(...)
```

- 「何を検証しているか」がメソッド名だけで分かるようにする。
- 1 メソッド 1 振る舞いを原則とし、複数条件を 1 つの名前に詰め込まない。

### `[Test]` 属性

`[Test]` の引数にはテストの**日本語の説明**を文字列で記述する。

```csharp
[Test($"{nameof(KaonaviClient)}(constructor) > {nameof(ArgumentNullException)}をスローする。")]
```

- 説明は `対象 > 条件 > 期待結果` の形式で記述する。
- `nameof()` を積極的に使い、リファクタリング耐性を持たせる。
- XMLドキュメントコメントでも同様の説明を付ける。

### `DisplayName`（`[Arguments]` との組み合わせ）

パラメータ化テストでは `DisplayName` を使って各ケースを明確にする：

```csharp
[Arguments(null, "foo", "consumerKey",
    DisplayName = $"{nameof(KaonaviClient)}(null, <non-null>) > ArgumentNullException(consumerKey)")]
```

### パラメータテストのデータ渡し

- 値が短く、可読性を損なわないケース（`bool`、`int`、短い文字列、enum など）は `Arguments` を使う。
- JSON の長文、複雑なオブジェクト、引数が多いケースは、`...TestData` という名前のメンバーデータを用意して `MethodDataSource` で渡す。

```csharp
// 短い値: Arguments
[Arguments(false, "/sheet_layouts")]
[Arguments(true, "/sheet_layouts?get_calc_type=true")]

// 長い値・複雑な値: ...TestData + MethodDataSource
public static IEnumerable<TestDataRow<(string json, int id)>> TestData { get { ... } }

[MethodDataSource(nameof(TestData))]
public async Task CanDeserializeJSON(string json, int id) { ... }
```

- どちらを使うか迷う場合は「属性行だけでテスト意図が読めるか」を基準にし、読みにくい場合は `...TestData` に切り替える。

### `[Category]` 属性

論理的なグループを `[Category]` で指定する（例: `"Constructor"`, `"Member"`, `"Sheet"`）。

---

## アサーション

TUnit の`Assert`クラスを使う。

```csharp
// 例外の検証
await Assert.That(() => someAction()).Throws<ArgumentNullException>().WithParameterName("param");

// 値の検証
await Assert.That(result).IsEqualTo(expected);
await Assert.That(collection).HasCount().EqualTo(3);
```

カスタムアサーションは `test/Kaonavi.NET.Core.Tests/Assertions/` に追加する。

---

## HTTP モックの使い方

```csharp
// Arrange
using var client = Mock.HttpClient("https://example.com/");
client.Handler.OnGet("/endpoint").Respond(HttpStatusCode.OK, "{\"ok\":true}");

var sut = CreateSut(client, "token");

// Act
var result = await sut.SomeAsync();

// Assert
await Assert.That(result).IsNotNull();
client.Handler.Verify(r => r.Method(HttpMethod.Get).Path("/endpoint"), Times.Once);
```

- `Mock.HttpClient(...)` でクライアントを作成し、`client.Handler.OnXxx(...)` で期待レスポンスを設定する。
- 実行後は `client.Handler.Verify(...)` で HTTP メソッド・パス・呼び出し回数を検証する。
- 引数検証テストなど HTTP を発行しないケースは、`OnAnyRequest().Respond(...)` を設定したうえで `client.Handler.Requests` が空であることを検証する。

---

## マルチターゲット

テストプロジェクトは .NET 8, 9, 10 全フレームワークで実行される。フレームワーク固有の API に依存しないこと。

---

## JSON 文字列リテラル

テスト内の JSON 文字列はコメントで型情報を示す：

```csharp
/*lang=json,strict*/
private const string SomeJson = """{"key":"value"}""";
```

`/*lang=json,strict*/` を付けると、IDE が JSON として認識して補完・検証を行う。

---

## テストデータの出典

エンティティのデシリアライズテストおよびクライアントの HTTP モックレスポンスには、**カオナビ API v2 公式ドキュメント（https://developer.kaonavi.jp/api/v2.0/index.html）に掲載されているサンプル JSON をそのまま使用する**。

- 実際の API レスポンスに近い形でテストできるため、仕様との乖離を早期に検出できる。
- サンプル JSON が存在しない場合は、ドキュメントのスキーマ定義に沿った JSON を自作する。

### エンティティのデシリアライズテスト

公式ドキュメントのレスポンス例 JSON をそのままテストデータとして使い、デシリアライズ結果が期待値と一致することを検証する。

### HTTP モックレスポンス

公式ドキュメントのレスポンス例 JSON を `client.Handler.OnXxx(...).RespondWithJson(...)` のボディに使う。
クライアントテストではエラーなくデシリアライズできることと、正しいエンドポイントへのリクエストが行われたことを確認する。（値の検証はエンティティのデシリアライズテストで行う）

```csharp
// 公式ドキュメントのレスポンス例をそのままモックに使う
client.Handler.OnGet("/members")
    .RespondWithJson(/*lang=json,strict*/ """{"members":[{"code":"A001","name":"山田太郎"}]}""");
```

---

## タイムアウト

アセンブリ全体に 20,000 ms のタイムアウトが設定されている。

- 単独で重い処理を行うテスト（長時間の待機・大量データの生成など）は書かない。
- 時刻や経過時間に依存するロジックのテストでは、`await Task.Delay(...)` など実際に時間を待つコードは使わない。`FakeTimeProvider` を DI して時刻を任意に進めて検証する。
