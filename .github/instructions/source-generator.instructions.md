---
applyTo: "src/Kaonavi.NET.Generator/**"
---

# ソースジェネレーター開発規約

`src/Kaonavi.NET.Generator/` 配下のコードに適用されるルールです。

---

## 基本方針

- **`IIncrementalGenerator`** インターフェースを実装する（`ISourceGenerator` は使わない）。
- ソースジェネレーターの実装は、Roslyn Source Generator の基本的な構成・責務分離（入力収集 / 変換 / 出力）に準ずる。
- ジェネレーターの出力は **再現可能（決定論的）** にする。同じ入力には必ず同じ出力。
- キャッシュを最大限に活用し、不要な再計算を避ける。

### `Microsoft.CodeAnalysis.CSharp` のバージョン管理

- `Microsoft.CodeAnalysis.CSharp` のバージョンは **むやみに上げない**。
  - ソースジェネレーターは Visual Studio や .NET SDK に同梱された Roslyn ホスト上で動作する。参照するバージョンが Roslyn ホストより新しい場合、型の不一致でロードエラーが発生する。
  - `.csproj` の `VersionOverride` で古いバージョンに固定しているのはこのためである（コメント "Pin version to use old C# compiler" 参照）。
- バージョンを上げる場合は、サポート対象の Visual Studio / .NET SDK が同等以上のバージョンの Roslyn を同梱していることを確認してから行う。

### ソースジェネレーターの出力コード最適化方針

- この節は、**ソースジェネレーターが `context.AddSource(...)` で出力するコード**の方針を定義する。
- 出力先の C# 言語バージョンを判別し、そのバージョンで利用可能な中で最も効率的かつ可読性の高いコードを出力する。
- 例:
    - C# 12 以上: コレクション式（`[]`）を優先する。
    - C# 11 以下: 配列初期化子（`new[] { ... }`）にフォールバックする。
- 新しい構文を使う場合は、古い言語バージョン向けのフォールバックを同時に実装し、生成失敗を防ぐ。

---

## `Initialize` メソッドの構造

```csharp
public void Initialize(IncrementalGeneratorInitializationContext context)
{
    // 1. 対象構文をフィルタリング（SyntaxProvider）
    var typeDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
        Consts.SheetSerializable,               // 属性の完全修飾名
        predicate: static (node, token) => ..., // 構文レベルの高速フィルタ
        transform: static (context, token) => ...  // セマンティック情報へ変換
    );

    // 2. 必要に応じて他のプロバイダと結合
    var source = typeDeclarations
        .Combine(context.CompilationProvider)
        .WithComparer(Comparer.Instance)       // キャッシュのためにカスタムEqualityComparerを使う
        .Combine(context.ParseOptionsProvider...);

    // 3. ソース出力を登録
    context.RegisterSourceOutput(source, static (context, source) => Emit(...));
}
```

---

## アロケーション最小化

ソースジェネレーターはビルド中に高頻度で呼び出されるため、不要なアロケーションを避けることが重要。

- ラムダには **`static`** を付ける（クロージャによるヒープアロケーションを防ぐ）。
- `new` するオブジェクトを減らす。インスタンスを使い回せる場合はシングルトン（`static readonly`）にする。
- 文字列結合には `StringBuilder` や補間文字列ハンドラーを活用し、中間文字列を生成しない。
- ラムダ内では必ず `token.ThrowIfCancellationRequested()` を先頭で呼ぶ。

---

## 不要な再実行の抑制（EqualityComparer）

Incremental Generator のパイプラインは、各ステップの出力が「前回と等しい」と判定された時点でそれ以降のステップを呼び出さない。したがって **適切な等値性を定義することが、ソース生成の無駄な再呼び出しを減らす最も効果的な手段**である。

- `Combine` 後の型が独自型の場合、`IEqualityComparer<T>` を実装して `.WithComparer()` を渡す。
- `Equals` が `true` を返せば下流のステップ（変換・出力）は一切呼ばれない。比較コストは最小限にする。
- `Comparer` 自体のアロケーションを避けるため `static readonly` シングルトンにする。

```csharp
private class Comparer : IEqualityComparer<(TypeDeclarationSyntax, Compilation)>
{
    public static readonly Comparer Instance = new();
    public bool Equals(...) => x.Item1.Equals(y.Item1);  // シンタックスノードで比較
    public int GetHashCode(...) => obj.Item1.GetHashCode();
}
```

---

## 診断（エラー / 警告）の発行

- 診断コードは `DiagnosticDescriptors.cs` に集約して定義する。
- コードは `KAONAVI000` の形式。新しい診断コードを追加する際は、対応するドキュメントを `docs/analyzer/KAONAVI000.md` に作成する。
  - 既存のドキュメント（例: `docs/analyzer/KAONAVI001.md`）を参考にして内容を記述する。
- リソース文字列はデフォルトを英語（`Resources.resx`）、日本語訳を `Resources.ja-JP.resx` に記述する。
  - `Resources.Designer.cs` は自動生成されるため手動編集しない。

```csharp
// DiagnosticDescriptors.cs に追加する例
internal static readonly DiagnosticDescriptor KAONAVI999 = new(
    id: "KAONAVI999",
    title: new LocalizableResourceString(nameof(Resources.KAONAVI999Title), Resources.ResourceManager, typeof(Resources)),
    messageFormat: new LocalizableResourceString(nameof(Resources.KAONAVI999Message), Resources.ResourceManager, typeof(Resources)),
    category: "Usage",
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true);
```

### `AnalyzerReleases.*.md` の管理

診断ルールは `Microsoft.CodeAnalysis.Analyzers` によって `AnalyzerReleases.*.md` で管理されている。

- **`AnalyzerReleases.Unshipped.md`**: Analyzer が自動生成するため、**手動編集しない。**ビルド後に追加した診断コードが反映されていることを確認する。
- **`AnalyzerReleases.Shipped.md`**: リリース時に `AnalyzerReleases.Unshipped.md` の内容を移動する。その際、各エントリの Notes 列に `docs/analyzer/KAONAVI000.md` へのリンクを追加する。
  - 通常の開発中は手動編集しない。

---

## コード生成（`Emit` メソッド）

- `context.AddSource(hintName, sourceText)` でファイルを追加する。
- `hintName` は衝突を避けるため完全修飾型名ベースにする（例: `Namespace.ClassName.g.cs`）。
- 生成コードは **`#nullable enable`** を先頭に含める。
- エラー時は `context.ReportDiagnostic()` を呼び、`AddSource` は呼ばない。

---

## テスト

ジェネレーターのテストは `test/Kaonavi.NET.Generator.Tests/` に記述する。

- 単体テスト: `Microsoft.CodeAnalysis.CSharp` の `CSharpGeneratorDriver` を使い、診断と生成コードのスナップショットを検証する。
- 結合テスト: 生成後のコードを含むコンパイル結果に対して、生成メソッドが実際に呼び出せること・期待どおりに動作することを検証する。
- 言語バージョン分岐がある場合は、分岐の発生するバージョン（例: C# 9/11/12）で期待出力と動作の両方を確認する。
